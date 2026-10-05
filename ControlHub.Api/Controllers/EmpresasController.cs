
using ControlHub.Api.DTOs.Empresas;
using ControlHub.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Master")]
public class EmpresasController : ControllerBase
{
    private readonly EmpresaService _empresaService;

    public EmpresasController(EmpresaService empresaService)
    {
        _empresaService = empresaService;
    }

    // ==========================================
    // CRIAR EMPRESA
    // ==========================================

    [HttpPost]
    public async Task<IActionResult> Criar(CriarEmpresaDto dto)
    {
        try
        {
            var empresa = await _empresaService
                .CriarEmpresaAsync(dto);

            return CreatedAtAction(
                nameof(Criar),
                new { id = empresa.Id },
                new
                {
                    empresa.Id,
                    empresa.Nome,
                    empresa.NomeFantasia,
                    empresa.Cpf,
                    empresa.Url,
                    empresa.DonoId
                });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    // ==========================================
    // BUSCAR POR ID DA EMPRESA
    // ==========================================

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> BuscarPorId(Guid id)
    {
        var empresa = await _empresaService
            .BuscarPorIdAsync(id);

        if (empresa is null)
        {
            return NotFound(new
            {
                mensagem = "Empresa não encontrada."
            });
        }

        return Ok(new
        {
            empresa.Id,
            empresa.Cpf,
            empresa.Nome,
            empresa.NomeFantasia,
            empresa.LogoUrl,
            empresa.ImagemLoginUrl,
            empresa.Url,
            empresa.Ativa,
            empresa.CriadaEm,
            empresa.DonoId,
            Dono = empresa.Dono == null
                ? null
                : new
                {
                    empresa.Dono.Id,
                    empresa.Dono.Nome,
                    empresa.Dono.Email,
                    empresa.Dono.Perfil,
                    empresa.Dono.Ativo,
                    empresa.Dono.CriadoEm
                }
        });
    }

    // ==========================================
    // BUSCAR POR ID DO DONO
    // ==========================================

    [HttpGet("dono/{donoId:guid}")]
    public async Task<IActionResult> BuscarPorDonoId(Guid donoId)
    {
        var empresa = await _empresaService
            .BuscarPorDonoIdAsync(donoId);

        if (empresa is null)
        {
            return NotFound(new
            {
                mensagem = "Empresa não encontrada."
            });
        }

       return Ok(new
        {
            empresa.Id,
            empresa.Cpf,
            empresa.Nome,
            empresa.NomeFantasia,
            empresa.LogoUrl,
            empresa.ImagemLoginUrl,
            empresa.Url,
            empresa.Ativa,
            empresa.CriadaEm,
            empresa.DonoId,
            Dono = empresa.Dono == null
                ? null
                : new
                {
                    empresa.Dono.Id,
                    empresa.Dono.Nome,
                    empresa.Dono.Email,
                    empresa.Dono.Perfil,
                    empresa.Dono.Ativo,
                    empresa.Dono.CriadoEm
                }
        });
    }

    // ==========================================
    // BUSCAR POR NOME DO DONO
    // ==========================================

    [HttpGet("buscar/dono")]
    public async Task<IActionResult> BuscarPorNomeDono(
        [FromQuery] string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            return BadRequest(new
            {
                mensagem = "Informe o nome do dono."
            });
        }

        var empresas = await _empresaService
            .BuscarPorNomeDonoAsync(nome);

        return Ok(empresas.Select(empresa => new
        {
            empresa.Id,
            empresa.Cpf,
            empresa.Nome,
            empresa.NomeFantasia,
            empresa.LogoUrl,
            empresa.ImagemLoginUrl,
            empresa.Url,
            empresa.Ativa,
            empresa.CriadaEm,
            empresa.DonoId,
            Dono = empresa.Dono == null
                ? null
                : new
                {
                    empresa.Dono.Id,
                    empresa.Dono.Nome,
                    empresa.Dono.Email,
                    empresa.Dono.Perfil,
                    empresa.Dono.Ativo,
                    empresa.Dono.CriadoEm
                }
        }));
    }

    // ==========================================
    // BUSCAR POR NOME DA EMPRESA
    // ==========================================

    [HttpGet("buscar")]
    public async Task<IActionResult> BuscarPorNome(
        [FromQuery] string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            return BadRequest(new
            {
                mensagem = "Informe o nome da empresa."
            });
        }

        var empresas = await _empresaService
            .BuscarPorNomeAsync(nome);

        return Ok(empresas.Select(empresa => new
        {
            empresa.Id,
            empresa.Cpf,
            empresa.Nome,
            empresa.NomeFantasia,
            empresa.LogoUrl,
            empresa.ImagemLoginUrl,
            empresa.Url,
            empresa.Ativa,
            empresa.CriadaEm,
            empresa.DonoId,
            Dono = empresa.Dono == null
                ? null
                : new
                {
                    empresa.Dono.Id,
                    empresa.Dono.Nome,
                    empresa.Dono.Email,
                    empresa.Dono.Perfil,
                    empresa.Dono.Ativo,
                    empresa.Dono.CriadoEm
                }
        }));
    }

