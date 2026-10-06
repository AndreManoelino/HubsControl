namespace ControlHub.Api.DTOs.Vendas;

public class CriarVendaDto
{
    public List<ItemVendaDto> Itens { get; set; } = new();
}