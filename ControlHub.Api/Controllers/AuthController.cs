using ControlHub.Api.DTOs.Auth;
using ControlHub.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ControlHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest(new
            {
                mensagem = "E-mail é obrigatório."
            });

        if (string.IsNullOrWhiteSpace(dto.Senha))
            return BadRequest(new
            {
                mensagem = "Senha é obrigatória."
            });

        var resultado = await _authService.LoginAsync(dto);

        if (resultado is null)
        {
            return Unauthorized(new
            {
                mensagem = "E-mail ou senha inválidos."
            });
        }

        return Ok(resultado);
    }
}