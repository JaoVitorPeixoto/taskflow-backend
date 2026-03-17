using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Configurations;

public class ListConfiguration : EntityBaseConfiguration<List>
{

    protected override void ConfigureEntity(EntityTypeBuilder<List> builder)
    {
        builder.Property(x => x.Title)
            .HasColumnName("Titulo")
            .HasMaxLength(100)
            .IsRequired(true);

        
        builder.Property(x => x.Description)
            .HasColumnName("Descricao")
            .HasMaxLength(500)
            .IsRequired(false);
        

        builder.Property(x => x.UserId)
            .HasColumnName("IdUsuario")
            .IsRequired(true);


        builder.HasOne(x => x.User)
            .WithMany(u => u.Lists)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Tasks)
            .WithOne(t => t.List)
            .HasForeignKey(t => t.ListId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
