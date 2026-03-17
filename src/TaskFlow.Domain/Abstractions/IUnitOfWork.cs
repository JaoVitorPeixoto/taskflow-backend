using System;

namespace TaskFlow.Domain.Abstractions;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IListRepository ListRepository { get; }
    ITaskRepository TaskRepository { get; }

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
