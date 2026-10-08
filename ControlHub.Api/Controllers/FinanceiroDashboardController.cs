using ControlHub.Api.DTOs.Financeiro;
using ControlHub.Api.Services;
using ControlHub.Api.Services.Relatorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Dono,Master")]
public class FinanceiroDashboardController : ControllerBase
{
    private readonly FinanceiroDashboardService _service;
    private readonly RelatorioFinanceiroService _relatorioService;

    public FinanceiroDashboardController(
        FinanceiroDashboardService service,
        RelatorioFinanceiroService relatorioService)
    {
        _service = service;
        _relatorioService = relatorioService;
    }


    // ============================================================
    // DASHBOARD
    // ============================================================

    [HttpGet]
    public async Task<ActionResult<FinanceiroDashboardDto>> Obter(
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim)
    {
        try
        {
            var empresaId = ObterEmpresaId();

            var resultado =
                await _service.ObterAsync(
                    empresaId,
                    dataInicio,
                    dataFim);

            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }


    // ============================================================
    // MOVIMENTAÇÕES
    // ============================================================

    [HttpGet("movimentacoes")]
    public async Task<
        ActionResult<List<FinanceiroMovimentacaoResumoDto>>>
        Movimentacoes(
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
    {
        try
        {
            var empresaId = ObterEmpresaId();

            var resultado =
                await _service.ListarMovimentacoesAsync(
                    empresaId,
                    dataInicio,
                    dataFim);

            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }


    // ============================================================
    // PDF
    // ============================================================

    [HttpGet("exportar/pdf")]
    public async Task<IActionResult> ExportarPdf(
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim)
    {
        try
        {
            var empresaId = ObterEmpresaId();

            var arquivo =
                await _relatorioService.GerarPdfAsync(
                    empresaId,
                    dataInicio,
                    dataFim);

            var nomeArquivo =
                $"relatorio-financeiro-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";

            return File(
                arquivo,
                "application/pdf",
                nomeArquivo);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }


    // ============================================================
    // EXCEL
    // ============================================================

    [HttpGet("exportar/excel")]
    public async Task<IActionResult> ExportarExcel(
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim)
    {
        try
        {
            var empresaId = ObterEmpresaId();

            var arquivo =
                await _relatorioService.GerarExcelAsync(
                    empresaId,
                    dataInicio,
                    dataFim);

            var nomeArquivo =
                $"relatorio-financeiro-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";

            return File(
                arquivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                nomeArquivo);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }


    // ============================================================
    // CSV
    // ============================================================

    [HttpGet("exportar/csv")]
    public async Task<IActionResult> ExportarCsv(
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim)
    {
        try
        {
            var empresaId = ObterEmpresaId();

            var arquivo =
                await _service.GerarCsvAsync(
                    empresaId,
                    dataInicio,
                    dataFim);

            var nomeArquivo =
                $"relatorio-financeiro-{DateTime.UtcNow:yyyyMMddHHmmss}.csv";

            return File(
                arquivo,
                "text/csv",
                nomeArquivo);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }


    // ============================================================
    // EMPRESA DO TOKEN
    // ============================================================

    private Guid ObterEmpresaId()
    {
        var empresaIdClaim =
            User.FindFirst("EmpresaId")?.Value;

        if (!Guid.TryParse(
                empresaIdClaim,
                out var empresaId))
        {
            throw new InvalidOperationException(
                "Empresa não identificada no token.");
        }

        return empresaId;
    }
}