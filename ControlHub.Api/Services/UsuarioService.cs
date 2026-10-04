using ControlHub.Api.Data;
using ControlHub.Api.DTOs.Usuarios;
using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlHub.Api.Services;

public class UsuarioService
{
    private readonly ControlHubDbContext _context;

    public UsuarioService(ControlHubDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario> CriarUsuarioAsync(CriarUsuarioDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-mail é obrigatório.");

        if (string.IsNullOrWhiteSpace(dto.Senha))
            throw new ArgumentException("Senha é obrigatória.");

        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Nome é obrigatório.");

        var emailExistente = await _context.Usuarios
            .AnyAsync(x => x.Email.ToLower() == email);

        if (emailExistente)
            throw new ArgumentException(
                "Já existe um usuário cadastrado com este e-mail.");

        var empresaExiste = await _context.Empresas
            .AnyAsync(x => x.Id == dto.EmpresaId);

        if (!empresaExiste)
            throw new ArgumentException(
                "A empresa informada não existe.");

        if (dto.Perfil == Perfil.Master)
            throw new ArgumentException(
                "Não é permitido criar outro usuário Master.");

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome.Trim(),
            Email = email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            Perfil = dto.Perfil,
            Ativo = true,
            CriadoEm = DateTime.UtcNow,
            EmpresaId = dto.EmpresaId
        };

        _context.Usuarios.Add(usuario);

        await _context.SaveChangesAsync();

        return usuario;
    }
}