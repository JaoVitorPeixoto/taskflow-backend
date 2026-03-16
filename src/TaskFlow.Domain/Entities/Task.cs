using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.ValueObjects;

namespace TaskFlow.Domain.Entities;

public class Task : EntityBase
{
    public Guid? ListId { get; private set; }
    public Guid UserId { get; private init; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public TaskPriority Priority { get; private set; }
    public Scheduling? Scheduling { get; private set; }
    public bool Notify { get; private set; }


    // Navegações
    public User User { get; private set; }
    public List? List { get; private set; }

    // Ef Core
    private Task() {}

    public Task(
        Guid userId,
        string title,
        string? description = null,
        Guid? listId = null,
        TaskPriority priority = TaskPriority.P4,
        Scheduling? scheduling = null,
        bool notify = false
    ) : base()
    {
        
        if (string.IsNullOrWhiteSpace(title))
            throw new DataIsInvalidException("TITLE_IS_EMPTY", "Title cannot be empty.");
        
        if (notify == true && (scheduling == null || scheduling.Time == null))
            throw new DataIsInvalidException("NOTIFY_WITH_SCHEDULING_TIME_IS_NULL", "In order to issue a notification, the scheduling and time cannot be null.");

        this.ListId = listId;
        this.UserId = userId;
        this.Title = title;
        this.Description = description;
        this.IsCompleted = false;
        this.CompletedAt = null;
        this.Priority = priority;
        this.Scheduling = scheduling;
        this.Notify = notify;
    }

    private void CanEditTask()
    {
         if (this.IsCompleted)
            throw new TaskIsCompletedException();
    }

    public void UpdateDetails(string title, string? description = null)
    {
        CanEditTask();

        if (string.IsNullOrWhiteSpace(title))
            throw new DataIsInvalidException("TITLE_IS_EMPTY", "Title cannot be empty.");

        this.Title = title;
        this.Description = description;

        this.UpdateAudit();
    }

    public void UpdateScheduling(Scheduling scheduling)
    {
        CanEditTask();

        this.Scheduling = scheduling;
        
        if (scheduling.Time == null)
            this.Notify = false;
        
        this.UpdateAudit();
    }

    public void ShouldNotify()
    {
        CanEditTask();

        if (Notify)
            return;
        
        Notify = true;

        this.UpdateAudit();
    }

    public void RemoveNotify()
    {
        CanEditTask();

        if (!Notify)
            return;
        
        Notify = false;

        this.UpdateAudit();
    }

    public void RemoveScheduling()
    {
        CanEditTask();
        
        this.Scheduling = null;
        this.Notify = false;

        this.UpdateAudit();
    }

    public void UpdatePriority(TaskPriority priority)
    {
        CanEditTask();

        this.Priority = priority;

        this.UpdateAudit();
    }

    public void MoveToList(Guid listId)
    {
        CanEditTask();

        this.ListId = listId;

        this.UpdateAudit();
    }

    public void RemoveList()
    {
        CanEditTask();

        this.ListId = null;

        this.UpdateAudit();
    }

    public Task? Complete()
    {
        CanEditTask();

        this.IsCompleted = true;
        this.CompletedAt = DateTime.UtcNow;

        if (Scheduling == null || !Scheduling.IsRecurring)
            return null;

        var nextDate = Scheduling.Recurrence switch
        {
          RecurrenceSchedule.Daily => Scheduling.Date.AddDays(1),
          RecurrenceSchedule.Weekly => Scheduling.Date.AddDays(7),
          RecurrenceSchedule.Monthly => Scheduling.Date.AddMonths(1),
          RecurrenceSchedule.Annual => Scheduling.Date.AddYears(1),
          _=> (DateOnly?)null  
        };

        if (nextDate is null)
            return null;

        var newTask = new Task(
            UserId,
            Title,
            Description,
            ListId,
            Priority,
            Scheduling.Factory.Recurring(
                nextDate.Value,
                Scheduling.Time,
                Scheduling.Recurrence
            ),
            Notify
        );

        this.UpdateAudit();

        return newTask;
    }

}
