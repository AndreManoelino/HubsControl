using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlHub.Api.Data.Configurations;

public class ItemVendaConfiguration : IEntityTypeConfiguration<ItemVenda>
{
    public void Configure(EntityTypeBuilder<ItemVenda> builder)
    {
        builder.ToTable("ItensVenda");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.NomeProduto)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Quantidade)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(x => x.PrecoUnitario)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.ValorTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.CustoUnitario)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.CustoTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Lucro)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(x => x.Venda)
            .WithMany(x => x.Itens)
            .HasForeignKey(x => x.VendaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Produto)
            .WithMany()
            .HasForeignKey(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}