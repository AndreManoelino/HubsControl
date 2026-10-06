using ControlHub.Api.Data;
using ControlHub.Api.DTOs.Vendas;
using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlHub.Api.Services;

public class VendaService
{
    private readonly ControlHubDbContext _context;

    public VendaService(ControlHubDbContext context)
    {
        _context = context;
    }

    public async Task<Venda> CriarAsync(
        Guid empresaId,
        CriarVendaDto dto)
    {
        if (dto.Itens is null || dto.Itens.Count == 0)
            throw new InvalidOperationException(
                "A venda deve possuir pelo menos um item.");

        var produtoIds = dto.Itens
            .Select(x => x.ProdutoId)
            .Distinct()
            .ToList();

        var produtos = await _context.Produtos
            .Include(x => x.Estoque)
            .Where(x =>
                x.EmpresaId == empresaId &&
                produtoIds.Contains(x.Id))
            .ToListAsync();

        if (produtos.Count != produtoIds.Count)
            throw new InvalidOperationException(
                "Um ou mais produtos não foram encontrados.");

        foreach (var item in dto.Itens)
        {
            if (item.Quantidade <= 0)
                throw new InvalidOperationException(
                    "A quantidade deve ser maior que zero.");

            var produto = produtos.First(x =>
                x.Id == item.ProdutoId);

            if (!produto.Ativo)
                throw new InvalidOperationException(
                    $"O produto '{produto.Nome}' está bloqueado.");

            if (produto.ControlaEstoque)
            {
                if (produto.Estoque is null)
                    throw new InvalidOperationException(
                        $"O produto '{produto.Nome}' não possui estoque.");

                if (produto.Estoque.Quantidade < item.Quantidade)
                    throw new InvalidOperationException(
                        $"Estoque insuficiente para '{produto.Nome}'.");
            }
        }

        var venda = new Venda
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            ValorTotal = 0,
            CriadaEm = DateTime.UtcNow
        };

        decimal valorTotal = 0;

        foreach (var itemDto in dto.Itens)
        {
            var produto = produtos.First(x =>
                x.Id == itemDto.ProdutoId);

            var valorItem =
                itemDto.Quantidade * produto.PrecoVenda;

            var custoUnitario = 0m;

            var ultimoCusto = await _context.ItensCompraFornecedor
                .Where(x =>
                    x.ProdutoId == produto.Id &&
                    x.CompraFornecedor.EmpresaId == empresaId)
                .OrderByDescending(x =>
                    x.CompraFornecedor.DataCompra)
                .Select(x => (decimal?)x.CustoUnitario)
                .FirstOrDefaultAsync();

            if (ultimoCusto.HasValue)
                custoUnitario = ultimoCusto.Value;

            var custoTotal =
                itemDto.Quantidade * custoUnitario;

            var lucro =
                valorItem - custoTotal;

            venda.Itens.Add(
                new ItemVenda
                {
                    Id = Guid.NewGuid(),
                    ProdutoId = produto.Id,
                    NomeProduto = produto.Nome,
                    Quantidade = itemDto.Quantidade,
                    PrecoUnitario = produto.PrecoVenda,
                    ValorTotal = valorItem,
                    CustoUnitario = custoUnitario,
                    CustoTotal = custoTotal,
                    Lucro = lucro
                });

            valorTotal += valorItem;

            if (produto.ControlaEstoque &&
                produto.Estoque is not null)
            {
                var quantidadeAnterior =
                    produto.Estoque.Quantidade;

                produto.Estoque.Quantidade -=
                    itemDto.Quantidade;

                produto.Estoque.AtualizadoEm =
                    DateTime.UtcNow;

                _context.MovimentacoesEstoque.Add(
                    new MovimentacaoEstoque
                    {
                        Id = Guid.NewGuid(),
                        EmpresaId = empresaId,
                        ProdutoId = produto.Id,
                        Tipo = TipoMovimentacaoEstoque.Saida,
                        Quantidade = itemDto.Quantidade,
                        QuantidadeAnterior = quantidadeAnterior,
                        QuantidadeAtual = produto.Estoque.Quantidade,
                        Observacao = "Saída por venda.",
                        CriadaEm = DateTime.UtcNow
                    });
            }
        }

        venda.ValorTotal = valorTotal;

        _context.Vendas.Add(venda);

        _context.MovimentacoesFinanceiras.Add(
            new MovimentacaoFinanceira
            {
                Id = Guid.NewGuid(),
                EmpresaId = empresaId,
                Categoria = CategoriaMovimentacaoFinanceira.Venda,
                Descricao = $"Venda {venda.Id}",
                Valor = venda.ValorTotal,
                Entrada = true,
                CriadaEm = DateTime.UtcNow
            });

        await _context.SaveChangesAsync();

        return venda;
    }

    public async Task<List<Venda>> ListarAsync(
        Guid empresaId)
    {
        return await _context.Vendas
            .Include(x => x.Itens)
            .Where(x => x.EmpresaId == empresaId)
            .OrderByDescending(x => x.CriadaEm)
            .ToListAsync();
    }

    public async Task<Venda?> ObterPorIdAsync(
        Guid empresaId,
        Guid id)
    {
        return await _context.Vendas
            .Include(x => x.Itens)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.EmpresaId == empresaId);
    }
}