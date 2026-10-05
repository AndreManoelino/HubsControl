using ControlHub.Api.Data;
using ControlHub.Api.DTOs.Empresas;
using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlHub.Api.Services;

public class EmpresaService
{
    private readonly ControlHubDbContext _context;

    public EmpresaService(ControlHubDbContext context)
    {
        _context = context;
    }

    public async Task<Empresa> CriarEmpresaAsync(CriarEmpresaDto dto)
    {
        var cpf = dto.Cpf
            .Trim()
            .Replace(".", "")
            .Replace("-", "");

        if (cpf.Length != 11 || !cpf.All(char.IsDigit))
            throw new ArgumentException("CPF inválido.");

        var email = dto.EmailDono.Trim().ToLower();

        var cpfExistente = await _context.Empresas
            .AnyAsync(x => x.Cpf == cpf);

        if (cpfExistente)
            throw new ArgumentException(
                "Já existe uma empresa cadastrada com este CPF.");

        var emailExistente = await _context.Usuarios
            .AnyAsync(x => x.Email.ToLower() == email);

        if (emailExistente)
            throw new ArgumentException(
                "Já existe um usuário cadastrado com este e-mail.");

        var empresa = new Empresa
        {
            Id = Guid.NewGuid(),
            Cpf = cpf,
            Nome = dto.Nome.Trim(),
            NomeFantasia = dto.NomeFantasia?.Trim(),
            LogoUrl = dto.LogoUrl?.Trim(),
            ImagemLoginUrl = dto.ImagemLoginUrl?.Trim(),
            Url = dto.Url.Trim(),
            Ativa = true,
            CriadaEm = DateTime.UtcNow,
            DonoId = null
        };

        _context.Empresas.Add(empresa);

        await _context.SaveChangesAsync();

        var dono = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = dto.NomeDono.Trim(),
            Email = email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.SenhaDono),
            Perfil = Perfil.Dono,
            Ativo = true,
            CriadoEm = DateTime.UtcNow,
            EmpresaId = empresa.Id
        };

        _context.Usuarios.Add(dono);

        await _context.SaveChangesAsync();

        empresa.DonoId = dono.Id;

        await _context.SaveChangesAsync();

        return empresa;
    }

    public async Task<Empresa?> BuscarPorIdAsync(Guid id)
    {
        return await _context.Empresas
            .Include(x => x.Dono)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.Ativa);
    }

    public async Task<Empresa?> BuscarPorDonoIdAsync(Guid donoId)
    {
        return await _context.Empresas
            .Include(x => x.Dono)
            .FirstOrDefaultAsync(x =>
                x.DonoId == donoId &&
                x.Ativa);
    }

    public async Task<List<Empresa>> BuscarPorNomeDonoAsync(string nome)
    {
        nome = nome.Trim().ToLower();

        return await _context.Empresas
            .Include(x => x.Dono)
            .Where(x =>
                x.Ativa &&
                x.Dono != null &&
                x.Dono.Nome.ToLower().Contains(nome))
            .ToListAsync();
    }

    public async Task<List<Empresa>> BuscarPorNomeAsync(string nome)
    {
        nome = nome.Trim().ToLower();

        return await _context.Empresas
            .Include(x => x.Dono)
            .Where(x =>
                x.Ativa &&
                (
                    x.Nome.ToLower().Contains(nome) ||
                    (x.NomeFantasia != null &&
                    x.NomeFantasia.ToLower().Contains(nome))
                ))
            .ToListAsync();
    }

    public async Task<Empresa?> EditarEmpresaAsync(
        Guid id,
        CriarEmpresaDto dto)
    {
        var empresa = await _context.Empresas
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.Ativa);

        if (empresa is null)
            return null;

        var cpf = dto.Cpf
            .Trim()
            .Replace(".", "")
            .Replace("-", "");

        if (cpf.Length != 11 || !cpf.All(char.IsDigit))
            throw new ArgumentException("CPF inválido.");

        var cpfExistente = await _context.Empresas
            .AnyAsync(x =>
                x.Cpf == cpf &&
                x.Id != id);

        if (cpfExistente)
            throw new ArgumentException(
                "Já existe uma empresa cadastrada com este CPF.");

        empresa.Cpf = cpf;
        empresa.Nome = dto.Nome.Trim();
        empresa.NomeFantasia = dto.NomeFantasia?.Trim();
        empresa.LogoUrl = dto.LogoUrl?.Trim();
        empresa.ImagemLoginUrl = dto.ImagemLoginUrl?.Trim();
        empresa.Url = dto.Url.Trim();

        await _context.SaveChangesAsync();

        return empresa;
    }

    public async Task<bool> BloquearEmpresaAsync(Guid id)
    {
        var empresa = await _context.Empresas
            .FirstOrDefaultAsync(x => x.Id == id);

        if (empresa is null)
            return false;

        empresa.Ativa = false;

        if (empresa.DonoId.HasValue)
        {
            var dono = await _context.Usuarios
                .FirstOrDefaultAsync(x =>
                    x.Id == empresa.DonoId.Value);

            if (dono is not null)
            {
                dono.Ativo = false;
            }
        }

        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<List<Empresa>> BuscarTodasAsync()
    {
        return await _context.Empresas
            .Include(x => x.Dono)
            .OrderBy(x => x.Nome)
            .ToListAsync();
    }

    public async Task<Empresa?> BuscarPublicaPorUrlAsync(string url)
    {
        if(string.IsNullOrWhiteSpace(url))
            return null;

            url = url.Trim().Trim('/').ToLower();
            return await _context.Empresas
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Url.ToLower() == url && x.Ativa);

    }
}