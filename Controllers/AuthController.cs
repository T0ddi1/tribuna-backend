using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.DTOs;
using NewsPortal.Api.Models;
using NewsPortal.Api.Services;

namespace NewsPortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private const string RefreshCookieName = "refreshToken";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly TokenService _tokenService;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;
    private readonly IHostEnvironment _environment;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        TokenService tokenService,
        ApplicationDbContext context,
        IConfiguration config,
        ILogger<AuthController> logger,
        IHostEnvironment environment)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _context = context;
        _config = config;
        _logger = logger;
        _environment = environment;
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var usuario = await _userManager.FindByEmailAsync(dto.Email);

        // Mensagem genérica em todos os casos de falha: evita que um atacante
        // descubra se o e-mail existe (enumeração de contas).
        var mensagemInvalida = new { mensagem = "E-mail ou senha inválidos." };

        if (usuario is null || !usuario.Ativo)
        {
            return Unauthorized(mensagemInvalida);
        }

        var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, dto.Senha, lockoutOnFailure: true);

        if (resultado.IsLockedOut)
        {
            _logger.LogWarning("Login bloqueado por excesso de tentativas: {Email}", dto.Email);
            return Unauthorized(new { mensagem = "Conta temporariamente bloqueada por excesso de tentativas. Tente novamente mais tarde." });
        }

        if (!resultado.Succeeded)
        {
            return Unauthorized(mensagemInvalida);
        }

        var roles = await _userManager.GetRolesAsync(usuario);
        var resposta = await EmitirTokensAsync(usuario, roles);
        return Ok(resposta);
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<AuthResponseDto>> Refresh()
    {
        if (!Request.Cookies.TryGetValue(RefreshCookieName, out var refreshBruto) || string.IsNullOrEmpty(refreshBruto))
        {
            return Unauthorized(new { mensagem = "Sessão inválida." });
        }

        var hash = TokenService.CalcularHash(refreshBruto);
        var tokenExistente = await _context.RefreshTokens
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.TokenHash == hash);

        if (tokenExistente is null || tokenExistente.Usuario is null || !tokenExistente.Usuario.Ativo)
        {
            Response.Cookies.Delete(RefreshCookieName, MontarOpcoesCookieBase());
            return Unauthorized(new { mensagem = "Sessão inválida." });
        }

        if (!tokenExistente.Ativo)
        {
            // Token reaproveitado ou expirado: revoga toda a família por segurança
            // (indício de reuse attack — token pode ter sido roubado).
            await RevogarTodosTokensAsync(tokenExistente.UserId, "reuse-detectado");
            Response.Cookies.Delete(RefreshCookieName, MontarOpcoesCookieBase());
            return Unauthorized(new { mensagem = "Sessão inválida." });
        }

        tokenExistente.RevogadoEm = DateTime.UtcNow;
        tokenExistente.RevogadoPorIp = HttpContext.Connection.RemoteIpAddress?.ToString();

        var roles = await _userManager.GetRolesAsync(tokenExistente.Usuario);
        var resposta = await EmitirTokensAsync(tokenExistente.Usuario, roles, tokenExistente);
        return Ok(resposta);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        if (Request.Cookies.TryGetValue(RefreshCookieName, out var refreshBruto) && !string.IsNullOrEmpty(refreshBruto))
        {
            var hash = TokenService.CalcularHash(refreshBruto);
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash);
            if (token is not null && token.Ativo)
            {
                token.RevogadoEm = DateTime.UtcNow;
                token.RevogadoPorIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                await _context.SaveChangesAsync();
            }
        }

        Response.Cookies.Delete(RefreshCookieName, MontarOpcoesCookieBase());
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UsuarioResponseDto>> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var usuario = await _userManager.FindByIdAsync(userId);
        if (usuario is null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(usuario);
        return Ok(new UsuarioResponseDto
        {
            Id = usuario.Id,
            NomeCompleto = usuario.NomeCompleto,
            Email = usuario.Email!,
            Ativo = usuario.Ativo,
            Roles = roles,
        });
    }

    // Não existe autocadastro público: só um Admin já autenticado cria novas contas
    // de Editor/Admin. Evita que qualquer visitante crie uma conta com privilégios.
    [HttpPost("usuarios")]
    [Authorize(Roles = Models.Roles.Admin)]
    public async Task<ActionResult<UsuarioResponseDto>> CriarUsuario(CriarUsuarioDto dto)
    {
        if (!Models.Roles.Todas.Contains(dto.Role))
        {
            return BadRequest(new { mensagem = "Role inválida." });
        }

        var existente = await _userManager.FindByEmailAsync(dto.Email);
        if (existente is not null)
        {
            return Conflict(new { mensagem = "Já existe um usuário com este e-mail." });
        }

        var usuario = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            NomeCompleto = dto.NomeCompleto,
            EmailConfirmed = true,
        };

        var resultado = await _userManager.CreateAsync(usuario, dto.Senha);
        if (!resultado.Succeeded)
        {
            return BadRequest(resultado.Errors.Select(e => e.Description));
        }

        await _userManager.AddToRoleAsync(usuario, dto.Role);

        return Ok(new UsuarioResponseDto
        {
            Id = usuario.Id,
            NomeCompleto = usuario.NomeCompleto,
            Email = usuario.Email!,
            Ativo = usuario.Ativo,
            Roles = [dto.Role],
        });
    }

    [HttpGet("usuarios")]
    [Authorize(Roles = Models.Roles.Admin)]
    public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> ListarUsuarios()
    {
        var usuarios = await _userManager.Users.OrderBy(u => u.NomeCompleto).ToListAsync();

        var resultado = new List<UsuarioResponseDto>();
        foreach (var usuario in usuarios)
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            resultado.Add(new UsuarioResponseDto
            {
                Id = usuario.Id,
                NomeCompleto = usuario.NomeCompleto,
                Email = usuario.Email!,
                Ativo = usuario.Ativo,
                Roles = roles,
            });
        }

        return Ok(resultado);
    }

    // Desativar em vez de excluir: preserva a autoria dos artigos já publicados
    // e revoga todas as sessões ativas da conta imediatamente.
    [HttpPost("usuarios/{id}/status")]
    [Authorize(Roles = Models.Roles.Admin)]
    public async Task<IActionResult> AlterarStatusUsuario(string id, [FromBody] bool ativo)
    {
        var idAtual = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id == idAtual && !ativo)
        {
            return BadRequest(new { mensagem = "Você não pode desativar a própria conta." });
        }

        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario is null)
        {
            return NotFound();
        }

        usuario.Ativo = ativo;
        await _userManager.UpdateAsync(usuario);

        if (!ativo)
        {
            await RevogarTodosTokensAsync(usuario.Id, "conta-desativada");
        }

        return NoContent();
    }

    private async Task<AuthResponseDto> EmitirTokensAsync(ApplicationUser usuario, IList<string> roles, RefreshToken? tokenAnterior = null)
    {
        var (accessToken, expiraEm) = _tokenService.GerarAccessToken(usuario, roles);

        var refreshBruto = TokenService.GerarRefreshTokenBruto();
        var refreshDias = int.Parse(_config["Jwt:RefreshTokenDias"] ?? "7");
        var novoToken = new RefreshToken
        {
            TokenHash = TokenService.CalcularHash(refreshBruto),
            UserId = usuario.Id,
            ExpiraEm = DateTime.UtcNow.AddDays(refreshDias),
            CriadoPorIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
        };

        if (tokenAnterior is not null)
        {
            tokenAnterior.SubstituidoPorHash = novoToken.TokenHash;
        }

        _context.RefreshTokens.Add(novoToken);
        await _context.SaveChangesAsync();

        var opcoesCookie = MontarOpcoesCookieBase();
        opcoesCookie.Expires = novoToken.ExpiraEm;
        Response.Cookies.Append(RefreshCookieName, refreshBruto, opcoesCookie);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            ExpiraEm = expiraEm,
            Email = usuario.Email!,
            NomeCompleto = usuario.NomeCompleto,
            Roles = roles,
        };
    }

    private CookieOptions MontarOpcoesCookieBase() => new()
    {
        HttpOnly = true,
        // Secure exige HTTPS — em dev local (http://localhost) isso impediria o navegador
        // de enviar o cookie de volta, então relaxamos apenas fora de produção/staging.
        Secure = !_environment.IsDevelopment(),
        // Frontend (otribuna.com.br) e backend (railway.app) são domínios diferentes,
        // então o cookie é cross-site: SameSite=Strict/Lax nunca seria enviado em
        // chamadas via fetch/XHR. None exige Secure=true, por isso só em produção.
        SameSite = _environment.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.None,
        Path = "/api/auth",
    };

    private async Task RevogarTodosTokensAsync(string userId, string motivo)
    {
        var tokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevogadoEm == null)
            .ToListAsync();

        foreach (var t in tokens)
        {
            t.RevogadoEm = DateTime.UtcNow;
        }

        if (tokens.Count > 0)
        {
            _logger.LogWarning("Revogados {Qtd} refresh tokens do usuário {UserId}: {Motivo}", tokens.Count, userId, motivo);
            await _context.SaveChangesAsync();
        }
    }
}
