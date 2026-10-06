using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlHub.Api.Data.Configurations;

public class FornecedorConfiguration : IEntityTypeConfiguration<Fornecedor>
{
    public void Configure(EntityTypeBuilder<Fornecedor> builder)
    {
        builder.ToTable("Fornecedores");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Documento)
            .HasMaxLength(20);

        builder.Property(x => x.Email)
            .HasMaxLength(200);

        builder.Property(x => x.Telefone)
            .HasMaxLength(30);

        builder.Property(x => x.Ativo)
            .IsRequired();

        builder.Property(x => x.CriadoEm)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.EmpresaId,
            x.Nome
        });

        builder.HasOne(x => x.Empresa)
            .WithMany()
            .HasForeignKey(x => x.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}