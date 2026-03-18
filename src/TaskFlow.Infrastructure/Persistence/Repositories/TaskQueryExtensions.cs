using System;
using TaskFlow.Domain.Enums;
using TaskEntity = TaskFlow.Domain.Entities.Task;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public static class TaskQueryExtensions
{
    public static IQueryable<TaskEntity> ApplyTaskFilter(this IQueryable<TaskEntity> query, TaskFilter filter)
    {
        return filter switch
        {
            TaskFilter.All => query,
            TaskFilter.OnlyComplete => query.Where(t => t.IsCompleted),
            TaskFilter.OnlyIncomplete => query.Where(t => !t.IsCompleted),
            _ => throw new InvalidOperationException("Task filter is incorrect.")
        };
    }
}