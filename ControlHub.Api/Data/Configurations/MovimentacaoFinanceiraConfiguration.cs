using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlHub.Api.Data.Configurations;

public class MovimentacaoFinanceiraConfiguration
    : IEntityTypeConfiguration<MovimentacaoFinanceira>
{
    public void Configure(EntityTypeBuilder<MovimentacaoFinanceira> builder)
    {
        builder.ToTable("MovimentacoesFinanceiras");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Categoria)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Descricao)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(x => x.Valor)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Entrada)
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
    }
}