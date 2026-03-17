using System;
using TaskFlow.Domain.Enums;
using TaskEntity = TaskFlow.Domain.Entities.Task;

namespace TaskFlow.Domain.Abstractions;

public interface ITaskRepository : IRepositoryBase<TaskEntity>
{
    Task<IEnumerable<TaskEntity>> GetTodayTasksByUserIdAsync(Guid userId, TaskFilter filter = TaskFilter.OnlyIncomplete, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskEntity>> GetAllTasksByUserIdAsync(Guid userId, TaskFilter filter = TaskFilter.OnlyIncomplete, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskEntity>> GetSubTasksByParentTaskIdAsync(Guid parentTaskId, TaskFilter filter = TaskFilter.OnlyIncomplete, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskEntity>> GetTasksByListIdAsync(Guid listId, TaskFilter filter = TaskFilter.OnlyIncomplete, CancellationToken cancellationToken = default);
}
