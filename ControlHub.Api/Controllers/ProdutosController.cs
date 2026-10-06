using ControlHub.Api.DTOs.Produtos;
using ControlHub.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Dono")]
public class ProdutosController : ControllerBase
{
    private readonly ProdutoService _service;

    public ProdutosController(ProdutoService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(CriarProdutoDto dto)
    {
        try
        {
            var empresaId = ObterEmpresaId();

            var produto = await _service.CriarAsync(
                empresaId,
                dto);

            return Ok(new
            {
                produto.Id,
                produto.EmpresaId,
                produto.SecaoId,
                produto.Nome,
                produto.Descricao,
                produto.PrecoVenda,
                produto.ImagemUrl,
                produto.ImagemArquivo,
                produto.ControlaEstoque,
                produto.Ativo,
                produto.CriadoEm
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

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var empresaId = ObterEmpresaId();

        var produtos = await _service.ListarAsync(empresaId);

        return Ok(produtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id)
    {
        var empresaId = ObterEmpresaId();

        var produto = await _service.ObterPorIdAsync(
            empresaId,
            id);

        if (produto is null)
            return NotFound(new
            {
                mensagem = "Produto não encontrado."
            });

        return Ok(produto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(
        Guid id,
        AtualizarProdutoDto dto)
    {
        try
        {
            var empresaId = ObterEmpresaId();

            var produto = await _service.AtualizarAsync(
                empresaId,
                id,
                dto);

            return Ok(new
            {
                produto.Id,
                produto.EmpresaId,
                produto.SecaoId,
                produto.Nome,
                produto.Descricao,
                produto.PrecoVenda,
                produto.ImagemUrl,
                produto.ImagemArquivo,
                produto.ControlaEstoque,
                produto.Ativo,
                produto.CriadoEm,
                QuantidadeEstoque = produto.ControlaEstoque
                    ? produto.Estoque?.Quantidade
                    : null,
                QuantidadeMinima = produto.ControlaEstoque
                    ? produto.Estoque?.QuantidadeMinima
                    : null
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

    [HttpPut("{id:guid}/bloquear")]
    public async Task<IActionResult> Bloquear(
        Guid id,
        BloquearProdutoDto dto)
    {
        try
        {
            var empresaId = ObterEmpresaId();

            await _service.BloquearAsync(
                empresaId,
                id,
                dto);

            return Ok(new
            {
                mensagem = "Produto bloqueado com sucesso."
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