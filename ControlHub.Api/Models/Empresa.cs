namespace ControlHub.Api.Models;

public class Empresa
{
    public Guid Id { get; set; }

    // CPF usado como identificador principal da empresa/dono
    public string Cpf { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string? NomeFantasia { get; set; }

    // Logo da empresa
    public string? LogoUrl { get; set; }

    // Imagem utilizada na tela de login da empresa
    public string? ImagemLoginUrl { get; set; }

    // URL pública da empresa
    public string Url { get; set; } = string.Empty;

    public bool Ativa { get; set; } = true;

    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;

    // Dono da empresa
    public Guid? DonoId { get; set; }

    public Usuario? Dono { get; set; } = null!;

    // Usuários pertencentes a esta empresa
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}