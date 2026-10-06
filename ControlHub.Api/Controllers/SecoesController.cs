using ControlHub.Api.DTOs.Secoes;
using ControlHub.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ControlHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Dono")]
public class SecoesController : ControllerBase
{
    private readonly SecaoService _service;

    public SecoesController(SecaoService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(CriarSecaoDto dto)
    {
        var empresaId = ObterEmpresaId();

        var secao = await _service.CriarAsync(empresaId, dto);

        return Ok(secao);
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var empresaId = ObterEmpresaId();

        var secoes = await _service.ListarAsync(empresaId);

        return Ok(secoes);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id)
    {
        var empresaId = ObterEmpresaId();

        var secao = await _service.ObterPorIdAsync(
            empresaId,
            id);

        if (secao is null)
            return NotFound(new
            {
                mensagem = "Seção não encontrada."
            });

        return Ok(secao);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(
        Guid id,
        AtualizarSecaoDto dto)
    {
        try
        {
            var empresaId = ObterEmpresaId();

            var secao = await _service.AtualizarAsync(
                empresaId,
                id,
                dto);

            return Ok(secao);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    [HttpPut("{id:guid}/bloquear")]
    public async Task<IActionResult> Bloquear(Guid id)
    {
        try
        {
            var empresaId = ObterEmpresaId();

            await _service.BloquearAsync(
                empresaId,
                id);

            return Ok(new
            {
                mensagem = "Seção bloqueada com sucesso."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    private Guid ObterEmpresaId()
    {
        var claim = User.FindFirst("EmpresaId")?.Value;

        if (!Guid.TryParse(claim, out var empresaId))
            throw new UnauthorizedAccessException(
                "Empresa não identificada.");

        return empresaId;
    }

    [HttpPut("{id:guid}/ativar")]
    public async Task<IActionResult> Ativar(Guid id)
    {
        try
        {
            var empresaId = ObterEmpresaId();

            await _service.AtivarAsync(
                empresaId,
                id);

            return Ok(new
            {
                mensagem = "Seção ativada com sucesso."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }
}