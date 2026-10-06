using ControlHub.Api.DTOs.Estoque;
using ControlHub.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Dono")]
public class EstoqueController : ControllerBase
{
    private readonly EstoqueService _service;

    public EstoqueController(EstoqueService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var empresaId = ObterEmpresaId();

        var estoques = await _service.ListarAsync(empresaId);

        return Ok(estoques);
    }

    [HttpGet("{produtoId:guid}")]
    public async Task<IActionResult> Obter(Guid produtoId)
    {
        var empresaId = ObterEmpresaId();

        var estoque = await _service.ObterAsync(
            empresaId,
            produtoId);

        if (estoque is null)
            return NotFound(new
            {
                mensagem = "Estoque não encontrado."
            });

        return Ok(estoque);
    }

    [HttpPost("entrada")]
    public async Task<IActionResult> Entrada(
        MovimentarEstoqueDto dto)
    {
        try
        {
            var empresaId = ObterEmpresaId();

            var estoque = await _service.EntradaAsync(
                empresaId,
                dto);

            return Ok(estoque);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    [HttpPost("saida")]
    public async Task<IActionResult> Saida(
        MovimentarEstoqueDto dto)
    {
        try
        {
            var empresaId = ObterEmpresaId();

            var estoque = await _service.SaidaAsync(
                empresaId,
                dto);

            return Ok(estoque);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    [HttpPost("ajuste")]
    public async Task<IActionResult> Ajustar(
        AjustarEstoqueDto dto)
    {
        try
        {
            var empresaId = ObterEmpresaId();

            var estoque = await _service.AjustarAsync(
                empresaId,
                dto);

            return Ok(estoque);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    [HttpGet("{produtoId:guid}/historico")]
    public async Task<IActionResult> Historico(Guid produtoId)
    {
        var empresaId = ObterEmpresaId();

        var historico = await _service.HistoricoAsync(
            empresaId,
            produtoId);

        return Ok(historico);
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