using ControlHub.Api.Data;
using ControlHub.Api.DTOs.Produtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlHub.Api.Controllers;

[ApiController]
[Route("api/publico/produtos")]
public class ProdutosPublicosController : ControllerBase
{
    private readonly ControlHubDbContext _context;

    public ProdutosPublicosController(
        ControlHubDbContext context)
    {
        _context = context;
    }

    [HttpGet("{empresaUrl}")]
    public async Task<IActionResult> Listar(
        string empresaUrl)
    {
        var empresa = await _context.Empresas
            .FirstOrDefaultAsync(x =>
                x.Url == empresaUrl &&
                x.Ativa);

        if (empresa is null)
            return NotFound(new
            {
                mensagem = "Empresa não encontrada."
            });

        var produtos = await _context.Produtos
            .Include(x => x.Estoque)
            .Where(x =>
                x.EmpresaId == empresa.Id &&
                x.Ativo &&
                x.Secao.Ativa)
            .OrderBy(x => x.Secao.Nome)
            .ThenBy(x => x.Nome)
            .Select(x => new ProdutoPublicoResponseDto
            {
                Id = x.Id,
                SecaoId = x.SecaoId,
                Nome = x.Nome,
                Descricao = x.Descricao,
                PrecoVenda = x.PrecoVenda,
                ImagemUrl = x.ImagemUrl,
                ImagemArquivo = x.ImagemArquivo,
                Disponivel = !x.ControlaEstoque ||
                             (x.Estoque != null &&
                              x.Estoque.Quantidade > 0)
            })
            .ToListAsync();

        return Ok(produtos);
    }
}