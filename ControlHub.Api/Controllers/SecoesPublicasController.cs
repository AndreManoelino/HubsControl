using ControlHub.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlHub.Api.Controllers;

[ApiController]
[Route("api/publico/secoes")]
public class SecoesPublicasController : ControllerBase
{
    private readonly ControlHubDbContext _context;

    public SecoesPublicasController(
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

        var secoes = await _context.Secoes
            .Where(x =>
                x.EmpresaId == empresa.Id &&
                x.Ativa)
            .Include(x => x.Produtos)
                .ThenInclude(x => x.Estoque)
            .OrderBy(x => x.Nome)
            .Select(x => new
            {
                x.Id,
                x.Nome,
                Produtos = x.Produtos
                    .Where(p => p.Ativo)
                    .OrderBy(p => p.Nome)
                    .Select(p => new
                    {
                        p.Id,
                        p.Nome,
                        p.Descricao,
                        p.PrecoVenda,
                        p.ImagemUrl,
                        p.ImagemArquivo,
                        Disponivel =
                            !p.ControlaEstoque ||
                            (p.Estoque != null &&
                             p.Estoque.Quantidade > 0)
                    })
                    .ToList()
            })
            .ToListAsync();

        return Ok(secoes);
    }
}