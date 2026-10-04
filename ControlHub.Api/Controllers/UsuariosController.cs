using ControlHub.Api.DTOs.Usuarios;
using ControlHub.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Master")]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService _usuarioService;

    public UsuariosController(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(CriarUsuarioDto dto)
    {
        try
        {
            var usuario = await _usuarioService.CriarUsuarioAsync(dto);

            return CreatedAtAction(
                nameof(Criar),
                new { id = usuario.Id },
                new
                {
                    usuario.Id,
                    usuario.Nome,
                    usuario.Email,
                    usuario.Perfil,
                    usuario.EmpresaId,
                    usuario.Ativo
                });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }
}