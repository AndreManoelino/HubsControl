using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlHub.Api.Data.Configurations;

public class MovimentacaoEstoqueConfiguration
    : IEntityTypeConfiguration<MovimentacaoEstoque>
{
    public void Configure(EntityTypeBuilder<MovimentacaoEstoque> builder)
    {
        builder.ToTable("MovimentacoesEstoque");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Tipo)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Quantidade)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(x => x.QuantidadeAnterior)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(x => x.QuantidadeAtual)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(x => x.Observacao)
            .HasMaxLength(500);

        builder.Property(x => x.CriadaEm)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.EmpresaId,
            x.ProdutoId,
            x.CriadaEm
        });

        builder.HasOne(x => x.Empresa)
            .WithMany()
            .HasForeignKey(x => x.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Produto)
            .WithMany()
            .HasForeignKey(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}