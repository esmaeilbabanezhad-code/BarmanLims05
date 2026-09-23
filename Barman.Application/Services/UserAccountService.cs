using System.Security.Cryptography;
using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class UserAccountService : IUserAccountService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserAccountService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<UserAccount>> GetAllAsync(
    CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.UserAccounts.GetAllAsync();
    }

    public async Task<UserAccount?> GetByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
            return null;

        return await _unitOfWork.UserAccounts
            .GetByUsernameAsync(username.Trim());
    }

    public async Task<UserAccount?> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        if (employeeId == Guid.Empty)
            return null;

        return await _unitOfWork.UserAccounts
            .GetByEmployeeIdAsync(employeeId);
    }

    public async Task<UserAccount?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        return await _unitOfWork.UserAccounts
            .GetByIdAsync(id);
    }
    public async Task<UserAccount?> AuthenticateAsync(
    string username,
    string password,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
            return null;

        var account =
            await _unitOfWork.UserAccounts
                .GetByUsernameAsync(username.Trim());

        if (account == null ||
            !account.IsActive ||
            account.IsDeleted ||
            account.Employee == null ||
            !account.Employee.IsActive ||
            account.Employee.IsDeleted)
            return null;

        if (!VerifyPassword(password, account.PasswordHash))
            return null;

        account.LastLoginAt = DateTimeOffset.UtcNow;
        account.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.UserAccounts.Update(account);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return account;
    }
    public async Task<UserAccount> CreateAsync(
        Guid employeeId,
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        if (employeeId == Guid.Empty)
            throw new ArgumentException(
                "Employee is required.");

        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException(
                "Username is required.");

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException(
                "Password is required.");

        username = username.Trim();

        if (await _unitOfWork.UserAccounts
                .ExistsByUsernameAsync(username))
        {
            throw new InvalidOperationException(
                "این نام کاربری قبلاً استفاده شده است.");
        }

        var existingAccount =
            await _unitOfWork.UserAccounts
                .GetByEmployeeIdAsync(employeeId);

        if (existingAccount != null)
        {
            throw new InvalidOperationException(
                "این کارمند قبلاً حساب کاربری دارد.");
        }

        var account = new UserAccount
        {
            EmployeeId = employeeId,
            Username = username,
            PasswordHash = HashPassword(password),
            IsActive = true,
            IsDeleted = false
        };

        await _unitOfWork.UserAccounts.AddAsync(account);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return account;
    }

    public async Task<bool> UpdateAsync(
        UserAccount account,
        CancellationToken cancellationToken = default)
    {
        if (account == null ||
            account.Id == Guid.Empty)
            return false;

        if (string.IsNullOrWhiteSpace(account.Username))
            throw new ArgumentException(
                "Username is required.");

        var duplicate =
            await _unitOfWork.UserAccounts
                .ExistsByUsernameAsync(
                    account.Username.Trim(),
                    account.Id);

        if (duplicate)
        {
            throw new InvalidOperationException(
                "این نام کاربری قبلاً استفاده شده است.");
        }

        account.Username =
            account.Username.Trim();

        _unitOfWork.UserAccounts.Update(account);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> ChangePasswordAsync(
        Guid accountId,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        if (accountId == Guid.Empty)
            return false;

        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException(
                "Password is required.");

        var account =
            await _unitOfWork.UserAccounts
                .GetByIdAsync(accountId);

        if (account == null)
            return false;

        account.PasswordHash =
            HashPassword(newPassword);

        account.ModifiedAt =
            DateTimeOffset.UtcNow;

        _unitOfWork.UserAccounts.Update(account);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> DeactivateAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        var account =
            await _unitOfWork.UserAccounts
                .GetByIdAsync(accountId);

        if (account == null)
            return false;

        account.IsActive = false;
        account.ModifiedAt =
            DateTimeOffset.UtcNow;

        _unitOfWork.UserAccounts.Update(account);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> ActivateAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        var account =
            await _unitOfWork.UserAccounts
                .GetByIdAsync(accountId);

        if (account == null)
            return false;

        account.IsDeleted = false;
        account.IsActive = true;
        account.ModifiedAt =
            DateTimeOffset.UtcNow;

        _unitOfWork.UserAccounts.Update(account);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    private static string HashPassword(
        string password)
    {
        const int iterations = 100_000;
        const int saltSize = 16;
        const int keySize = 32;

        var salt = RandomNumberGenerator
            .GetBytes(saltSize);

        var key = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            keySize);

        return string.Join(
            ".",
            "PBKDF2",
            iterations,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(key));
    }
    private static bool VerifyPassword(
    string password,
    string storedHash)
    {
        if (string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(storedHash))
            return false;

        var parts = storedHash.Split('.');

        if (parts.Length != 4 ||
            parts[0] != "PBKDF2" ||
            !int.TryParse(parts[1], out var iterations))
            return false;

        try
        {
            var salt = Convert.FromBase64String(parts[2]);
            var expectedKey = Convert.FromBase64String(parts[3]);

            var actualKey = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expectedKey.Length);

            return CryptographicOperations.FixedTimeEquals(
                actualKey,
                expectedKey);
        }
        catch
        {
            return false;
        }
    }
}