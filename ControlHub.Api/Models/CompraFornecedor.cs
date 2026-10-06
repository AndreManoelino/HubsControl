namespace ControlHub.Api.Models;

public class CompraFornecedor
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public Guid FornecedorId { get; set; }

    public decimal ValorTotal { get; set; }

    public string? NumeroNota { get; set; }

    public DateTime DataCompra { get; set; } = DateTime.UtcNow;

    public Empresa Empresa { get; set; } = null!;

    public Fornecedor Fornecedor { get; set; } = null!;

    public ICollection<ItemCompraFornecedor> Itens { get; set; } =
        new List<ItemCompraFornecedor>();
}