namespace ControlHub.Api.DTOs.Compras;

public class CriarCompraFornecedorDto
{
    public Guid FornecedorId { get; set; }

    public string? NumeroNota { get; set; }

    public DateTime? DataCompra { get; set; }

    public List<ItemCompraFornecedorDto> Itens { get; set; } = new();
}