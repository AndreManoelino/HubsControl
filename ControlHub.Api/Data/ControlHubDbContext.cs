using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlHub.Api.Data;

public class ControlHubDbContext : DbContext
{
    public ControlHubDbContext(
        DbContextOptions<ControlHubDbContext> options)
        : base(options)
    {
    }

    public DbSet<Empresa> Empresas { get; set; } = null!;

    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Secao> Secoes { get; set; } = null!;
    public DbSet<Produto> Produtos { get; set; } = null!;
    public DbSet<Estoque> Estoques { get; set; } = null!;
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; } = null!;
    public DbSet<Venda> Vendas { get; set; } = null!;
    public DbSet<ItemVenda> ItensVenda { get; set; } = null!;
    public DbSet<Fornecedor> Fornecedores { get; set; } = null!;
    public DbSet<CompraFornecedor> ComprasFornecedor { get; set; } = null!;
    public DbSet<ItemCompraFornecedor> ItensCompraFornecedor { get; set; } = null!;
    public DbSet<MovimentacaoFinanceira> MovimentacoesFinanceiras { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ControlHubDbContext).Assembly);
    }
}