using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlHub.Api.Data.Configurations;

public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        builder.ToTable("Empresas");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Cpf)
            .IsRequired()
            .HasMaxLength(11);

        builder.HasIndex(x => x.Cpf)
            .IsUnique();

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.NomeFantasia)
            .HasMaxLength(150);

        builder.Property(x => x.LogoUrl)
            .HasMaxLength(500);

        builder.Property(x => x.ImagemLoginUrl)
            .HasMaxLength(500);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(300);

        builder.HasIndex(x => x.Url)
            .IsUnique();

        builder.Property(x => x.Ativa)
            .IsRequired();

        builder.Property(x => x.CriadaEm)
            .IsRequired();

        // Empresa possui um dono
       builder.HasOne(x => x.Dono)
            .WithOne()
            .HasForeignKey<Empresa>(x => x.DonoId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Empresa possui vários usuários
        builder.HasMany(x => x.Usuarios)
            .WithOne(x => x.Empresa)
            .HasForeignKey(x => x.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}