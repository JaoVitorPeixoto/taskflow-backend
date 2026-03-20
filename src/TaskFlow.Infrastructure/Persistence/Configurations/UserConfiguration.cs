using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Configurations;

public class UserConfiguration : EntityBaseConfiguration<User>
{
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Usuario");

        builder.Property(x => x.Name)
            .HasColumnName("Nome")
            .HasMaxLength(250)
            .IsRequired(true);

        
        builder.OwnsOne(x => x.Email, email =>
        {
            email.Property(e => e.Address)
                .HasColumnName("Email")
                .HasMaxLength(200)
                .IsRequired(true);  

            email.HasIndex(e => e.Address)
                .IsUnique();
        });

        
        builder.Property(x => x.Password)
            .HasColumnName("Senha")
            .HasMaxLength(255)
            .IsRequired();
        

        builder.OwnsOne(x => x.AvatarUrl, avatarUrl =>
        {
           avatarUrl.Property(a => a.Url)
            .HasColumnName("AvatarUrl")
            .IsRequired(false); 
        });


        builder.HasMany(x => x.Lists)
            .WithOne(l => l.User)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Navigation(x => x.Lists)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        

        builder.HasMany(x => x.Tasks)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Tasks)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsOne(x => x.UserTimeZone, userTimeZone =>
        {
            userTimeZone.Property(tz => tz.ZoneId)
                .HasColumnName("TimeZone")
                .HasMaxLength(120)
                .IsRequired(true);
        });
    }
}
