using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlHub.Api.Data.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Descricao)
            .HasMaxLength(1000);

        builder.Property(x => x.PrecoVenda)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.ImagemUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.ImagemArquivo)
            .HasMaxLength(500);

        builder.Property(x => x.ControlaEstoque)
            .IsRequired();

        builder.Property(x => x.Ativo)
            .IsRequired();

        builder.Property(x => x.CriadoEm)
            .IsRequired();

        builder.HasIndex(x => new { x.EmpresaId, x.Nome });

        builder.HasOne(x => x.Empresa)
            .WithMany()
            .HasForeignKey(x => x.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Secao)
            .WithMany(x => x.Produtos)
            .HasForeignKey(x => x.SecaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Estoque)
            .WithOne(x => x.Produto)
            .HasForeignKey<Estoque>(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}