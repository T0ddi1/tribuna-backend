using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Services;

public static class SeedService
{
    // Editorias fixas do site (correspondem às rotas /capital, /esportes, /tech, /gg
    // no frontend) — criadas de fábrica pra já existir algo pra selecionar no admin.
    private static readonly (string Nome, string Slug, string Tagline, string CorAccent)[] VerticaisPadrao =
    [
        ("Capital", "capital", "Economia, mercado e negócios", "#d97706"),
        ("Esportes", "esportes", "O que move o esporte", "#16a34a"),
        ("Tech", "tech", "Tecnologia e inovação", "#2563eb"),
        ("Games", "gg", "Games e cultura gamer", "#9333ea"),
    ];

    // Artigos de exemplo — só pra time novo já ver conteúdo real ao rodar pela
    // primeira vez, sem precisar cadastrar tudo na mão. (VerticalSlug, Titulo, Resumo, Html, Destaque)
    private static readonly (string VerticalSlug, string Titulo, string Resumo, string Html, bool Destaque)[] ArtigosPadrao =
    [
        ("capital", "Mercado fecha semana em alta puxado por tecnologia",
            "Índices futuros sobem com resultados acima do esperado das grandes empresas do setor.",
            "<p>O mercado encerrou a semana em alta, impulsionado principalmente pelo desempenho de empresas de tecnologia que superaram as expectativas de analistas.</p><p>Investidores monitoram agora os próximos indicadores econômicos para avaliar a continuidade da tendência.</p>",
            true),
        ("capital", "Novo marco regulatório para fintechs entra em vigor",
            "Mudanças afetam diretamente como startups financeiras operam no país.",
            "<p>A partir deste mês, novas regras passam a valer para empresas de tecnologia financeira, com impacto direto em processos de abertura de conta e crédito digital.</p>",
            false),
        ("esportes", "Seleção confirma escalação para as próximas rodadas",
            "Comissão técnica revela detalhes da preparação para os próximos desafios.",
            "<p>Depois de semanas de treinamento intenso, a comissão técnica definiu a escalação que entra em campo nas próximas partidas decisivas.</p>",
            true),
        ("esportes", "Campeonato regional bate recorde de público neste ano",
            "Arenas lotadas marcam a temporada mais assistida da história da competição.",
            "<p>O torneio regional registrou o maior público pago de sua história, reflexo do investimento em infraestrutura e na base de torcedores.</p>",
            false),
        ("tech", "Inteligência artificial já responde por parte crescente das buscas",
            "Ferramentas de IA generativa mudam o comportamento de consumo de informação.",
            "<p>Pesquisas recentes apontam crescimento acelerado no uso de assistentes de IA para tarefas que antes dependiam de buscadores tradicionais.</p><p>Especialistas debatem os impactos dessa mudança para produtores de conteúdo.</p>",
            true),
        ("tech", "Startups brasileiras atraem novo ciclo de investimento",
            "Aportes internacionais voltam a crescer após período de retração no setor.",
            "<p>Fundos internacionais voltam a olhar para o ecossistema brasileiro de startups, sinalizando retomada de confiança no mercado local.</p>",
            false),
        ("gg", "Novo lançamento domina as paradas de jogos mais jogados",
            "Título figura entre os mais comentados da semana nas redes.",
            "<p>O lançamento mais recente do gênero já figura entre os mais jogados da semana, com destaque também nas transmissões ao vivo.</p>",
            true),
        ("gg", "Cena competitiva nacional cresce com novos torneios",
            "Premiações e patrocínios aumentam o profissionalismo do circuito local.",
            "<p>O circuito competitivo nacional segue em expansão, com mais torneios, maiores premiações e crescente atenção de patrocinadores.</p>",
            false),
    ];

    // Cria as roles e, se ainda não existir nenhum usuário, o primeiro Admin —
    // sempre a partir de variáveis de ambiente/user-secrets, nunca de senha fixa no código.
    public static async Task ExecutarAsync(IServiceProvider services, IConfiguration config, ILogger logger)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var context = services.GetRequiredService<ApplicationDbContext>();

        if (!await context.Verticais.AnyAsync())
        {
            var ordem = 0;
            foreach (var (nome, slug, tagline, cor) in VerticaisPadrao)
            {
                context.Verticais.Add(new Vertical
                {
                    Nome = nome,
                    Slug = slug,
                    Tagline = tagline,
                    CorAccent = cor,
                    Ordem = ordem++,
                });
            }

            await context.SaveChangesAsync();
            logger.LogInformation("Verticais padrão criadas: {Verticais}", string.Join(", ", VerticaisPadrao.Select(v => v.Slug)));
        }

        foreach (var role in Roles.Todas)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        ApplicationUser? admin;

        if (userManager.Users.Any())
        {
            admin = null;
        }
        else
        {
            var adminEmail = config["Admin:Email"];
            var adminSenha = config["Admin:Senha"];

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminSenha))
            {
                logger.LogWarning(
                    "Nenhum usuário encontrado e Admin:Email/Admin:Senha não configurados. " +
                    "Defina essas variáveis (user-secrets ou ambiente) e reinicie para criar o primeiro Admin.");
                return;
            }

            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                NomeCompleto = "Administrador",
                EmailConfirmed = true,
            };

            var resultado = await userManager.CreateAsync(admin, adminSenha);
            if (resultado.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Roles.Admin);
                logger.LogInformation("Usuário Admin inicial criado para {Email}.", adminEmail);
            }
            else
            {
                logger.LogError(
                    "Falha ao criar Admin inicial: {Erros}",
                    string.Join("; ", resultado.Errors.Select(e => e.Description)));
                return;
            }
        }

        if (admin is not null && !await context.Artigos.AnyAsync())
        {
            var verticaisPorSlug = await context.Verticais.ToDictionaryAsync(v => v.Slug);
            var agora = DateTime.UtcNow;
            var indice = 0;

            foreach (var (verticalSlug, titulo, resumo, html, destaque) in ArtigosPadrao)
            {
                if (!verticaisPorSlug.TryGetValue(verticalSlug, out var vertical))
                {
                    continue;
                }

                context.Artigos.Add(new Artigo
                {
                    Slug = GerarSlugSeed(titulo),
                    Titulo = titulo,
                    Resumo = resumo,
                    ConteudoHtml = html,
                    Publicada = true,
                    Destaque = destaque,
                    PublicadoEm = agora.AddMinutes(-indice),
                    VerticalId = vertical.Id,
                    AutorId = admin.Id,
                });

                indice++;
            }

            await context.SaveChangesAsync();
            logger.LogInformation("{Qtd} artigos de exemplo criados.", ArtigosPadrao.Length);
        }
    }

    private static string GerarSlugSeed(string titulo)
    {
        var semAcento = titulo.Trim().ToLowerInvariant()
            .Replace('á', 'a').Replace('à', 'a').Replace('ã', 'a').Replace('â', 'a')
            .Replace('é', 'e').Replace('ê', 'e')
            .Replace('í', 'i')
            .Replace('ó', 'o').Replace('ô', 'o').Replace('õ', 'o')
            .Replace('ú', 'u').Replace('ü', 'u')
            .Replace('ç', 'c');

        var normalizado = System.Text.RegularExpressions.Regex.Replace(semAcento, "[^a-z0-9\\s-]", "");
        normalizado = System.Text.RegularExpressions.Regex.Replace(normalizado, "\\s+", "-").Trim('-');
        return normalizado;
    }
}
