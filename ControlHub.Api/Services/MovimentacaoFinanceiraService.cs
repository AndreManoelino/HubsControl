using ControlHub.Api.Data;
using ControlHub.Api.DTOs.Financeiro;
using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlHub.Api.Services;

public class MovimentacaoFinanceiraService
{
    private readonly ControlHubDbContext _context;

    public MovimentacaoFinanceiraService(ControlHubDbContext context)
    {
        _context = context;
    }

    public async Task<MovimentacaoFinanceira> CriarAsync(
        Guid empresaId,
        CriarMovimentacaoFinanceiraDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Descricao))
            throw new InvalidOperationException(
                "Descrição é obrigatória.");

        if (dto.Valor <= 0)
            throw new InvalidOperationException(
                "O valor deve ser maior que zero.");

        var movimentacao = new MovimentacaoFinanceira
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            Categoria = dto.Categoria,
            Descricao = dto.Descricao.Trim(),
            Valor = dto.Valor,
            Entrada = dto.Entrada,
            CriadaEm = DateTime.UtcNow
        };

        _context.MovimentacoesFinanceiras.Add(movimentacao);

        await _context.SaveChangesAsync();

        return movimentacao;
    }

    public async Task<List<MovimentacaoFinanceira>> ListarAsync(
        Guid empresaId)
    {
        return await _context.MovimentacoesFinanceiras
            .Where(x => x.EmpresaId == empresaId)
            .OrderByDescending(x => x.CriadaEm)
            .ToListAsync();
    }

    public async Task<MovimentacaoFinanceira?> ObterPorIdAsync(
        Guid empresaId,
        Guid id)
    {
        return await _context.MovimentacoesFinanceiras
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.EmpresaId == empresaId);
    }

    public async Task<decimal> TotalEntradasAsync(
        Guid empresaId)
    {
        return await _context.MovimentacoesFinanceiras
            .Where(x =>
                x.EmpresaId == empresaId &&
                x.Entrada)
            .SumAsync(x => x.Valor);
    }

    public async Task<decimal> TotalSaidasAsync(
        Guid empresaId)
    {
        return await _context.MovimentacoesFinanceiras
            .Where(x =>
                x.EmpresaId == empresaId &&
                !x.Entrada)
            .SumAsync(x => x.Valor);
    }

    public async Task<decimal> SaldoAsync(
        Guid empresaId)
    {
        var entradas = await TotalEntradasAsync(empresaId);
        var saidas = await TotalSaidasAsync(empresaId);

        return entradas - saidas;
    }
}