using Microsoft.EntityFrameworkCore.Storage;
using TaskFlow.Domain.Abstractions;
using Task = System.Threading.Tasks.Task;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;


    // Para lazy loading
    private IUserRepository? _userRepository;
    private IListRepository? _listRepository;
    private ITaskRepository? _taskRepository;

    public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);
    public IListRepository ListRepository => _listRepository ??= new ListRepository(_context);
    public ITaskRepository TaskRepository => _taskRepository ??= new TaskRepository(_context);


    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);


    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
            return;

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            return;

        await _transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            return;

        await _transaction.RollbackAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
            await _transaction.DisposeAsync();

        await _context.DisposeAsync();
    }
}
