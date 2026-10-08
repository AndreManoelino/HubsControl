using ControlHub.Api.Data;
using ControlHub.Api.DTOs.Financeiro;
using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace ControlHub.Api.Services;

public class FinanceiroDashboardService
{
    private readonly ControlHubDbContext _context;

    public FinanceiroDashboardService(
        ControlHubDbContext context)
    {
        _context = context;
    }

    // ============================================================
    // DASHBOARD
    // ============================================================

    public async Task<FinanceiroDashboardDto> ObterAsync(
        Guid empresaId,
        DateTime? dataInicio,
        DateTime? dataFim)
    {
        var (inicio, fimExclusivo, fim) =
            PrepararPeriodo(dataInicio, dataFim);

        // --------------------------------------------------------
        // MOVIMENTAÇÕES FINANCEIRAS
        // --------------------------------------------------------

        var movimentacoes = await _context
            .MovimentacoesFinanceiras
            .AsNoTracking()
            .Where(x =>
                x.EmpresaId == empresaId &&
                x.CriadaEm >= inicio &&
                x.CriadaEm < fimExclusivo)
            .OrderByDescending(x => x.CriadaEm)
            .ToListAsync();

        var totalEntradas = movimentacoes
            .Where(x => x.Entrada)
            .Sum(x => x.Valor);

        var totalSaidas = movimentacoes
            .Where(x => !x.Entrada)
            .Sum(x => x.Valor);

        // --------------------------------------------------------
        // VENDAS
        // --------------------------------------------------------

        var vendas = await _context.Vendas
            .AsNoTracking()
            .Include(x => x.Itens)
            .Where(x =>
                x.EmpresaId == empresaId &&
                x.CriadaEm >= inicio &&
                x.CriadaEm < fimExclusivo)
            .OrderByDescending(x => x.CriadaEm)
            .ToListAsync();

        var totalVendas = vendas.Sum(x => x.ValorTotal);

        var custoDasVendas = vendas
            .SelectMany(x => x.Itens)
            .Sum(x => x.CustoTotal);

        var lucroBruto =
            totalVendas - custoDasVendas;

        // --------------------------------------------------------
        // COMPRAS
        // --------------------------------------------------------

        var compras = await _context.ComprasFornecedor
            .AsNoTracking()
            .Include(x => x.Fornecedor)
            .Where(x =>
                x.EmpresaId == empresaId &&
                x.DataCompra >= inicio &&
                x.DataCompra < fimExclusivo)
            .OrderByDescending(x => x.DataCompra)
            .ToListAsync();

        var totalCompras = compras.Sum(x => x.ValorTotal);

        // --------------------------------------------------------
        // ESTOQUE ATUAL
        // --------------------------------------------------------

        var estoques = await _context.Estoques
            .AsNoTracking()
            .Include(x => x.Produto)
            .Where(x =>
                x.EmpresaId == empresaId &&
                x.Quantidade > 0)
            .OrderBy(x => x.Produto.Nome)
            .ToListAsync();

        var produtosIds = estoques
            .Select(x => x.ProdutoId)
            .Distinct()
            .ToList();

        // --------------------------------------------------------
        // ÚLTIMO CUSTO DE COMPRA POR PRODUTO
        //
        // Futuramente poderemos trocar isso por CUSTO MÉDIO.
        // O Financeiro já está preparado para isso.
        // --------------------------------------------------------

        var ultimosCustos = await _context
            .ItensCompraFornecedor
            .AsNoTracking()
            .Include(x => x.CompraFornecedor)
            .Where(x =>
                produtosIds.Contains(x.ProdutoId) &&
                x.CompraFornecedor.EmpresaId == empresaId)
            .GroupBy(x => x.ProdutoId)
            .Select(g => new
            {
                ProdutoId = g.Key,

                CustoUnitario = g
                    .OrderByDescending(x =>
                        x.CompraFornecedor.DataCompra)
                    .Select(x => x.CustoUnitario)
                    .FirstOrDefault()
            })
            .ToDictionaryAsync(
                x => x.ProdutoId,
                x => x.CustoUnitario);

        // --------------------------------------------------------
        // MONTA ESTOQUE FINANCEIRO
        // --------------------------------------------------------

        var estoqueDto =
            new List<FinanceiroProdutoEstoqueDto>();

        foreach (var estoque in estoques)
        {
            var produto = estoque.Produto;

            var custoUnitario =
                ultimosCustos.TryGetValue(
                    produto.Id,
                    out var custo)
                    ? custo
                    : 0m;

            var custoTotal =
                estoque.Quantidade * custoUnitario;

            var valorVendaTotal =
                estoque.Quantidade * produto.PrecoVenda;

            var lucroPotencial =
                valorVendaTotal - custoTotal;

            estoqueDto.Add(
                new FinanceiroProdutoEstoqueDto
                {
                    ProdutoId = produto.Id,
                    NomeProduto = produto.Nome,
                    Quantidade = estoque.Quantidade,
                    PrecoVenda = produto.PrecoVenda,
                    CustoUnitario = custoUnitario,
                    CustoTotal = custoTotal,
                    ValorVendaTotal = valorVendaTotal,
                    LucroPotencial = lucroPotencial,
                    Ativo = produto.Ativo
                });
        }

        // --------------------------------------------------------
        // TOTAIS DO ESTOQUE
        // --------------------------------------------------------

        var custoTotalEstoque =
            estoqueDto.Sum(x => x.CustoTotal);

        var valorVendaEstoque =
            estoqueDto.Sum(x => x.ValorVendaTotal);

        var lucroPotencialEstoque =
            estoqueDto.Sum(x => x.LucroPotencial);

        var quantidadeTotalEstoque =
            estoqueDto.Sum(x => x.Quantidade);

        // --------------------------------------------------------
        // RESPOSTA
        // --------------------------------------------------------

        return new FinanceiroDashboardDto
        {
            DataInicio = inicio,
            DataFim = fim,

            TotalEntradas = totalEntradas,
            TotalSaidas = totalSaidas,
            Saldo = totalEntradas - totalSaidas,

            TotalVendas = totalVendas,
            QuantidadeVendas = vendas.Count,

            CustoDasVendas = custoDasVendas,
            LucroBruto = lucroBruto,

            TotalCompras = totalCompras,
            QuantidadeCompras = compras.Count,

            CustoTotalEstoque = custoTotalEstoque,
            ValorVendaEstoque = valorVendaEstoque,
            LucroPotencialEstoque = lucroPotencialEstoque,
            QuantidadeTotalEstoque = quantidadeTotalEstoque,
            ProdutosComEstoque = estoqueDto.Count,

            UltimasMovimentacoes = movimentacoes
                .Take(20)
                .Select(x =>
                    new FinanceiroMovimentacaoResumoDto
                    {
                        Id = x.Id,
                        Categoria =
                            x.Categoria.ToString(),

                        Descricao =
                            x.Descricao,

                        Valor =
                            x.Valor,

                        Entrada =
                            x.Entrada,

                        CriadaEm =
                            x.CriadaEm
                    })
                .ToList(),

            Estoque = estoqueDto,

            Vendas = vendas
                .Take(100)
                .Select(x =>
                    new FinanceiroVendaResumoDto
                    {
                        Id = x.Id,
                        ValorTotal = x.ValorTotal,

                        CustoTotal =
                            x.Itens.Sum(i =>
                                i.CustoTotal),

                        Lucro =
                            x.Itens.Sum(i =>
                                i.Lucro),

                        CriadaEm =
                            x.CriadaEm
                    })
                .ToList(),

            Compras = compras
                .Take(100)
                .Select(x =>
                    new FinanceiroCompraResumoDto
                    {
                        Id = x.Id,
                        FornecedorId =
                            x.FornecedorId,

                        NomeFornecedor =
                            x.Fornecedor.Nome,

                        NumeroNota =
                            x.NumeroNota,

                        ValorTotal =
                            x.ValorTotal,

                        DataCompra =
                            x.DataCompra
                    })
                .ToList()
        };
    }


