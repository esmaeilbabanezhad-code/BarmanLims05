using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class Employee : BaseEntity
{
    public string PersonnelCode { get; set; } = "";

    public string FullName { get; set; } = "";

    public string NationalCode { get; set; } = "";

    public string? Mobile { get; set; }

    public string? Email { get; set; }

    public Guid DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    public Guid RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public bool CanApprove { get; set; }

    public bool CanReview { get; set; }

    public bool IsSystemUser { get; set; }
}