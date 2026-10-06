using ControlHub.Api.DTOs.Fornecedores;
using ControlHub.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Dono")]
public class FornecedoresController : ControllerBase
{
    private readonly FornecedorService _service;

    public FornecedoresController(FornecedorService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(
        CriarFornecedorDto dto)
    {
        try
        {
            var empresaId = ObterEmpresaId();

            var fornecedor = await _service.CriarAsync(
                empresaId,
                dto);

            return Ok(fornecedor);
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

        var fornecedores = await _service.ListarAsync(
            empresaId);

        return Ok(fornecedores);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id)
    {
        var empresaId = ObterEmpresaId();

        var fornecedor = await _service.ObterPorIdAsync(
            empresaId,
            id);

        if (fornecedor is null)
            return NotFound(new
            {
                mensagem = "Fornecedor não encontrado."
            });

        return Ok(fornecedor);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(
        Guid id,
        AtualizarFornecedorDto dto)
    {
        try
        {
            var empresaId = ObterEmpresaId();

            var fornecedor = await _service.AtualizarAsync(
                empresaId,
                id,
                dto);

            return Ok(fornecedor);
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
                mensagem = "Fornecedor bloqueado com sucesso."
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
}