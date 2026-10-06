using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlHub.Api.Data.Configurations;

public class EstoqueConfiguration : IEntityTypeConfiguration<Estoque>
{
    public void Configure(EntityTypeBuilder<Estoque> builder)
    {
        builder.ToTable("Estoques");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Quantidade)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(x => x.QuantidadeMinima)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(x => x.AtualizadoEm)
            .IsRequired();

        builder.HasIndex(x => new { x.EmpresaId, x.ProdutoId })
            .IsUnique();

        builder.HasOne(x => x.Empresa)
            .WithMany()
            .HasForeignKey(x => x.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Produto)
            .WithOne(x => x.Estoque)
            .HasForeignKey<Estoque>(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}