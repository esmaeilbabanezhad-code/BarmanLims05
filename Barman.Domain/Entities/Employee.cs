using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class Employee : BaseEntity
{
    public string PersonnelCode { get; set; } = "";

    public string FullName { get; set; } = "";

    public string NationalCode { get; set; } = "";

    public string? Mobile { get; set; }

    public string? Email { get; set; }

    public bool CanApprove { get; set; }

    public bool CanReview { get; set; }

    public bool IsSystemUser { get; set; }

    public ICollection<EmployeeDepartment> EmployeeDepartments { get; set; }
        = new List<EmployeeDepartment>();

    public ICollection<EmployeeRole> EmployeeRoles { get; set; }
        = new List<EmployeeRole>();

    public ICollection<DepartmentResponsibility> DepartmentResponsibilities { get; set; }
        = new List<DepartmentResponsibility>();

    public ICollection<TechnicalManagerSectionHead> TechnicalManagerSectionHeads { get; set; }
        = new List<TechnicalManagerSectionHead>();

    public ICollection<TechnicalManagerSectionHead> SectionHeadTechnicalManagers { get; set; }
        = new List<TechnicalManagerSectionHead>();
}
