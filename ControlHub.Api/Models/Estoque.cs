namespace ControlHub.Api.Models;

public class Estoque
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public Guid ProdutoId { get; set; }

    public decimal Quantidade { get; set; }

    public decimal QuantidadeMinima { get; set; }

    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;

    public Empresa Empresa { get; set; } = null!;

    public Produto Produto { get; set; } = null!;
}