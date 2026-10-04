using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlHub.Api.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.SenhaHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Perfil)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Ativo)
            .IsRequired();

        builder.Property(x => x.CriadoEm)
            .IsRequired();

        // Usuário pertence a uma empresa
        builder.HasOne(x => x.Empresa)
            .WithMany(x => x.Usuarios)
            .HasForeignKey(x => x.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}