namespace ControlHub.Api.Models;

public class ItemVenda
{
    public Guid Id { get; set; }

    public Guid VendaId { get; set; }

    public Guid ProdutoId { get; set; }

    public string NomeProduto { get; set; } = string.Empty;

    public decimal Quantidade { get; set; }

    public decimal PrecoUnitario { get; set; }

    public decimal ValorTotal { get; set; }

    public decimal CustoUnitario { get; set; }

    public decimal CustoTotal { get; set; }

    public decimal Lucro { get; set; }

    public Venda Venda { get; set; } = null!;

    public Produto Produto { get; set; } = null!;
}