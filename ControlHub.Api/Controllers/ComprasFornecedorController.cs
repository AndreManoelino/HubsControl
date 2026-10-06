using ControlHub.Api.DTOs.Compras;
using ControlHub.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Dono")]
public class ComprasFornecedorController : ControllerBase
{
    private readonly CompraFornecedorService _service;

    public ComprasFornecedorController(
        CompraFornecedorService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(
        CriarCompraFornecedorDto dto)
    {
        try
        {
            var empresaId = ObterEmpresaId();

            var compra = await _service.CriarAsync(
                empresaId,
                dto);

            return Ok(compra);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var empresaId = ObterEmpresaId();

        var compras = await _service.ListarAsync(
            empresaId);

        return Ok(compras);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id)
    {
        var empresaId = ObterEmpresaId();

        var compra = await _service.ObterPorIdAsync(
            empresaId,
            id);

        if (compra is null)
            return NotFound(new
            {
                mensagem = "Compra não encontrada."
            });

        return Ok(compra);
    }

    private Guid ObterEmpresaId()
    {
        var claim = User.FindFirst("EmpresaId")?.Value;

        if (!Guid.TryParse(claim, out var empresaId))
            throw new UnauthorizedAccessException(
                "Empresa não identificada.");

        return empresaId;
    }
}