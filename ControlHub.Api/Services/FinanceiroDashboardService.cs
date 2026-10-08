
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

        // ========================================================
        // MOVIMENTAÇÕES FINANCEIRAS
        // ========================================================

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

        // ========================================================
        // VENDAS
        // ========================================================

        var vendas = await _context
            .Vendas
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

        // ========================================================
        // COMPRAS
        // ========================================================

        var compras = await _context
            .ComprasFornecedor
            .AsNoTracking()
            .Include(x => x.Fornecedor)
            .Where(x =>
                x.EmpresaId == empresaId &&
                x.DataCompra >= inicio &&
                x.DataCompra < fimExclusivo)
            .OrderByDescending(x => x.DataCompra)
            .ToListAsync();

        var totalCompras = compras.Sum(x => x.ValorTotal);

        // ========================================================
        // ESTOQUE ATUAL
        // ========================================================

        var estoques = await _context
            .Estoques
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

        // ========================================================
        // ÚLTIMO CUSTO DE COMPRA POR PRODUTO
        //
        // Atualmente usamos o último custo de compra.
        //
        // Futuramente podemos substituir por:
        // - custo médio ponderado;
        // - custo médio móvel;
        // - FIFO;
        // - outro método contábil.
        // ========================================================

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

        // ========================================================
        // MONTA ESTOQUE FINANCEIRO
        // ========================================================

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

                    NomeProduto =
                        produto.Nome,

                    Quantidade =
                        estoque.Quantidade,

                    PrecoVenda =
                        produto.PrecoVenda,

                    CustoUnitario =
                        custoUnitario,

                    CustoTotal =
                        custoTotal,

                    ValorVendaTotal =
                        valorVendaTotal,

                    LucroPotencial =
                        lucroPotencial,

                    Ativo =
                        produto.Ativo
                });
        }

        // ========================================================
        // TOTAIS DO ESTOQUE
        // ========================================================

        var custoTotalEstoque =
            estoqueDto.Sum(x => x.CustoTotal);

        var valorVendaEstoque =
            estoqueDto.Sum(x => x.ValorVendaTotal);

        var lucroPotencialEstoque =
            estoqueDto.Sum(x => x.LucroPotencial);

        var quantidadeTotalEstoque =
            estoqueDto.Sum(x => x.Quantidade);

        // ========================================================
        // RESPOSTA
        // ========================================================

        return new FinanceiroDashboardDto
        {
            DataInicio = inicio,

            DataFim = fim,

            // ----------------------------------------------------
            // FINANCEIRO
            // ----------------------------------------------------

            TotalEntradas =
                totalEntradas,

            TotalSaidas =
                totalSaidas,

            Saldo =
                totalEntradas - totalSaidas,

            // ----------------------------------------------------
            // VENDAS
            // ----------------------------------------------------

            TotalVendas =
                totalVendas,

            QuantidadeVendas =
                vendas.Count,

            CustoDasVendas =
                custoDasVendas,

            LucroBruto =
                lucroBruto,

            // ----------------------------------------------------
            // COMPRAS
            // ----------------------------------------------------

            TotalCompras =
                totalCompras,

            QuantidadeCompras =
                compras.Count,

            // ----------------------------------------------------
            // ESTOQUE
            // ----------------------------------------------------

            CustoTotalEstoque =
                custoTotalEstoque,

            ValorVendaEstoque =
                valorVendaEstoque,

            LucroPotencialEstoque =
                lucroPotencialEstoque,

            QuantidadeTotalEstoque =
                quantidadeTotalEstoque,

            ProdutosComEstoque =
                estoqueDto.Count,

            // ----------------------------------------------------
            // ÚLTIMAS MOVIMENTAÇÕES
            // ----------------------------------------------------

            UltimasMovimentacoes =
                movimentacoes
                    .Take(20)
                    .Select(x =>
                        new FinanceiroMovimentacaoResumoDto
                        {
                            Id =
                                x.Id,

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

            // ----------------------------------------------------
            // ESTOQUE
            // ----------------------------------------------------

            Estoque =
                estoqueDto,

            // ----------------------------------------------------
            // VENDAS
            // ----------------------------------------------------

            Vendas =
                vendas
                    .Take(100)
                    .Select(x =>
                        new FinanceiroVendaResumoDto
                        {
                            Id =
                                x.Id,

                            ValorTotal =
                                x.ValorTotal,

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

            // ----------------------------------------------------
            // COMPRAS
            // ----------------------------------------------------

            Compras =
                compras
                    .Take(100)
                    .Select(x =>
                        new FinanceiroCompraResumoDto
                        {
                            Id =
                                x.Id,

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
            PrepararPeriodo(
                dataInicio,
                dataFim);

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
                    Id =
                        x.Id,

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
        var dashboard =
            await ObterAsync(
                empresaId,
                dataInicio,
                dataFim);

        var csv =
            new StringBuilder();

        // ========================================================
        // CONFIGURAÇÃO DO CSV
        // ========================================================

        // Ajuda o Excel brasileiro a interpretar corretamente
        // o arquivo usando ponto e vírgula como separador.

        csv.AppendLine("sep=;");

        csv.AppendLine();

        // ========================================================
        // CABEÇALHO
        // ========================================================

        csv.AppendLine(
            "RELATORIO FINANCEIRO");

        csv.AppendLine(
            $"Periodo;{FormatarData(dashboard.DataInicio)};{FormatarData(dashboard.DataFim)}");

        csv.AppendLine(
            $"Gerado em;{DateTime.Now:dd/MM/yyyy HH:mm:ss}");

        csv.AppendLine();

        // ========================================================
        // RESUMO FINANCEIRO
        // ========================================================

        csv.AppendLine(
            "RESUMO FINANCEIRO");

        csv.AppendLine(
            "Indicador;Valor");

        csv.AppendLine(
            $"Total de Entradas;{FormatarMoeda(dashboard.TotalEntradas)}");

        csv.AppendLine(
            $"Total de Saidas;{FormatarMoeda(dashboard.TotalSaidas)}");

        csv.AppendLine(
            $"Saldo;{FormatarMoeda(dashboard.Saldo)}");

        csv.AppendLine(
            $"Total de Vendas;{FormatarMoeda(dashboard.TotalVendas)}");

        csv.AppendLine(
            $"Quantidade de Vendas;{dashboard.QuantidadeVendas}");

        csv.AppendLine(
            $"Custo das Vendas;{FormatarMoeda(dashboard.CustoDasVendas)}");

        csv.AppendLine(
            $"Lucro Bruto;{FormatarMoeda(dashboard.LucroBruto)}");

        csv.AppendLine(
            $"Total de Compras;{FormatarMoeda(dashboard.TotalCompras)}");

        csv.AppendLine(
            $"Quantidade de Compras;{dashboard.QuantidadeCompras}");

        csv.AppendLine(
            $"Custo Total do Estoque;{FormatarMoeda(dashboard.CustoTotalEstoque)}");

        csv.AppendLine(
            $"Valor de Venda do Estoque;{FormatarMoeda(dashboard.ValorVendaEstoque)}");

        csv.AppendLine(
            $"Lucro Potencial do Estoque;{FormatarMoeda(dashboard.LucroPotencialEstoque)}");

        csv.AppendLine(
            $"Quantidade Total em Estoque;{FormatarQuantidade(dashboard.QuantidadeTotalEstoque)}");

        csv.AppendLine(
            $"Produtos com Estoque;{dashboard.ProdutosComEstoque}");

        csv.AppendLine();

        // ========================================================
        // MOVIMENTAÇÕES FINANCEIRAS
        // ========================================================

        csv.AppendLine(
            "MOVIMENTACOES FINANCEIRAS");

        csv.AppendLine(
            "Data;Categoria;Descricao;Tipo;Valor");

        foreach (var item in dashboard.UltimasMovimentacoes)
        {
            csv.AppendLine(
                string.Join(
                    ";",
                    EscaparCsv(
                        FormatarDataHora(item.CriadaEm)),

                    EscaparCsv(
                        TraduzirCategoria(item.Categoria)),

                    EscaparCsv(
                        item.Descricao),

                    EscaparCsv(
                        item.Entrada
                            ? "Entrada"
                            : "Saida"),

                    EscaparCsv(
                        FormatarMoeda(item.Valor))
                ));
        }

        csv.AppendLine();

        // ========================================================
        // VENDAS
        // ========================================================

        csv.AppendLine(
            "VENDAS");

        csv.AppendLine(
            "Data;Venda ID;Valor Total;Custo Total;Lucro");

        foreach (var venda in dashboard.Vendas)
        {
            csv.AppendLine(
                string.Join(
                    ";",
                    EscaparCsv(
                        FormatarDataHora(venda.CriadaEm)),

                    EscaparCsv(
                        venda.Id.ToString()),

                    EscaparCsv(
                        FormatarMoeda(venda.ValorTotal)),

                    EscaparCsv(
                        FormatarMoeda(venda.CustoTotal)),

                    EscaparCsv(
                        FormatarMoeda(venda.Lucro))
                ));
        }

        csv.AppendLine();

        // ========================================================
        // COMPRAS
        // ========================================================

        csv.AppendLine(
            "COMPRAS DE FORNECEDORES");

        csv.AppendLine(
            "Data;Compra ID;Fornecedor;Numero Nota;Valor Total");

        foreach (var compra in dashboard.Compras)
        {
            csv.AppendLine(
                string.Join(
                    ";",
                    EscaparCsv(
                        FormatarDataHora(compra.DataCompra)),

                    EscaparCsv(
                        compra.Id.ToString()),

                    EscaparCsv(
                        compra.NomeFornecedor),

                    EscaparCsv(
                        compra.NumeroNota),

                    EscaparCsv(
                        FormatarMoeda(compra.ValorTotal))
                ));
        }

        csv.AppendLine();

        // ========================================================
        // ESTOQUE
        // ========================================================

        csv.AppendLine(
            "ESTOQUE ATUAL");

        csv.AppendLine(
            "Produto;Quantidade;Custo Unitario;Custo Total;Preco Venda;Valor Venda;Lucro Potencial;Status");

        foreach (var item in dashboard.Estoque)
        {
            csv.AppendLine(
                string.Join(
                    ";",
                    EscaparCsv(
                        item.NomeProduto),

                    EscaparCsv(
                        FormatarQuantidade(item.Quantidade)),

                    EscaparCsv(
                        FormatarMoeda(item.CustoUnitario)),

                    EscaparCsv(
                        FormatarMoeda(item.CustoTotal)),

                    EscaparCsv(
                        FormatarMoeda(item.PrecoVenda)),

                    EscaparCsv(
                        FormatarMoeda(item.ValorVendaTotal)),

                    EscaparCsv(
                        FormatarMoeda(item.LucroPotencial)),

                    EscaparCsv(
                        item.Ativo
                            ? "Ativo"
                            : "Bloqueado")
                ));
        }

        csv.AppendLine();

        // ========================================================
        // RODAPÉ
        // ========================================================

        csv.AppendLine(
            "FIM DO RELATORIO");

        // ========================================================
        // UTF-8 COM BOM
        //
        // O BOM faz o Excel reconhecer corretamente:
        // - acentos;
        // - ç;
        // - ã;
        // - ê;
        // - caracteres especiais.
        // ========================================================

        var texto =
            csv.ToString();

        var bytes =
            Encoding.UTF8.GetBytes(texto);

        return Encoding.UTF8
            .GetPreamble()
            .Concat(bytes)
            .ToArray();
    }

    // ============================================================
    // FORMATAÇÃO CSV
    // ============================================================

    private static string EscaparCsv(
        string? valor)
    {
        if (string.IsNullOrEmpty(valor))
            return "\"\"";

        var resultado =
            valor
                .Replace("\"", "\"\"")
                .Replace("\r", " ")
                .Replace("\n", " ");

        return $"\"{resultado}\"";
    }

    // ============================================================
    // FORMATAÇÃO DE MOEDA
    // ============================================================

    private static string FormatarMoeda(
        decimal valor)
    {
        return valor.ToString(
            "#,##0.00",
            new CultureInfo("pt-BR"));
    }

    // ============================================================
    // FORMATAÇÃO DE QUANTIDADE
    // ============================================================

    private static string FormatarQuantidade(
        decimal valor)
    {
        return valor.ToString(
            "#,##0.###",
            new CultureInfo("pt-BR"));
    }

    // ============================================================
    // FORMATAÇÃO DE DATA
    // ============================================================

    private static string FormatarData(
        DateTime data)
    {
        return data.ToString(
            "dd/MM/yyyy",
            new CultureInfo("pt-BR"));
    }

    // ============================================================
    // FORMATAÇÃO DE DATA E HORA
    // ============================================================

    private static string FormatarDataHora(
        DateTime data)
    {
        return data.ToString(
            "dd/MM/yyyy HH:mm:ss",
            new CultureInfo("pt-BR"));
    }

    // ============================================================
    // TRADUZ CATEGORIA FINANCEIRA
    // ============================================================

    private static string TraduzirCategoria(
        string categoria)
    {
        return categoria switch
        {
            nameof(CategoriaMovimentacaoFinanceira.Venda)
                => "Venda",

            nameof(CategoriaMovimentacaoFinanceira.CompraFornecedor)
                => "Compra de Fornecedor",

            nameof(CategoriaMovimentacaoFinanceira.Despesa)
                => "Despesa",

            nameof(CategoriaMovimentacaoFinanceira.Ajuste)
                => "Ajuste",

            _ => categoria
        };
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
        var inicio =
            dataInicio.HasValue
                ? DateTime.SpecifyKind(
                    dataInicio.Value.Date,
                    DateTimeKind.Utc)
                : DateTime.UtcNow.Date;

        var fim =
            dataFim.HasValue
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
