using TaskFlow.Domain.Entities;
using TaskFlow.Domain.ValueObjects;

namespace TaskFlow.Domain.Abstractions;

public interface IUserRepository : IRepositoryBase<User>    
{   
    Task<User> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default);
}
