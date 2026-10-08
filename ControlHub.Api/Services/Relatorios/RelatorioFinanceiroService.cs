using ClosedXML.Excel;
using ControlHub.Api.Data;
using ControlHub.Api.DTOs.Financeiro;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.Text;

namespace ControlHub.Api.Services.Relatorios;

public class RelatorioFinanceiroService
{
    private readonly ControlHubDbContext _context;
    private readonly FinanceiroDashboardService _financeiroService;

    public RelatorioFinanceiroService(
        ControlHubDbContext context,
        FinanceiroDashboardService financeiroService)
    {
        _context = context;
        _financeiroService = financeiroService;
    }

    // ============================================================
    // PDF
    // ============================================================

    public async Task<byte[]> GerarPdfAsync(
        Guid empresaId,
        DateTime? dataInicio,
        DateTime? dataFim)
    {
        var dashboard = await _financeiroService.ObterAsync(
            empresaId,
            dataInicio,
            dataFim);

        var empresa = await _context.Empresas
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == empresaId);

        var nomeEmpresa =
            empresa?.NomeFantasia
            ?? "Empresa";

        var cpfEmpresa =
            empresa?.Cpf
            ?? string.Empty;

        var pdf = Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);

                page.DefaultTextStyle(
                    x => x.FontSize(9));

                page.Header()
                    .Element(container =>
                        CriarCabecalho(
                            container,
                            nomeEmpresa,
                            cpfEmpresa,
                            dashboard));

                page.Content()
                    .PaddingVertical(15)
                    .Column(column =>
                    {
                        column.Spacing(12);

                        CriarResumo(
                            column,
                            dashboard);

                        CriarEstoque(
                            column,
                            dashboard);

                        CriarMovimentacoes(
                            column,
                            dashboard);

                        CriarVendas(
                            column,
                            dashboard);

                        CriarCompras(
                            column,
                            dashboard);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span(
                            $"Gerado em {DateTime.Now:dd/MM/yyyy HH:mm}");

                        text.Span("  •  ");

                        text.CurrentPageNumber();
                    });
            });
        });

        return pdf.GeneratePdf();
    }

    private void CriarCabecalho(
        IContainer container,
        string nomeEmpresa,
        string documento,
        FinanceiroDashboardDto dashboard)
    {
        container
            .BorderBottom(1)
            .PaddingBottom(10)
            .Column(column =>
            {
                column.Item()
                    .Text(nomeEmpresa)
                    .Bold()
                    .FontSize(18);

                if (!string.IsNullOrWhiteSpace(documento))
                {
                    column.Item()
                        .Text($"Documento: {documento}")
                        .FontSize(8);
                }

                column.Item()
                    .PaddingTop(5)
                    .Text("RELATÓRIO FINANCEIRO")
                    .Bold()
                    .FontSize(13);

                column.Item()
                    .Text(
                        $"Período: {dashboard.DataInicio:dd/MM/yyyy} até {dashboard.DataFim:dd/MM/yyyy}");
            });
    }

    private void CriarResumo(
        ColumnDescriptor column,
        FinanceiroDashboardDto dashboard)
    {
        column.Item()
            .Text("Resumo financeiro")
            .Bold()
            .FontSize(13);

        column.Item()
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                CriarCard(table, "Entradas", dashboard.TotalEntradas);
                CriarCard(table, "Saídas", dashboard.TotalSaidas);
                CriarCard(table, "Saldo", dashboard.Saldo);

                CriarCard(table, "Vendas", dashboard.TotalVendas);
                CriarCard(table, "Compras", dashboard.TotalCompras);
                CriarCard(table, "Lucro bruto", dashboard.LucroBruto);
            });
    }

    private void CriarCard(
        TableDescriptor table,
        string titulo,
        decimal valor)
    {
        table.Cell()
            .Border(1)
            .Padding(8)
            .Column(column =>
            {
                column.Item()
                    .Text(titulo)
                    .FontSize(8);

                column.Item()
                    .PaddingTop(4)
                    .Text(
                        valor.ToString(
                            "C",
                            new CultureInfo("pt-BR")))
                    .Bold()
                    .FontSize(12);
            });
    }

    private void CriarEstoque(
        ColumnDescriptor column,
        FinanceiroDashboardDto dashboard)
    {
        column.Item()
            .Text("Estoque")
            .Bold()
            .FontSize(13);

        column.Item()
            .Text(
                $"Produtos com estoque: {dashboard.ProdutosComEstoque}   •   " +
                $"Quantidade: {dashboard.QuantidadeTotalEstoque:N3}   •   " +
                $"Custo: {dashboard.CustoTotalEstoque:C}   •   " +
                $"Valor de venda: {dashboard.ValorVendaEstoque:C}   •   " +
                $"Lucro potencial: {dashboard.LucroPotencialEstoque:C}");

        if (dashboard.Estoque.Count == 0)
        {
            column.Item()
                .Text("Nenhum produto em estoque.");
            return;
        }

        column.Item()
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2.5f);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                HeaderCell(table, "Produto");
                HeaderCell(table, "Qtd.");
                HeaderCell(table, "Custo");
                HeaderCell(table, "Custo total");
                HeaderCell(table, "Venda total");
                HeaderCell(table, "Lucro");

                foreach (var item in dashboard.Estoque)
                {
                    BodyCell(table, item.NomeProduto);
                    BodyCell(table, item.Quantidade.ToString("N3"));
                    BodyCell(table, item.CustoUnitario.ToString("C"));
                    BodyCell(table, item.CustoTotal.ToString("C"));
                    BodyCell(table, item.ValorVendaTotal.ToString("C"));
                    BodyCell(table, item.LucroPotencial.ToString("C"));
                }
            });
    }

    private void CriarMovimentacoes(
        ColumnDescriptor column,
        FinanceiroDashboardDto dashboard)
    {
        column.Item()
            .Text("Movimentações")
            .Bold()
            .FontSize(13);

        if (dashboard.UltimasMovimentacoes.Count == 0)
        {
            column.Item()
                .Text("Nenhuma movimentação no período.");
            return;
        }

        column.Item()
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                HeaderCell(table, "Data");
                HeaderCell(table, "Categoria");
                HeaderCell(table, "Descrição");
                HeaderCell(table, "Tipo");
                HeaderCell(table, "Valor");

                foreach (var item in dashboard.UltimasMovimentacoes)
                {
                    BodyCell(
                        table,
                        item.CriadaEm.ToLocalTime()
                            .ToString("dd/MM/yyyy HH:mm"));

                    BodyCell(
                        table,
                        item.Categoria);

                    BodyCell(
                        table,
                        item.Descricao);

                    BodyCell(
                        table,
                        item.Entrada
                            ? "Entrada"
                            : "Saída");

                    BodyCell(
                        table,
                        item.Valor.ToString("C"));
                }
            });
    }

    private void CriarVendas(
        ColumnDescriptor column,
        FinanceiroDashboardDto dashboard)
    {
        column.Item()
            .Text("Vendas")
            .Bold()
            .FontSize(13);

        if (dashboard.Vendas.Count == 0)
        {
            column.Item()
                .Text("Nenhuma venda no período.");
            return;
        }

        column.Item()
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                HeaderCell(table, "Data");
                HeaderCell(table, "Venda");
                HeaderCell(table, "Custo");
                HeaderCell(table, "Lucro");

                foreach (var item in dashboard.Vendas)
                {
                    BodyCell(
                        table,
                        item.CriadaEm.ToLocalTime()
                            .ToString("dd/MM/yyyy HH:mm"));

                    BodyCell(
                        table,
                        item.ValorTotal.ToString("C"));

                    BodyCell(
                        table,
                        item.CustoTotal.ToString("C"));

                    BodyCell(
                        table,
                        item.Lucro.ToString("C"));
                }
            });
    }

    private void CriarCompras(
        ColumnDescriptor column,
        FinanceiroDashboardDto dashboard)
    {
        column.Item()
            .Text("Compras de fornecedores")
            .Bold()
            .FontSize(13);

        if (dashboard.Compras.Count == 0)
        {
            column.Item()
                .Text("Nenhuma compra no período.");
            return;
        }

        column.Item()
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1.5f);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                HeaderCell(table, "Data");
                HeaderCell(table, "Fornecedor");
                HeaderCell(table, "Nota");
                HeaderCell(table, "Valor");

                foreach (var item in dashboard.Compras)
                {
                    BodyCell(
                        table,
                        item.DataCompra.ToLocalTime()
                            .ToString("dd/MM/yyyy"));

                    BodyCell(
                        table,
                        item.NomeFornecedor);

                    BodyCell(
                        table,
                        item.NumeroNota ?? "-");

                    BodyCell(
                        table,
                        item.ValorTotal.ToString("C"));
                }
            });
    }

    private void HeaderCell(
        TableDescriptor table,
        string text)
    {
        table.Cell()
            .Background("#1F2937")
            .Padding(5)
            .Text(text)
            .FontColor("#FFFFFF")
            .Bold()
            .FontSize(8);
    }

    private void BodyCell(
        TableDescriptor table,
        string text)
    {
        table.Cell()
            .BorderBottom(0.5f)
            .Padding(4)
            .Text(text)
            .FontSize(7);
    }


    // ============================================================
    // EXCEL
    // ============================================================

    public async Task<byte[]> GerarExcelAsync(
        Guid empresaId,
        DateTime? dataInicio,
        DateTime? dataFim)
    {
        var dashboard = await _financeiroService.ObterAsync(
            empresaId,
            dataInicio,
            dataFim);

        var empresa = await _context.Empresas
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == empresaId);

        using var workbook = new XLWorkbook();

        CriarAbaResumo(
            workbook,
            dashboard,
            empresa?.NomeFantasia ?? "Empresa");

        CriarAbaMovimentacoes(
            workbook,
            dashboard);

        CriarAbaVendas(
            workbook,
            dashboard);

        CriarAbaCompras(
            workbook,
            dashboard);

        CriarAbaEstoque(
            workbook,
            dashboard);

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        return stream.ToArray();
    }

    private void CriarAbaResumo(
        XLWorkbook workbook,
        FinanceiroDashboardDto dashboard,
        string nomeEmpresa)
    {
        var sheet = workbook.Worksheets.Add("Resumo");

        sheet.Cell("A1")
            .Value = nomeEmpresa;

        sheet.Cell("A1")
            .Style.Font.Bold = true;

        sheet.Cell("A1")
            .Style.Font.FontSize = 18;

        sheet.Cell("A2")
            .Value = "RELATÓRIO FINANCEIRO";

        sheet.Cell("A3")
            .Value =
                $"Período: {dashboard.DataInicio:dd/MM/yyyy} até {dashboard.DataFim:dd/MM/yyyy}";

        sheet.Cell("A5").Value = "Indicador";
        sheet.Cell("B5").Value = "Valor";

        var resumo = new[]
        {
            ("Total Entradas", dashboard.TotalEntradas),
            ("Total Saídas", dashboard.TotalSaidas),
            ("Saldo", dashboard.Saldo),
            ("Total Vendas", dashboard.TotalVendas),
            ("Total Compras", dashboard.TotalCompras),
            ("Custo das Vendas", dashboard.CustoDasVendas),
            ("Lucro Bruto", dashboard.LucroBruto),
            ("Custo do Estoque", dashboard.CustoTotalEstoque),
            ("Valor de Venda do Estoque", dashboard.ValorVendaEstoque),
            ("Lucro Potencial do Estoque", dashboard.LucroPotencialEstoque)
        };

        var linha = 6;

        foreach (var item in resumo)
        {
            sheet.Cell(linha, 1).Value = item.Item1;
            sheet.Cell(linha, 2).Value = item.Item2;

            sheet.Cell(linha, 2)
                .Style.NumberFormat
                .Format = "R$ #,##0.00";

            linha++;
        }

        FormatarCabecalho(
            sheet.Range("A5:B5"));

        AjustarColunas(sheet);
    }

    private void CriarAbaMovimentacoes(
        XLWorkbook workbook,
        FinanceiroDashboardDto dashboard)
    {
        var sheet =
            workbook.Worksheets.Add("Movimentações");

        sheet.Cell("A1").Value = "Data";
        sheet.Cell("B1").Value = "Categoria";
        sheet.Cell("C1").Value = "Descrição";
        sheet.Cell("D1").Value = "Tipo";
        sheet.Cell("E1").Value = "Valor";

        FormatarCabecalho(
            sheet.Range("A1:E1"));

        var linha = 2;

        foreach (var item in dashboard.UltimasMovimentacoes)
        {
            sheet.Cell(linha, 1)
                .Value = item.CriadaEm.ToLocalTime();

            sheet.Cell(linha, 1)
                .Style.DateFormat.Format =
                "dd/MM/yyyy HH:mm";

            sheet.Cell(linha, 2)
                .Value = item.Categoria;

            sheet.Cell(linha, 3)
                .Value = item.Descricao;

            sheet.Cell(linha, 4)
                .Value =
                item.Entrada
                    ? "Entrada"
                    : "Saída";

            sheet.Cell(linha, 5)
                .Value = item.Valor;

            sheet.Cell(linha, 5)
                .Style.NumberFormat
                .Format = "R$ #,##0.00";

            linha++;
        }

        AjustarColunas(sheet);
    }

    private void CriarAbaVendas(
        XLWorkbook workbook,
        FinanceiroDashboardDto dashboard)
    {
        var sheet =
            workbook.Worksheets.Add("Vendas");

        sheet.Cell("A1").Value = "Data";
        sheet.Cell("B1").Value = "Venda";
        sheet.Cell("C1").Value = "Custo";
        sheet.Cell("D1").Value = "Lucro";

        FormatarCabecalho(
            sheet.Range("A1:D1"));

        var linha = 2;

        foreach (var item in dashboard.Vendas)
        {
            sheet.Cell(linha, 1)
                .Value = item.CriadaEm.ToLocalTime();

            sheet.Cell(linha, 1)
                .Style.DateFormat.Format =
                "dd/MM/yyyy HH:mm";

            sheet.Cell(linha, 2)
                .Value = item.ValorTotal;

            sheet.Cell(linha, 3)
                .Value = item.CustoTotal;

            sheet.Cell(linha, 4)
                .Value = item.Lucro;

            sheet.Range(linha, 2, linha, 4)
                .Style.NumberFormat
                .Format = "R$ #,##0.00";

            linha++;
        }

        AjustarColunas(sheet);
    }

    private void CriarAbaCompras(
        XLWorkbook workbook,
        FinanceiroDashboardDto dashboard)
    {
        var sheet =
            workbook.Worksheets.Add("Compras");

        sheet.Cell("A1").Value = "Data";
        sheet.Cell("B1").Value = "Fornecedor";
        sheet.Cell("C1").Value = "Nota";
        sheet.Cell("D1").Value = "Valor";

        FormatarCabecalho(
            sheet.Range("A1:D1"));

        var linha = 2;

        foreach (var item in dashboard.Compras)
        {
            sheet.Cell(linha, 1)
                .Value = item.DataCompra.ToLocalTime();

            sheet.Cell(linha, 1)
                .Style.DateFormat.Format =
                "dd/MM/yyyy";

            sheet.Cell(linha, 2)
                .Value = item.NomeFornecedor;

            sheet.Cell(linha, 3)
                .Value =
                item.NumeroNota ?? "-";

            sheet.Cell(linha, 4)
                .Value = item.ValorTotal;

            sheet.Cell(linha, 4)
                .Style.NumberFormat
                .Format = "R$ #,##0.00";

            linha++;
        }

        AjustarColunas(sheet);
    }

    private void CriarAbaEstoque(
        XLWorkbook workbook,
        FinanceiroDashboardDto dashboard)
    {
        var sheet =
            workbook.Worksheets.Add("Estoque");

        sheet.Cell("A1").Value = "Produto";
        sheet.Cell("B1").Value = "Quantidade";
        sheet.Cell("C1").Value = "Custo Unitário";
        sheet.Cell("D1").Value = "Custo Total";
        sheet.Cell("E1").Value = "Preço Venda";
        sheet.Cell("F1").Value = "Valor Venda";
        sheet.Cell("G1").Value = "Lucro Potencial";
        sheet.Cell("H1").Value = "Status";

        FormatarCabecalho(
            sheet.Range("A1:H1"));

        var linha = 2;

        foreach (var item in dashboard.Estoque)
        {
            sheet.Cell(linha, 1)
                .Value = item.NomeProduto;

            sheet.Cell(linha, 2)
                .Value = item.Quantidade;

            sheet.Cell(linha, 3)
                .Value = item.CustoUnitario;

            sheet.Cell(linha, 4)
                .Value = item.CustoTotal;

            sheet.Cell(linha, 5)
                .Value = item.PrecoVenda;

            sheet.Cell(linha, 6)
                .Value = item.ValorVendaTotal;

            sheet.Cell(linha, 7)
                .Value = item.LucroPotencial;

            sheet.Cell(linha, 8)
                .Value =
                item.Ativo
                    ? "Ativo"
                    : "Bloqueado";

            sheet.Range(linha, 3, linha, 7)
                .Style.NumberFormat
                .Format = "R$ #,##0.00";

            linha++;
        }

        AjustarColunas(sheet);
    }

    private void FormatarCabecalho(
        IXLRange range)
    {
        range.Style.Font.Bold = true;

        range.Style.Fill
            .BackgroundColor =
            XLColor.FromHtml("#1F2937");

        range.Style.Font.FontColor =
            XLColor.White;
    }

    private void AjustarColunas(
        IXLWorksheet sheet)
    {
        sheet.Columns()
            .AdjustToContents();

        sheet.SheetView.FreezeRows(1);
    }
}