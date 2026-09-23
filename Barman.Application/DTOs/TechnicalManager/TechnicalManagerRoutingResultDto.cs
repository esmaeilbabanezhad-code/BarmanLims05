namespace Barman.Application.DTOs.TechnicalManager;

public class TechnicalManagerRoutingResultDto
{
    public Guid TechnicalManagerId { get; set; }

    public Guid TechnicalManagerScopeId { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid RoutingRuleId { get; set; }
}