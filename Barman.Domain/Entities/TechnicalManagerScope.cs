using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class TechnicalManagerScope : BaseEntity
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Description { get; set; }

   

    public int Priority { get; set; } = 100;

    public ICollection<EmployeeTechnicalManagerScope> EmployeeScopes { get; set; }
        = new List<EmployeeTechnicalManagerScope>();
}