namespace ControlHub.Api.Models;

public class Venda
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public decimal ValorTotal { get; set; }

    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;

    public Empresa Empresa { get; set; } = null!;

    public ICollection<ItemVenda> Itens { get; set; } = new List<ItemVenda>();
}