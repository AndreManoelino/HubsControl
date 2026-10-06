namespace ControlHub.Api.DTOs.Produtos;

public class ProdutoPublicoResponseDto
{
    public Guid Id { get; set; }
    public Guid SecaoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal PrecoVenda { get; set; }
    public string? ImagemUrl { get; set; }
    public string? ImagemArquivo { get; set; }
    public bool Disponivel { get; set; }
}