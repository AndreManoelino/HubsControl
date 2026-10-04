using ControlHub.Api.Data;
using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlHub.Api.Services;

public static class MasterInitializer
{
    public static async Task CriarMasterAsync(
        IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ControlHubDbContext>();

        var email = Environment.GetEnvironmentVariable("MASTER_EMAIL");
        var senha = Environment.GetEnvironmentVariable("MASTER_PASSWORD");

        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidOperationException(
                "MASTER_EMAIL não foi configurado.");

        if (string.IsNullOrWhiteSpace(senha))
            throw new InvalidOperationException(
                "MASTER_PASSWORD não foi configurado.");

        email = email.Trim().ToLower();

        var masterExiste = await context.Usuarios
            .AnyAsync(x => x.Perfil == Perfil.Master);

        if (masterExiste)
            return;

        var emailExiste = await context.Usuarios
            .AnyAsync(x => x.Email.ToLower() == email);

        if (emailExiste)
            throw new InvalidOperationException(
                "O e-mail informado para o Master já está sendo utilizado.");

        var master = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Administrador Master",
            Email = email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha),
            Perfil = Perfil.Master,
            Ativo = true,
            CriadoEm = DateTime.UtcNow,
            EmpresaId = null
        };

        context.Usuarios.Add(master);

        await context.SaveChangesAsync();
    }
}