using System;
using System.Linq.Expressions;
using TaskFlow.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace TaskFlow.Domain.Abstractions;

public interface IRepositoryBase<T> where T : EntityBase
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken =default);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
