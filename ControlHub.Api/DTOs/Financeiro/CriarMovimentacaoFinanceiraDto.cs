using ControlHub.Api.Models;

namespace ControlHub.Api.DTOs.Financeiro;

public class CriarMovimentacaoFinanceiraDto
{
    public string Descricao { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public bool Entrada { get; set; }

    public CategoriaMovimentacaoFinanceira Categoria { get; set; }
}