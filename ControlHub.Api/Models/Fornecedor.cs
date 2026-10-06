namespace ControlHub.Api.Models;

public class Fornecedor
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? Documento { get; set; }

    public string? Email { get; set; }

    public string? Telefone { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public Empresa Empresa { get; set; } = null!;
}