using ControlHub.Api.Data;
using ControlHub.Api.DTOs.Compras;
using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlHub.Api.Services;

public class CompraFornecedorService
{
    private readonly ControlHubDbContext _context;

    public CompraFornecedorService(ControlHubDbContext context)
    {
        _context = context;
    }

    public async Task<CompraFornecedor> CriarAsync(
        Guid empresaId,
        CriarCompraFornecedorDto dto)
    {
        if (dto.FornecedorId == Guid.Empty)
            throw new InvalidOperationException(
                "Fornecedor é obrigatório.");

        if (dto.Itens is null || dto.Itens.Count == 0)
            throw new InvalidOperationException(
                "A compra deve possuir pelo menos um item.");

        var fornecedor = await _context.Fornecedores
            .FirstOrDefaultAsync(x =>
                x.Id == dto.FornecedorId &&
                x.EmpresaId == empresaId &&
                x.Ativo);

        if (fornecedor is null)
            throw new InvalidOperationException(
                "Fornecedor não encontrado ou inativo.");

        var produtoIds = dto.Itens
            .Select(x => x.ProdutoId)
            .Distinct()
            .ToList();

        var produtos = await _context.Produtos
            .Where(x =>
                x.EmpresaId == empresaId &&
                produtoIds.Contains(x.Id))
            .ToListAsync();

        if (produtos.Count != produtoIds.Count)
            throw new InvalidOperationException(
                "Um ou mais produtos não pertencem à empresa.");

        foreach (var item in dto.Itens)
        {
            if (item.Quantidade <= 0)
                throw new InvalidOperationException(
                    "A quantidade dos itens deve ser maior que zero.");

            if (item.CustoUnitario < 0)
                throw new InvalidOperationException(
                    "O custo unitário não pode ser negativo.");
        }

        var compra = new CompraFornecedor
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            FornecedorId = dto.FornecedorId,
            NumeroNota = dto.NumeroNota?.Trim(),
            DataCompra = dto.DataCompra ?? DateTime.UtcNow,
            ValorTotal = 0
        };

        decimal valorTotal = 0;

        foreach (var itemDto in dto.Itens)
        {
            var produto = produtos.First(x =>
                x.Id == itemDto.ProdutoId);

            var custoTotal =
                itemDto.Quantidade * itemDto.CustoUnitario;

            valorTotal += custoTotal;

            compra.Itens.Add(
                new ItemCompraFornecedor
                {
                    Id = Guid.NewGuid(),
                    ProdutoId = produto.Id,
                    Quantidade = itemDto.Quantidade,
                    CustoUnitario = itemDto.CustoUnitario,
                    CustoTotal = custoTotal
                });
        }

        compra.ValorTotal = valorTotal;

        _context.ComprasFornecedor.Add(compra);

        foreach (var item in compra.Itens)
        {
            var estoque = await _context.Estoques
                .FirstOrDefaultAsync(x =>
                    x.EmpresaId == empresaId &&
                    x.ProdutoId == item.ProdutoId);

            var produto = produtos.First(x =>
                x.Id == item.ProdutoId);

            if (produto.ControlaEstoque)
            {
                if (estoque is null)
                {
                    estoque = new Estoque
                    {
                        Id = Guid.NewGuid(),
                        EmpresaId = empresaId,
                        ProdutoId = item.ProdutoId,
                        Quantidade = 0,
                        QuantidadeMinima = 0,
                        AtualizadoEm = DateTime.UtcNow
                    };

                    _context.Estoques.Add(estoque);
                }

                var quantidadeAnterior = estoque.Quantidade;

                estoque.Quantidade += item.Quantidade;
                estoque.AtualizadoEm = DateTime.UtcNow;

                _context.MovimentacoesEstoque.Add(
                    new MovimentacaoEstoque
                    {
                        Id = Guid.NewGuid(),
                        EmpresaId = empresaId,
                        ProdutoId = item.ProdutoId,
                        Tipo = TipoMovimentacaoEstoque.Entrada,
                        Quantidade = item.Quantidade,
                        QuantidadeAnterior = quantidadeAnterior,
                        QuantidadeAtual = estoque.Quantidade,
                        Observacao = "Entrada por compra de fornecedor.",
                        CriadaEm = DateTime.UtcNow
                    });
            }
        }

        _context.MovimentacoesFinanceiras.Add(
            new MovimentacaoFinanceira
            {
                Id = Guid.NewGuid(),
                EmpresaId = empresaId,
                Categoria = CategoriaMovimentacaoFinanceira.CompraFornecedor,
                Descricao = $"Compra do fornecedor {fornecedor.Nome}",
                Valor = compra.ValorTotal,
                Entrada = false,
                CriadaEm = DateTime.UtcNow
            });

        await _context.SaveChangesAsync();

        return compra;
    }

    public async Task<List<CompraFornecedor>> ListarAsync(
        Guid empresaId)
    {
        return await _context.ComprasFornecedor
            .Include(x => x.Fornecedor)
            .Include(x => x.Itens)
            .ThenInclude(x => x.Produto)
            .Where(x => x.EmpresaId == empresaId)
            .OrderByDescending(x => x.DataCompra)
            .ToListAsync();
    }

    public async Task<CompraFornecedor?> ObterPorIdAsync(
        Guid empresaId,
        Guid id)
    {
        return await _context.ComprasFornecedor
            .Include(x => x.Fornecedor)
            .Include(x => x.Itens)
            .ThenInclude(x => x.Produto)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.EmpresaId == empresaId);
    }
}