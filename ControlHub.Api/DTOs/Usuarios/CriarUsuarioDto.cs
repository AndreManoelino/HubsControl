using ControlHub.Api.Models;

namespace ControlHub.Api.DTOs.Usuarios;

public class CriarUsuarioDto
{
    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Senha { get; set; } = string.Empty;

    public Perfil Perfil { get; set; }

    public Guid EmpresaId { get; set; }
}