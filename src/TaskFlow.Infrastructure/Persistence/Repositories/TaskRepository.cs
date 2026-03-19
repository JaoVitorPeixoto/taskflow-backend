using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Abstractions;
using TaskFlow.Domain.Enums;
using Task = System.Threading.Tasks.Task;
using TaskEntity = TaskFlow.Domain.Entities.Task;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public class TaskRepository : ITaskRepository
{

    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;   
    }

    public async Task AddAsync(TaskEntity entity, CancellationToken cancellationToken =default)
    {
        await _context.Tasks.AddAsync(entity, cancellationToken);
    }

    public Task DeleteAsync(TaskEntity entity)
    {
        _context.Tasks.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<TaskEntity>> GetAllAsync(Expression<Func<TaskEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        var query = filter != null ? _context.Tasks.Where(filter) : _context.Tasks;
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetAllTasksByUserIdAsync(Guid userId, TaskFilter filter = TaskFilter.OnlyIncomplete, CancellationToken cancellationToken = default)
    {     
        return await _context.Tasks
            .Where(t => t.UserId == userId)
            .ApplyTaskFilter(filter)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetSubTasksByParentTaskIdAsync(Guid parentTaskId, TaskFilter filter = TaskFilter.OnlyIncomplete, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .Where(t => t.ParentTaskId == parentTaskId)
            .ApplyTaskFilter(filter)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetTasksByListIdAsync(Guid listId, TaskFilter filter = TaskFilter.OnlyIncomplete, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .Where(t => t.ListId == listId)
            .ApplyTaskFilter(filter)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetTodayTasksByUserIdAsync(Guid userId, TaskFilter filter = TaskFilter.OnlyIncomplete, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .Where(t => 
                t.UserId == userId 
                && t.Scheduling != null 
                && t.Scheduling.Date == DateOnly.FromDateTime(DateTime.UtcNow))
            .ApplyTaskFilter(filter)
            .ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(TaskEntity entity)
    {
        _context.Tasks.Update(entity);
        return Task.CompletedTask;
    }
}