    // ==========================================
    // EDITAR EMPRESA
    // ==========================================

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Editar(
        Guid id,
        CriarEmpresaDto dto)
    {
        try
        {
            var empresa = await _empresaService
                .EditarEmpresaAsync(id, dto);

            if (empresa is null)
            {
                return NotFound(new
                {
                    mensagem = "Empresa não encontrada."
                });
            }

            return Ok(new
            {
                mensagem = "Empresa atualizada com sucesso.",
                empresa.Id,
                empresa.Nome,
                empresa.NomeFantasia,
                empresa.Cpf,
                empresa.Url,
                empresa.Ativa
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    // ==========================================
    // BLOQUEAR EMPRESA
    // ==========================================

    [HttpPut("{id:guid}/bloquear")]
    public async Task<IActionResult> Bloquear(Guid id)
    {
        var bloqueada = await _empresaService
            .BloquearEmpresaAsync(id);

        if (!bloqueada)
        {
            return NotFound(new
            {
                mensagem = "Empresa não encontrada."
            });
        }

        return Ok(new
        {
            mensagem = "Empresa bloqueada com sucesso."
        });
    }
    // ==========================================
// BUSCAR TODAS AS EMPRESAS
// ==========================================

    [HttpGet]
    public async Task<IActionResult> BuscarTodas()
    {
        var empresas = await _empresaService
            .BuscarTodasAsync();

        return Ok(empresas.Select(empresa => new
        {
            empresa.Id,
            empresa.Cpf,
            empresa.Nome,
            empresa.NomeFantasia,
            empresa.LogoUrl,
            empresa.ImagemLoginUrl,
            empresa.Url,
            empresa.Ativa,
            empresa.CriadaEm,
            empresa.DonoId,

            Dono = empresa.Dono == null
                ? null
                : new
                {
                    empresa.Dono.Id,
                    empresa.Dono.Nome,
                    empresa.Dono.Email,
                    empresa.Dono.Perfil,
                    empresa.Dono.Ativo,
                    empresa.Dono.CriadoEm
                }
        }));
    }
    [AllowAnonymous]
    [HttpGet("publica/{url}")]
    public async Task<IActionResult> BuscarEmpresaPublica(string url)
    {
        var empresa = await _empresaService.BuscarPublicaPorUrlAsync(url);

        if (empresa is null)
            return NotFound(new
            {
                mensagem = "Empresa não encontrada."
            });

        return Ok(new
        {
            id = empresa.Id,
            nome = empresa.Nome,
            nomeFantasia = empresa.NomeFantasia,
            logoUrl = empresa.LogoUrl,
            imagemLoginUrl = empresa.ImagemLoginUrl,
            url = empresa.Url
        });
    }
}
