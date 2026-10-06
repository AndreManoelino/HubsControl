namespace ControlHub.Api.DTOs.Vendas;

public class ItemVendaResponseDto
{
    public Guid ProdutoId { get; set; }

    public string NomeProduto { get; set; } = string.Empty;

    public decimal Quantidade { get; set; }

    public decimal PrecoUnitario { get; set; }

    public decimal ValorTotal { get; set; }

    public decimal CustoUnitario { get; set; }

    public decimal CustoTotal { get; set; }

    public decimal Lucro { get; set; }
}