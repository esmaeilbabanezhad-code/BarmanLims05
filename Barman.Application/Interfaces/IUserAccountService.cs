using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface IUserAccountService
{

    Task<List<UserAccount>> GetAllAsync(
    CancellationToken cancellationToken = default);
    Task<UserAccount?> GetByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default);

    Task<UserAccount?> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);

    Task<UserAccount?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<UserAccount?> AuthenticateAsync(
            string username,
            string password,
            CancellationToken cancellationToken = default);

    Task<UserAccount> CreateAsync(
        Guid employeeId,
        string username,
        string password,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        UserAccount account,
        CancellationToken cancellationToken = default);

    Task<bool> ChangePasswordAsync(
        Guid accountId,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    Task<bool> ActivateAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);
}