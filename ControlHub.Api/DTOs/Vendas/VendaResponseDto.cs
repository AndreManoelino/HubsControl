namespace ControlHub.Api.DTOs.Vendas;

public class VendaResponseDto
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public decimal ValorTotal { get; set; }

    public DateTime CriadaEm { get; set; }

    public List<ItemVendaResponseDto> Itens { get; set; } = new();
}