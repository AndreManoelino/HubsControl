namespace ControlHub.Api.Models;

public class Produto
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public Guid SecaoId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public decimal PrecoVenda { get; set; }

    public string? ImagemUrl { get; set; }

    public string? ImagemArquivo { get; set; }

    public bool ControlaEstoque { get; set; } = false;

    public bool Ativo { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public Empresa Empresa { get; set; } = null!;

    public Secao Secao { get; set; } = null!;

    public Estoque? Estoque { get; set; }
}