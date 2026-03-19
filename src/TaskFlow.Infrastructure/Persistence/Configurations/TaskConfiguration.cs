using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TaskFlow.Infrastructure.Persistence.Configurations.Converters;

namespace TaskFlow.Infrastructure.Persistence.Configurations;

public class TaskConfiguration : EntityBaseConfiguration<Domain.Entities.Task>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Domain.Entities.Task> builder)
    {
        builder.ToTable("Tarefa");

        builder.Property(x => x.ListId)
            .HasColumnName("IdLista")
            .IsRequired(false);

        builder.HasOne(x => x.List)
            .WithMany(l => l.Tasks)
            .HasForeignKey(x => x.ListId)
            .OnDelete(DeleteBehavior.SetNull);

        
        builder.Property(x => x.UserId)
            .HasColumnName("IdUsuario")
            .IsRequired(true);
        
        builder.HasOne(x => x.User)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.Property(x => x.ParentTaskId)
            .HasColumnName("IdTaskPai")
            .IsRequired(false);

        builder.HasOne(x => x.ParentTask)
            .WithMany(t => t.SubTasks)
            .HasForeignKey(t => t.ParentTaskId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.Navigation(x => x.SubTasks)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        
        builder.Property(x => x.Title)
            .HasColumnName("Titulo")
            .HasMaxLength(200)
            .IsRequired(true);

        
        builder.Property(x => x.Description)
            .HasColumnName("Descricao")
            .HasMaxLength(500)
            .IsRequired(false);

        
        builder.Property(x => x.IsCompleted)
            .HasColumnName("Completada")
            .HasDefaultValue(false);

        
        builder.Property(x => x.CompletedAt)
            .HasColumnName("DataCompletada")
            .IsRequired(false);


        builder.Property(x => x.Priority)
            .HasColumnName("Prioridade")
            .HasConversion<string>()
            .HasMaxLength(3)
            .IsRequired(true);
        

        builder.OwnsOne(x => x.Scheduling, scheduling =>
        {
            scheduling.Ignore(x => x.IsRecurring);
            
            scheduling.Property(x => x.Date)
                .HasColumnName("DataAgendamento")
                .IsRequired(true);

            scheduling.Property(x => x.Time)
                .HasColumnName("HoraAgendamento")
                .IsRequired(false);
            
            scheduling.Property(x => x.Recurrence)
                .HasColumnName("Recorrencia")
                .IsRequired(true)
                .HasMaxLength(20)
                .HasConversion(new RecurrenceScheduleConverter());           
        });


        builder.Property(x => x.Notify)
            .HasColumnName("NotificarAgendamento")
            .IsRequired(true);

    }
}
