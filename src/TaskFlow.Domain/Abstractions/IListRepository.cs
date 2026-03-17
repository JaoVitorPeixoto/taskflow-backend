using System;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.Abstractions;

public interface IListRepository : IRepositoryBase<List>
{
    Task<IEnumerable<List>> GetLitsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
