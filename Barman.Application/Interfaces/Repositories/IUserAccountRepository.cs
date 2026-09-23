using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface IUserAccountRepository
{
    Task<List<UserAccount>> GetAllAsync();

    Task<UserAccount?> GetByUsernameAsync(string username);

    Task<UserAccount?> GetByEmployeeIdAsync(Guid employeeId);

    Task<UserAccount?> GetByIdAsync(Guid id);

    Task<bool> ExistsByUsernameAsync(
        string username,
        Guid? excludeId = null);

    Task AddAsync(UserAccount account);

    void Update(UserAccount account);
}