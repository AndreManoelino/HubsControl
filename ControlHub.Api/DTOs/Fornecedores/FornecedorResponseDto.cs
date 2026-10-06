namespace ControlHub.Api.DTOs.Fornecedores;

public class FornecedorResponseDto
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? Documento { get; set; }

    public string? Email { get; set; }

    public string? Telefone { get; set; }

    public bool Ativo { get; set; }

    public DateTime CriadoEm { get; set; }
}