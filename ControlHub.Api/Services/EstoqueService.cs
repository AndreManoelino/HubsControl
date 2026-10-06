using ControlHub.Api.Data;
using ControlHub.Api.DTOs.Estoque;
using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlHub.Api.Services;

public class EstoqueService
{
    private readonly ControlHubDbContext _context;

    public EstoqueService(ControlHubDbContext context)
    {
        _context = context;
    }

    public async Task<Estoque> EntradaAsync(
        Guid empresaId,
        MovimentarEstoqueDto dto)
    {
        if (dto.Quantidade <= 0)
            throw new InvalidOperationException(
                "A quantidade deve ser maior que zero.");

        var estoque = await ObterEstoqueAsync(
            empresaId,
            dto.ProdutoId);

        if (estoque is null)
            throw new InvalidOperationException(
                "Estoque não encontrado.");

        var produto = await _context.Produtos
            .FirstOrDefaultAsync(x =>
                x.Id == dto.ProdutoId &&
                x.EmpresaId == empresaId);

        if (produto is null)
            throw new InvalidOperationException(
                "Produto não encontrado.");

        if (!produto.Ativo)
            throw new InvalidOperationException(
                "O produto está bloqueado.");

        if (!produto.ControlaEstoque)
            throw new InvalidOperationException(
                "Este produto não controla estoque.");

        var anterior = estoque.Quantidade;

        estoque.Quantidade += dto.Quantidade;
        estoque.AtualizadoEm = DateTime.UtcNow;

        _context.MovimentacoesEstoque.Add(
            new MovimentacaoEstoque
            {
                Id = Guid.NewGuid(),
                EmpresaId = empresaId,
                ProdutoId = dto.ProdutoId,
                Tipo = TipoMovimentacaoEstoque.Entrada,
                Quantidade = dto.Quantidade,
                QuantidadeAnterior = anterior,
                QuantidadeAtual = estoque.Quantidade,
                Observacao = dto.Observacao?.Trim(),
                CriadaEm = DateTime.UtcNow
            });

        await _context.SaveChangesAsync();

        return estoque;
    }

    public async Task<Estoque> SaidaAsync(
        Guid empresaId,
        MovimentarEstoqueDto dto)
    {
        if (dto.Quantidade <= 0)
            throw new InvalidOperationException(
                "A quantidade deve ser maior que zero.");

        var estoque = await ObterEstoqueAsync(
            empresaId,
            dto.ProdutoId);

        if (estoque is null)
            throw new InvalidOperationException(
                "Estoque não encontrado.");

        var produto = await _context.Produtos
            .FirstOrDefaultAsync(x =>
                x.Id == dto.ProdutoId &&
                x.EmpresaId == empresaId);

        if (produto is null)
            throw new InvalidOperationException(
                "Produto não encontrado.");

        if (!produto.Ativo)
            throw new InvalidOperationException(
                "O produto está bloqueado.");

        if (!produto.ControlaEstoque)
            throw new InvalidOperationException(
                "Este produto não controla estoque.");

        if (estoque.Quantidade < dto.Quantidade)
            throw new InvalidOperationException(
                "Estoque insuficiente.");

        var anterior = estoque.Quantidade;

        estoque.Quantidade -= dto.Quantidade;
        estoque.AtualizadoEm = DateTime.UtcNow;

        _context.MovimentacoesEstoque.Add(
            new MovimentacaoEstoque
            {
                Id = Guid.NewGuid(),
                EmpresaId = empresaId,
                ProdutoId = dto.ProdutoId,
                Tipo = TipoMovimentacaoEstoque.Saida,
                Quantidade = dto.Quantidade,
                QuantidadeAnterior = anterior,
                QuantidadeAtual = estoque.Quantidade,
                Observacao = dto.Observacao?.Trim(),
                CriadaEm = DateTime.UtcNow
            });

        await _context.SaveChangesAsync();

        return estoque;
    }

    public async Task<Estoque> AjustarAsync(
        Guid empresaId,
        AjustarEstoqueDto dto)
    {
        if (dto.NovaQuantidade < 0)
            throw new InvalidOperationException(
                "A quantidade não pode ser negativa.");

        var estoque = await ObterEstoqueAsync(
            empresaId,
            dto.ProdutoId);

        if (estoque is null)
            throw new InvalidOperationException(
                "Estoque não encontrado.");

        var produto = await _context.Produtos
            .FirstOrDefaultAsync(x =>
                x.Id == dto.ProdutoId &&
                x.EmpresaId == empresaId);

        if (produto is null)
            throw new InvalidOperationException(
                "Produto não encontrado.");

        if (!produto.ControlaEstoque)
            throw new InvalidOperationException(
                "Este produto não controla estoque.");

        var anterior = estoque.Quantidade;

        estoque.Quantidade = dto.NovaQuantidade;
        estoque.AtualizadoEm = DateTime.UtcNow;

        _context.MovimentacoesEstoque.Add(
            new MovimentacaoEstoque
            {
                Id = Guid.NewGuid(),
                EmpresaId = empresaId,
                ProdutoId = dto.ProdutoId,
                Tipo = TipoMovimentacaoEstoque.Ajuste,
                Quantidade = Math.Abs(dto.NovaQuantidade - anterior),
                QuantidadeAnterior = anterior,
                QuantidadeAtual = dto.NovaQuantidade,
                Observacao = dto.Observacao?.Trim(),
                CriadaEm = DateTime.UtcNow
            });

        await _context.SaveChangesAsync();

        return estoque;
    }

    public async Task<Estoque?> ObterAsync(
        Guid empresaId,
        Guid produtoId)
    {
        return await _context.Estoques
            .Include(x => x.Produto)
            .FirstOrDefaultAsync(x =>
                x.EmpresaId == empresaId &&
                x.ProdutoId == produtoId);
    }

    public async Task<List<Estoque>> ListarAsync(
        Guid empresaId)
    {
        return await _context.Estoques
            .Include(x => x.Produto)
            .Where(x => x.EmpresaId == empresaId)
            .OrderBy(x => x.Produto.Nome)
            .ToListAsync();
    }

    public async Task<List<MovimentacaoEstoque>> HistoricoAsync(
        Guid empresaId,
        Guid produtoId)
    {
        return await _context.MovimentacoesEstoque
            .Where(x =>
                x.EmpresaId == empresaId &&
                x.ProdutoId == produtoId)
            .OrderByDescending(x => x.CriadaEm)
            .ToListAsync();
    }

    private async Task<Estoque?> ObterEstoqueAsync(
        Guid empresaId,
        Guid produtoId)
    {
        return await _context.Estoques
            .FirstOrDefaultAsync(x =>
                x.EmpresaId == empresaId &&
                x.ProdutoId == produtoId);
    }
}