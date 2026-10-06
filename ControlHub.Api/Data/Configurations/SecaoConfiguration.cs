using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlHub.Api.Data.Configurations;

public class SecaoConfiguration : IEntityTypeConfiguration<Secao>
{
    public void Configure(EntityTypeBuilder<Secao> builder)
    {
        builder.ToTable("Secoes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Ativa)
            .IsRequired();

        builder.Property(x => x.CriadaEm)
            .IsRequired();

        builder.HasIndex(x => new { x.EmpresaId, x.Nome })
            .IsUnique();

        builder.HasOne(x => x.Empresa)
            .WithMany()
            .HasForeignKey(x => x.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Produtos)
            .WithOne(x => x.Secao)
            .HasForeignKey(x => x.SecaoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}