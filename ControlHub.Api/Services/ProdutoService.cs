using ControlHub.Api.Data;
using ControlHub.Api.DTOs.Produtos;
using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlHub.Api.Services;

public class ProdutoService
{
    private readonly ControlHubDbContext _context;

    public ProdutoService(ControlHubDbContext context)
    {
        _context = context;
    }

    public async Task<Produto> CriarAsync(
        Guid empresaId,
        CriarProdutoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new InvalidOperationException(
                "Nome do produto é obrigatório.");

        if (dto.PrecoVenda <= 0)
            throw new InvalidOperationException(
                "O preço de venda deve ser maior que zero.");

        var secao = await _context.Secoes
            .FirstOrDefaultAsync(x =>
                x.Id == dto.SecaoId &&
                x.EmpresaId == empresaId &&
                x.Ativa);

        if (secao is null)
            throw new InvalidOperationException(
                "Seção não encontrada ou inativa.");

        if (dto.ControlaEstoque &&
            (!dto.QuantidadeInicial.HasValue ||
             dto.QuantidadeInicial.Value < 0))
        {
            throw new InvalidOperationException(
                "Informe uma quantidade inicial válida para o estoque.");
        }

        if (dto.ControlaEstoque &&
            dto.QuantidadeMinima.HasValue &&
            dto.QuantidadeMinima.Value < 0)
        {
            throw new InvalidOperationException(
                "A quantidade mínima não pode ser negativa.");
        }

        var produto = new Produto
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            SecaoId = dto.SecaoId,
            Nome = dto.Nome.Trim(),
            Descricao = dto.Descricao?.Trim(),
            PrecoVenda = dto.PrecoVenda,
            ImagemUrl = dto.ImagemUrl?.Trim(),
            ControlaEstoque = dto.ControlaEstoque,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        _context.Produtos.Add(produto);

        if (dto.ControlaEstoque)
        {
            var estoque = new Estoque
            {
                Id = Guid.NewGuid(),
                EmpresaId = empresaId,
                ProdutoId = produto.Id,
                Quantidade = dto.QuantidadeInicial!.Value,
                QuantidadeMinima = dto.QuantidadeMinima ?? 0,
                AtualizadoEm = DateTime.UtcNow
            };

            _context.Estoques.Add(estoque);

            if (dto.QuantidadeInicial.Value > 0)
            {
                _context.MovimentacoesEstoque.Add(
                    new MovimentacaoEstoque
                    {
                        Id = Guid.NewGuid(),
                        EmpresaId = empresaId,
                        ProdutoId = produto.Id,
                        Tipo = TipoMovimentacaoEstoque.Entrada,
                        Quantidade = dto.QuantidadeInicial.Value,
                        QuantidadeAnterior = 0,
                        QuantidadeAtual = dto.QuantidadeInicial.Value,
                        Observacao = "Estoque inicial",
                        CriadaEm = DateTime.UtcNow
                    });
            }
        }

        await _context.SaveChangesAsync();

        return produto;
    }

    public async Task<List<ProdutoResponseDto>> ListarAsync(Guid empresaId)
    {
        return await _context.Produtos
            .AsNoTracking()
            .Where(x => x.EmpresaId == empresaId)
            .OrderBy(x => x.Nome)
            .Select(x => new ProdutoResponseDto
            {
                Id = x.Id,
                EmpresaId = x.EmpresaId,
                SecaoId = x.SecaoId,
                Nome = x.Nome,
                Descricao = x.Descricao,
                PrecoVenda = x.PrecoVenda,
                ImagemUrl = x.ImagemUrl,
                ImagemArquivo = x.ImagemArquivo,
                ControlaEstoque = x.ControlaEstoque,
                Ativo = x.Ativo,
                CriadoEm = x.CriadoEm,
                QuantidadeEstoque = x.ControlaEstoque
                    ? x.Estoque!.Quantidade
                    : null,
                QuantidadeMinima = x.ControlaEstoque
                    ? x.Estoque!.QuantidadeMinima
                    : null
            })
            .ToListAsync();
    }

