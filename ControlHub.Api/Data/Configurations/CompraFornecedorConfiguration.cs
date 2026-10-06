using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlHub.Api.Data.Configurations;

public class CompraFornecedorConfiguration
    : IEntityTypeConfiguration<CompraFornecedor>
{
    public void Configure(EntityTypeBuilder<CompraFornecedor> builder)
    {
        builder.ToTable("ComprasFornecedor");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ValorTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.NumeroNota)
            .HasMaxLength(100);

        builder.Property(x => x.DataCompra)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.EmpresaId,
            x.DataCompra
        });

        builder.HasOne(x => x.Empresa)
            .WithMany()
            .HasForeignKey(x => x.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Fornecedor)
            .WithMany()
            .HasForeignKey(x => x.FornecedorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Itens)
            .WithOne(x => x.CompraFornecedor)
            .HasForeignKey(x => x.CompraFornecedorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}