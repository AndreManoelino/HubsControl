namespace ControlHub.Api.DTOs.Estoque;

public class EstoqueResponseDto
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public Guid ProdutoId { get; set; }

    public string NomeProduto { get; set; } = string.Empty;

    public decimal Quantidade { get; set; }

    public decimal QuantidadeMinima { get; set; }

    public DateTime AtualizadoEm { get; set; }
}