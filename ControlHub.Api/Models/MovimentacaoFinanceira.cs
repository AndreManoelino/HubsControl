namespace ControlHub.Api.Models;

public class MovimentacaoFinanceira
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public CategoriaMovimentacaoFinanceira Categoria { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public bool Entrada { get; set; }

    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;

    public Empresa Empresa { get; set; } = null!;
}