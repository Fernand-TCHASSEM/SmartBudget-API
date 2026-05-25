using SmartBudget.Domain.Entities;

namespace SmartBudget.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    public Task<bool> ExistsByEmailAsync(string email);

    public Task<User> AddAsync(User user);

    public Task<User?> GetByIdAsync(string id);
}
