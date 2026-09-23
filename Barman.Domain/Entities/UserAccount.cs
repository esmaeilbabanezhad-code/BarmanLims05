using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class UserAccount : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public string Username { get; set; } = "";

    public string PasswordHash { get; set; } = "";

    public DateTimeOffset? LastLoginAt { get; set; }
}