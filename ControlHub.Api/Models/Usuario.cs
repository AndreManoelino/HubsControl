namespace ControlHub.Api.Models;

public class Usuario
{
    public Guid Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string SenhaHash { get; set; } = string.Empty;

    public Perfil Perfil { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // Empresa à qual o usuário pertence
    public Guid? EmpresaId { get; set; }

    public Empresa? Empresa { get; set; } = null!;
}