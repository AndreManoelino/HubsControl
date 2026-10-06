using ControlHub.Api.DTOs.Financeiro;
using ControlHub.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Dono")]
public class MovimentacoesFinanceirasController : ControllerBase
{
    private readonly MovimentacaoFinanceiraService _service;

    public MovimentacoesFinanceirasController(
        MovimentacaoFinanceiraService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(
        CriarMovimentacaoFinanceiraDto dto)
    {
        try
        {
            var empresaId = ObterEmpresaId();

            var movimentacao = await _service.CriarAsync(
                empresaId,
                dto);

            return Ok(movimentacao);
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

        var movimentacoes = await _service.ListarAsync(
            empresaId);

        return Ok(movimentacoes);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id)
    {
        var empresaId = ObterEmpresaId();

        var movimentacao = await _service.ObterPorIdAsync(
            empresaId,
            id);

        if (movimentacao is null)
            return NotFound(new
            {
                mensagem = "Movimentação financeira não encontrada."
            });

        return Ok(movimentacao);
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