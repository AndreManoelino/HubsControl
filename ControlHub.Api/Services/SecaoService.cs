using ControlHub.Api.Data;
using ControlHub.Api.DTOs.Secoes;
using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlHub.Api.Services;

public class SecaoService
{
    private readonly ControlHubDbContext _context;

    public SecaoService(ControlHubDbContext context)
    {
        _context = context;
    }

    public async Task<Secao> CriarAsync(Guid empresaId, CriarSecaoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new InvalidOperationException("Nome da seção é obrigatório.");

        var nome = dto.Nome.Trim();

        var existe = await _context.Secoes
            .AnyAsync(x =>
                x.EmpresaId == empresaId &&
                x.Nome.ToLower() == nome.ToLower());

        if (existe)
            throw new InvalidOperationException(
                "Já existe uma seção com esse nome.");

        var secao = new Secao
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            Nome = nome,
            Ativa = true,
            CriadaEm = DateTime.UtcNow
        };

        _context.Secoes.Add(secao);

        await _context.SaveChangesAsync();

        return secao;
    }

    public async Task<List<Secao>> ListarAsync(Guid empresaId)
    {
        return await _context.Secoes
            .Where(x => x.EmpresaId == empresaId)
            .OrderBy(x => x.Nome)
            .ToListAsync();
    }

    public async Task<Secao?> ObterPorIdAsync(
        Guid empresaId,
        Guid id)
    {
        return await _context.Secoes
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.EmpresaId == empresaId);
    }

    public async Task<Secao> AtualizarAsync(
        Guid empresaId,
        Guid id,
        AtualizarSecaoDto dto)
    {
        var secao = await ObterPorIdAsync(empresaId, id);

        if (secao is null)
            throw new InvalidOperationException(
                "Seção não encontrada.");

        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new InvalidOperationException(
                "Nome da seção é obrigatório.");

        var nome = dto.Nome.Trim();

        var existe = await _context.Secoes
            .AnyAsync(x =>
                x.EmpresaId == empresaId &&
                x.Id != id &&
                x.Nome.ToLower() == nome.ToLower());

        if (existe)
            throw new InvalidOperationException(
                "Já existe uma seção com esse nome.");

        secao.Nome = nome;
        secao.Ativa = dto.Ativa;

        await _context.SaveChangesAsync();

        return secao;
    }

    public async Task BloquearAsync(
        Guid empresaId,
        Guid id)
    {
        var secao = await ObterPorIdAsync(empresaId, id);

        if (secao is null)
            throw new InvalidOperationException(
                "Seção não encontrada.");

        secao.Ativa = false;

        await _context.SaveChangesAsync();
    }
}