using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Abstractions;
using TaskFlow.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public class ListRepository : IListRepository
{
    private readonly AppDbContext _context;

    public ListRepository(AppDbContext context)
    {
        _context = context;   
    }


    public async Task AddAsync(List entity, CancellationToken cancellationToken =default)
    {
        await _context.AddAsync(entity, cancellationToken);
    }

    public Task DeleteAsync(List entity)
    {
        _context.Lists.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<List>> GetAllAsync(Expression<Func<List, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        var query = filter != null ? _context.Lists.Where(filter) : _context.Lists;
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Lists.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<List>> GetLitsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Lists.Where(l => l.UserId == userId).ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(List entity)
    {
        _context.Lists.Update(entity);
        return Task.CompletedTask;
    }
}
