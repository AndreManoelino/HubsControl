using ControlHub.Api.Models;

namespace ControlHub.Api.DTOs.Financeiro;

public class FinanceiroDashboardDto
{
    public DateTime DataInicio { get; set; }

    public DateTime DataFim { get; set; }

    // =========================
    // FINANCEIRO
    // =========================

    public decimal TotalEntradas { get; set; }

    public decimal TotalSaidas { get; set; }

    public decimal Saldo { get; set; }

    // =========================
    // VENDAS
    // =========================

    public decimal TotalVendas { get; set; }

    public int QuantidadeVendas { get; set; }

    public decimal CustoDasVendas { get; set; }

    public decimal LucroBruto { get; set; }

    // =========================
    // COMPRAS
    // =========================

    public decimal TotalCompras { get; set; }

    public int QuantidadeCompras { get; set; }

    // =========================
    // ESTOQUE
    // =========================

    public decimal CustoTotalEstoque { get; set; }

    public decimal ValorVendaEstoque { get; set; }

    public decimal LucroPotencialEstoque { get; set; }

    public decimal QuantidadeTotalEstoque { get; set; }

    public int ProdutosComEstoque { get; set; }

    // =========================
    // RESUMOS
    // =========================

    public List<FinanceiroMovimentacaoResumoDto>
        UltimasMovimentacoes { get; set; } = new();

    public List<FinanceiroProdutoEstoqueDto>
        Estoque { get; set; } = new();

    public List<FinanceiroVendaResumoDto>
        Vendas { get; set; } = new();

    public List<FinanceiroCompraResumoDto>
        Compras { get; set; } = new();
}


public class FinanceiroMovimentacaoResumoDto
{
    public Guid Id { get; set; }

    public string Categoria { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public bool Entrada { get; set; }

    public DateTime CriadaEm { get; set; }
}


public class FinanceiroProdutoEstoqueDto
{
    public Guid ProdutoId { get; set; }

    public string NomeProduto { get; set; } = string.Empty;

    public decimal Quantidade { get; set; }

    public decimal PrecoVenda { get; set; }

    public decimal CustoUnitario { get; set; }

    public decimal CustoTotal { get; set; }

    public decimal ValorVendaTotal { get; set; }

    public decimal LucroPotencial { get; set; }

    public bool Ativo { get; set; }
}


public class FinanceiroVendaResumoDto
{
    public Guid Id { get; set; }

    public decimal ValorTotal { get; set; }

    public decimal CustoTotal { get; set; }

    public decimal Lucro { get; set; }

    public DateTime CriadaEm { get; set; }
}


public class FinanceiroCompraResumoDto
{
    public Guid Id { get; set; }

    public Guid FornecedorId { get; set; }

    public string NomeFornecedor { get; set; } = string.Empty;

    public string? NumeroNota { get; set; }

    public decimal ValorTotal { get; set; }

    public DateTime DataCompra { get; set; }
}