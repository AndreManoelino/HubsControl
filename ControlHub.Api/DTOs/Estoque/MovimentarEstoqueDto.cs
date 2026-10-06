namespace ControlHub.Api.DTOs.Estoque;

public class MovimentarEstoqueDto
{
    public Guid ProdutoId { get; set; }

    public decimal Quantidade { get; set; }

    public string? Observacao { get; set; }
}