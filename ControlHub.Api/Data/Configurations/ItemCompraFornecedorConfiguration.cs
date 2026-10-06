using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlHub.Api.Data.Configurations;

public class ItemCompraFornecedorConfiguration
    : IEntityTypeConfiguration<ItemCompraFornecedor>
{
    public void Configure(EntityTypeBuilder<ItemCompraFornecedor> builder)
    {
        builder.ToTable("ItensCompraFornecedor");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Quantidade)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(x => x.CustoUnitario)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.CustoTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(x => x.CompraFornecedor)
            .WithMany(x => x.Itens)
            .HasForeignKey(x => x.CompraFornecedorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Produto)
            .WithMany()
            .HasForeignKey(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}