    public async Task<Produto?> ObterPorIdAsync(
        Guid empresaId,
        Guid id)
    {
        return await _context.Produtos
            .Include(x => x.Estoque)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.EmpresaId == empresaId);
    }

    public async Task<Produto> AtualizarAsync(
        Guid empresaId,
        Guid id,
        AtualizarProdutoDto dto)
    {
        var produto = await ObterPorIdAsync(empresaId, id);

        if (produto is null)
            throw new InvalidOperationException(
                "Produto não encontrado.");

        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new InvalidOperationException(
                "Nome do produto é obrigatório.");

        if (dto.PrecoVenda <= 0)
            throw new InvalidOperationException(
                "O preço de venda deve ser maior que zero.");

        var secao = await _context.Secoes
            .FirstOrDefaultAsync(x =>
                x.Id == dto.SecaoId &&
                x.EmpresaId == empresaId &&
                x.Ativa);

        if (secao is null)
            throw new InvalidOperationException(
                "Seção não encontrada ou inativa.");

        produto.SecaoId = dto.SecaoId;
        produto.Nome = dto.Nome.Trim();
        produto.Descricao = dto.Descricao?.Trim();
        produto.PrecoVenda = dto.PrecoVenda;
        produto.ImagemUrl = dto.ImagemUrl?.Trim();

        if (dto.ControlaEstoque != produto.ControlaEstoque)
        {
            if (!dto.ControlaEstoque)
            {
                produto.ControlaEstoque = false;
            }
            else
            {
                produto.ControlaEstoque = true;

                if (produto.Estoque is null)
                {
                    _context.Estoques.Add(
                        new Estoque
                        {
                            Id = Guid.NewGuid(),
                            EmpresaId = empresaId,
                            ProdutoId = produto.Id,
                            Quantidade = 0,
                            QuantidadeMinima = dto.QuantidadeMinima ?? 0,
                            AtualizadoEm = DateTime.UtcNow
                        });
                }
            }
        }

        if (produto.ControlaEstoque &&
            produto.Estoque is not null &&
            dto.QuantidadeMinima.HasValue)
        {
            if (dto.QuantidadeMinima.Value < 0)
                throw new InvalidOperationException(
                    "A quantidade mínima não pode ser negativa.");

            produto.Estoque.QuantidadeMinima =
                dto.QuantidadeMinima.Value;

            produto.Estoque.AtualizadoEm = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return produto;
    }

    public async Task BloquearAsync(
        Guid empresaId,
        Guid id,
        BloquearProdutoDto dto)
    {
        var produto = await ObterPorIdAsync(empresaId, id);

        if (produto is null)
            throw new InvalidOperationException(
                "Produto não encontrado.");

        produto.Ativo = false;

        if (produto.Estoque is not null)
        {
            var quantidadeAnterior = produto.Estoque.Quantidade;

            produto.Estoque.Quantidade = 0;
            produto.Estoque.AtualizadoEm = DateTime.UtcNow;

            if (quantidadeAnterior > 0)
            {
                _context.MovimentacoesEstoque.Add(
                    new MovimentacaoEstoque
                    {
                        Id = Guid.NewGuid(),
                        EmpresaId = empresaId,
                        ProdutoId = produto.Id,
                        Tipo = TipoMovimentacaoEstoque.BloqueioProduto,
                        Quantidade = quantidadeAnterior,
                        QuantidadeAnterior = quantidadeAnterior,
                        QuantidadeAtual = 0,
                        Observacao = dto.Motivo?.Trim() ??
                                     "Produto bloqueado.",
                        CriadaEm = DateTime.UtcNow
                    });
            }
        }

        await _context.SaveChangesAsync();
    }
    public async Task AtivarAsync(
        Guid empresaId,
        Guid id)
    {
        var produto = await ObterPorIdAsync(empresaId, id);

        if (produto is null)
            throw new InvalidOperationException(
                "Produto não encontrado.");

        if (produto.Ativo)
            throw new InvalidOperationException(
                "O produto já está ativo.");

        produto.Ativo = true;

        await _context.SaveChangesAsync();
    }
}