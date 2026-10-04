namespace ControlHub.Api.DTOs.Empresas;

public class CriarEmpresaDto
{
    public string Cpf { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string? NomeFantasia { get; set; }

    public string? LogoUrl { get; set; }

    public string? ImagemLoginUrl { get; set; }

    public string Url { get; set; } = string.Empty;

    public string NomeDono { get; set; } = string.Empty;

    public string EmailDono { get; set; } = string.Empty;

    public string SenhaDono { get; set; } = string.Empty;
}