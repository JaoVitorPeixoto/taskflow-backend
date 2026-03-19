using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using TaskFlow.Domain.Abstractions;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.ValueObjects;

using Task = System.Threading.Tasks.Task;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{

    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;   
    }

    public async Task AddAsync(User entity, CancellationToken cancellationToken =default)
    {
       await  _context.Users.AddAsync(entity, cancellationToken);
    }

    public Task DeleteAsync(User entity)
    {
        _context.Users.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllAsync(Expression<Func<User, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        var query = filter != null ? _context.Users.Where(filter) : _context.Users;
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public Task UpdateAsync(User entity)
    {
        _context.Users.Update(entity);
        return Task.CompletedTask;
    }
}
