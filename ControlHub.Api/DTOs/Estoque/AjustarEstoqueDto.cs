namespace ControlHub.Api.DTOs.Estoque;

public class AjustarEstoqueDto
{
    public Guid ProdutoId { get; set; }

    public decimal NovaQuantidade { get; set; }

    public string? Observacao { get; set; }
}