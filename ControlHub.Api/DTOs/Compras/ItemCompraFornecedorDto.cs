namespace ControlHub.Api.DTOs.Compras;

public class ItemCompraFornecedorDto
{
    public Guid ProdutoId { get; set; }

    public decimal Quantidade { get; set; }

    public decimal CustoUnitario { get; set; }
}