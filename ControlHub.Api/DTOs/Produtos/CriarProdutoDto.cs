namespace ControlHub.Api.DTOs.Produtos;

public class CriarProdutoDto
{
    public Guid SecaoId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public decimal PrecoVenda { get; set; }

    public string? ImagemUrl { get; set; }

    public bool ControlaEstoque { get; set; }

    public decimal? QuantidadeInicial { get; set; }

    public decimal? QuantidadeMinima { get; set; }

    public decimal? CustoUnitario { get; set; }
}