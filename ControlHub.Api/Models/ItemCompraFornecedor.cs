namespace ControlHub.Api.Models;

public class ItemCompraFornecedor
{
    public Guid Id { get; set; }

    public Guid CompraFornecedorId { get; set; }

    public Guid ProdutoId { get; set; }

    public decimal Quantidade { get; set; }

    public decimal CustoUnitario { get; set; }

    public decimal CustoTotal { get; set; }

    public CompraFornecedor CompraFornecedor { get; set; } = null!;

    public Produto Produto { get; set; } = null!;
}