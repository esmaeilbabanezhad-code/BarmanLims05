using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class EmployeeTechnicalManagerScope : BaseEntity
{
    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public Guid TechnicalManagerScopeId { get; set; }

    public TechnicalManagerScope TechnicalManagerScope { get; set; } = null!;

    public bool IsPrimary { get; set; } = false;
}