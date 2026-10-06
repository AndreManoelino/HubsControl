using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlHub.Api.Data.Configurations;

public class VendaConfiguration : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.ToTable("Vendas");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ValorTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.CriadaEm)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.EmpresaId,
            x.CriadaEm
        });

        builder.HasOne(x => x.Empresa)
            .WithMany()
            .HasForeignKey(x => x.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Itens)
            .WithOne(x => x.Venda)
            .HasForeignKey(x => x.VendaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}