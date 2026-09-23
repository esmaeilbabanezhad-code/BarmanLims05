namespace Barman.Domain.Enums;

public enum TestAssignmentWorkflowStage
{
    TechnicalManagerAssignment = 0,
    SectionAssignment = 10,
    AnalystWork = 20,
    SectionResultApproval = 30,
    TechnicalManagerResultApproval = 40,
    DirectorResultApproval = 50,
    ReceptionCorrection = 55,
    Completed = 60
}