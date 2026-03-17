using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Configurations;

public abstract class EntityBaseConfiguration<T> : IEntityTypeConfiguration<T> where T : EntityBase
{
    void IEntityTypeConfiguration<T>.Configure(EntityTypeBuilder<T> builder)
    {
        ConfigureBase(builder);
        ConfigureEntity(builder);
    }

    private static void ConfigureBase(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("DataCriado")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("DataAlterado")
            .IsRequired(false); 

    }

    protected abstract void ConfigureEntity(EntityTypeBuilder<T> builder);
    
}
