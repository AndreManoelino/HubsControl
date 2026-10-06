using ControlHub.Api.Data;
using ControlHub.Api.DTOs.Fornecedores;
using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlHub.Api.Services;

public class FornecedorService
{
    private readonly ControlHubDbContext _context;

    public FornecedorService(ControlHubDbContext context)
    {
        _context = context;
    }

    public async Task<Fornecedor> CriarAsync(
        Guid empresaId,
        CriarFornecedorDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new InvalidOperationException(
                "Nome do fornecedor é obrigatório.");

        var nome = dto.Nome.Trim();

        var existe = await _context.Fornecedores
            .AnyAsync(x =>
                x.EmpresaId == empresaId &&
                x.Nome.ToLower() == nome.ToLower());

        if (existe)
            throw new InvalidOperationException(
                "Já existe um fornecedor com esse nome.");

        var fornecedor = new Fornecedor
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            Nome = nome,
            Documento = dto.Documento?.Trim(),
            Email = dto.Email?.Trim(),
            Telefone = dto.Telefone?.Trim(),
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        _context.Fornecedores.Add(fornecedor);

        await _context.SaveChangesAsync();

        return fornecedor;
    }

    public async Task<List<Fornecedor>> ListarAsync(
        Guid empresaId)
    {
        return await _context.Fornecedores
            .Where(x => x.EmpresaId == empresaId)
            .OrderBy(x => x.Nome)
            .ToListAsync();
    }

    public async Task<Fornecedor?> ObterPorIdAsync(
        Guid empresaId,
        Guid id)
    {
        return await _context.Fornecedores
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.EmpresaId == empresaId);
    }

    public async Task<Fornecedor> AtualizarAsync(
        Guid empresaId,
        Guid id,
        AtualizarFornecedorDto dto)
    {
        var fornecedor = await ObterPorIdAsync(
            empresaId,
            id);

        if (fornecedor is null)
            throw new InvalidOperationException(
                "Fornecedor não encontrado.");

        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new InvalidOperationException(
                "Nome do fornecedor é obrigatório.");

        var nome = dto.Nome.Trim();

        var existe = await _context.Fornecedores
            .AnyAsync(x =>
                x.EmpresaId == empresaId &&
                x.Id != id &&
                x.Nome.ToLower() == nome.ToLower());

        if (existe)
            throw new InvalidOperationException(
                "Já existe um fornecedor com esse nome.");

        fornecedor.Nome = nome;
        fornecedor.Documento = dto.Documento?.Trim();
        fornecedor.Email = dto.Email?.Trim();
        fornecedor.Telefone = dto.Telefone?.Trim();
        fornecedor.Ativo = dto.Ativo;

        await _context.SaveChangesAsync();

        return fornecedor;
    }

    public async Task BloquearAsync(
        Guid empresaId,
        Guid id)
    {
        var fornecedor = await ObterPorIdAsync(
            empresaId,
            id);

        if (fornecedor is null)
            throw new InvalidOperationException(
                "Fornecedor não encontrado.");

        fornecedor.Ativo = false;

        await _context.SaveChangesAsync();
    }
}