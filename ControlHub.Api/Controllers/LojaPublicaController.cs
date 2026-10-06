using ControlHub.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlHub.Api.Controllers;

[ApiController]
[Route("api/publico/loja")]
public class LojaPublicaController : ControllerBase
{
    private readonly ControlHubDbContext _context;

    public LojaPublicaController(ControlHubDbContext context)
    {
        _context = context;
    }

    [HttpGet("{empresaUrl}")]
    public async Task<IActionResult> Obter(string empresaUrl)
    {
        var empresa = await _context.Empresas
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Url == empresaUrl &&
                x.Ativa);

        if (empresa is null)
            return NotFound(new
            {
                mensagem = "Loja não encontrada."
            });

        return Ok(new
        {
            empresa.Id,
            empresa.Nome,
            empresa.NomeFantasia,
            empresa.LogoUrl,
            empresa.ImagemLoginUrl,
            empresa.Url
        });
    }
}