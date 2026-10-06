using ControlHub.Api.Models;

namespace ControlHub.Api.DTOs.Financeiro;

public class MovimentacaoFinanceiraResponseDto
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public CategoriaMovimentacaoFinanceira Categoria { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public bool Entrada { get; set; }

    public DateTime CriadaEm { get; set; }
}