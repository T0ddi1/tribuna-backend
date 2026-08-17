using Microsoft.AspNetCore.Identity;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Services;

public static class SeedService
{
    // Cria as roles e, se ainda não existir nenhum usuário, o primeiro Admin —
    // sempre a partir de variáveis de ambiente/user-secrets, nunca de senha fixa no código.
    public static async Task ExecutarAsync(IServiceProvider services, IConfiguration config, ILogger logger)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in Roles.Todas)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        if (userManager.Users.Any())
        {
            return;
        }

        var adminEmail = config["Admin:Email"];
        var adminSenha = config["Admin:Senha"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminSenha))
        {
            logger.LogWarning(
                "Nenhum usuário encontrado e Admin:Email/Admin:Senha não configurados. " +
                "Defina essas variáveis (user-secrets ou ambiente) e reinicie para criar o primeiro Admin.");
            return;
        }

        var admin = new ApplicationUser
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
        }
    }
}