    // ============================================================
    // MOVIMENTAÇÕES
    // ============================================================

    public async Task<List<FinanceiroMovimentacaoResumoDto>>
        ListarMovimentacoesAsync(
            Guid empresaId,
            DateTime? dataInicio,
            DateTime? dataFim)
    {
        var (inicio, fimExclusivo, _) =
            PrepararPeriodo(dataInicio, dataFim);

        return await _context
            .MovimentacoesFinanceiras
            .AsNoTracking()
            .Where(x =>
                x.EmpresaId == empresaId &&
                x.CriadaEm >= inicio &&
                x.CriadaEm < fimExclusivo)
            .OrderByDescending(x => x.CriadaEm)
            .Select(x =>
                new FinanceiroMovimentacaoResumoDto
                {
                    Id = x.Id,
                    Categoria =
                        x.Categoria.ToString(),

                    Descricao =
                        x.Descricao,

                    Valor =
                        x.Valor,

                    Entrada =
                        x.Entrada,

                    CriadaEm =
                        x.CriadaEm
                })
            .ToListAsync();
    }


    // ============================================================
    // RELATÓRIO CSV
    // ============================================================

    public async Task<byte[]> GerarCsvAsync(
        Guid empresaId,
        DateTime? dataInicio,
        DateTime? dataFim)
    {
        var dashboard = await ObterAsync(
            empresaId,
            dataInicio,
            dataFim);

        var csv = new StringBuilder();

        csv.AppendLine(
            "RELATORIO FINANCEIRO");

        csv.AppendLine(
            $"Periodo;{dashboard.DataInicio:dd/MM/yyyy};{dashboard.DataFim:dd/MM/yyyy}");

        csv.AppendLine();

        csv.AppendLine(
            "RESUMO");

        csv.AppendLine(
            "Total Entradas;Total Saidas;Saldo;Vendas;Compras;Custo das Vendas;Lucro Bruto;Custo Estoque;Venda Estoque;Lucro Potencial");

        csv.AppendLine(
            $"{dashboard.TotalEntradas.ToString("F2", CultureInfo.InvariantCulture)};" +
            $"{dashboard.TotalSaidas.ToString("F2", CultureInfo.InvariantCulture)};" +
            $"{dashboard.Saldo.ToString("F2", CultureInfo.InvariantCulture)};" +
            $"{dashboard.TotalVendas.ToString("F2", CultureInfo.InvariantCulture)};" +
            $"{dashboard.TotalCompras.ToString("F2", CultureInfo.InvariantCulture)};" +
            $"{dashboard.CustoDasVendas.ToString("F2", CultureInfo.InvariantCulture)};" +
            $"{dashboard.LucroBruto.ToString("F2", CultureInfo.InvariantCulture)};" +
            $"{dashboard.CustoTotalEstoque.ToString("F2", CultureInfo.InvariantCulture)};" +
            $"{dashboard.ValorVendaEstoque.ToString("F2", CultureInfo.InvariantCulture)};" +
            $"{dashboard.LucroPotencialEstoque.ToString("F2", CultureInfo.InvariantCulture)}");

        csv.AppendLine();

        csv.AppendLine(
            "MOVIMENTACOES");

        csv.AppendLine(
            "Data;Categoria;Descricao;Tipo;Valor");

        foreach (var item in dashboard.UltimasMovimentacoes)
        {
            csv.AppendLine(
                $"{item.CriadaEm:dd/MM/yyyy HH:mm};" +
                $"{item.Categoria};" +
                $"\"{item.Descricao.Replace("\"", "\"\"")}\";" +
                $"{(item.Entrada ? "Entrada" : "Saida")};" +
                $"{item.Valor.ToString("F2", CultureInfo.InvariantCulture)}");
        }

        csv.AppendLine();

        csv.AppendLine(
            "ESTOQUE");

        csv.AppendLine(
            "Produto;Quantidade;Custo Unitario;Custo Total;Preco Venda;Valor Venda;Lucro Potencial;Ativo");

        foreach (var item in dashboard.Estoque)
        {
            csv.AppendLine(
                $"\"{item.NomeProduto.Replace("\"", "\"\"")}\";" +
                $"{item.Quantidade.ToString("F3", CultureInfo.InvariantCulture)};" +
                $"{item.CustoUnitario.ToString("F2", CultureInfo.InvariantCulture)};" +
                $"{item.CustoTotal.ToString("F2", CultureInfo.InvariantCulture)};" +
                $"{item.PrecoVenda.ToString("F2", CultureInfo.InvariantCulture)};" +
                $"{item.ValorVendaTotal.ToString("F2", CultureInfo.InvariantCulture)};" +
                $"{item.LucroPotencial.ToString("F2", CultureInfo.InvariantCulture)};" +
                $"{(item.Ativo ? "Ativo" : "Bloqueado")}");
        }

        return Encoding.UTF8.GetPreamble()
            .Concat(Encoding.UTF8.GetBytes(csv.ToString()))
            .ToArray();
    }


    // ============================================================
    // PREPARAÇÃO DO PERÍODO
    // ============================================================

    private static (
        DateTime Inicio,
        DateTime FimExclusivo,
        DateTime Fim)
        PrepararPeriodo(
            DateTime? dataInicio,
            DateTime? dataFim)
    {
        var inicio = dataInicio.HasValue
            ? DateTime.SpecifyKind(
                dataInicio.Value.Date,
                DateTimeKind.Utc)
            : DateTime.UtcNow.Date;

        var fim = dataFim.HasValue
            ? DateTime.SpecifyKind(
                dataFim.Value.Date,
                DateTimeKind.Utc)
            : inicio;

        if (fim < inicio)
        {
            throw new InvalidOperationException(
                "A data final não pode ser menor que a data inicial.");
        }

        return (
            inicio,
            fim.AddDays(1),
            fim);
    }
}