namespace ControlHub.Api.Models;

public class MovimentacaoEstoque
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public Guid ProdutoId { get; set; }

    public TipoMovimentacaoEstoque Tipo { get; set; }

    public decimal Quantidade { get; set; }

    public decimal QuantidadeAnterior { get; set; }

    public decimal QuantidadeAtual { get; set; }

    public string? Observacao { get; set; }

    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;

    public Empresa Empresa { get; set; } = null!;

    public Produto Produto { get; set; } = null!;
}