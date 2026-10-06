using ControlHub.Api.DTOs.Vendas;
using ControlHub.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Dono")]
public class VendasController : ControllerBase
{
    private readonly VendaService _service;

    public VendasController(VendaService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(
        CriarVendaDto dto)
    {
        try
        {
            var empresaId = ObterEmpresaId();

            var venda = await _service.CriarAsync(
                empresaId,
                dto);

            return Ok(venda);
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

        var vendas = await _service.ListarAsync(
            empresaId);

        return Ok(vendas);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id)
    {
        var empresaId = ObterEmpresaId();

        var venda = await _service.ObterPorIdAsync(
            empresaId,
            id);

        if (venda is null)
            return NotFound(new
            {
                mensagem = "Venda não encontrada."
            });

        return Ok(venda);
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