using Barman.Application.Interfaces.Repositories;
using Barman.Application.Interfaces.Repositories.Reporting;

namespace Barman.Application.Interfaces;

public interface IUnitOfWork
{
    IReceptionRepository Receptions { get; }

    INumberSequenceRepository NumberSequences { get; }

    ICustomerRepository Customers { get; }

    IOrganizationTypeRepository OrganizationTypes { get; }

    ITestTariffRepository TestTariffs { get; }

    ISampleRepository Samples { get; }

    ICustomFieldDefinitionRepository CustomFieldDefinitions { get; }

    ICustomFieldValueRepository CustomFieldValues { get; }

    ISampleCategoryRepository SampleCategories { get; }

    IMatrixRepository Matrices { get; }

    ITestRepository Tests { get; }

    IReferenceLimitRepository ReferenceLimits { get; }

    ILimitReferenceRepository LimitReferences { get; }

    ITestLimitRuleRepository TestLimitRules { get; }

    ITestMethodRepository TestMethods { get; }

    IInstrumentRepository Instruments { get; }

    ITestPanelRepository TestPanels { get; }

    IStandardSampleRepository StandardSamples { get; }

    ITestPanelItemRepository TestPanelItems { get; }

    ITestResultDefinitionRepository TestResultDefinitions { get; }

    ICustomerTestPanelRepository CustomerTestPanels { get; }

    IDefaultTestSetRepository DefaultTestSets { get; }

    IDepartmentRepository Departments { get; }

    IRoleRepository Roles { get; }

    IRolePermissionRepository RolePermissions { get; }

    IPermissionRepository Permissions { get; }

    ITestAssignmentRepository TestAssignments { get; }


    ITestAssignmentResultValueRepository TestAssignmentResultValues { get; }

    ITestResultReviewRepository TestResultReviews { get; }

    ITestResultSetRepository TestResultSets { get; }

    ITestResultSetItemRepository TestResultSetItems { get; }

    ITestLimitChangeRequestRepository TestLimitChangeRequests { get; }

    IReceptionCorrectionRequestRepository ReceptionCorrectionRequests { get; }

    IEmployeeRepository Employees { get; }

    IUserAccountRepository UserAccounts { get; }

    IEmployeeDepartmentRepository EmployeeDepartments { get; }

    IEmployeeRoleRepository EmployeeRoles { get; }

    IDepartmentResponsibilityRepository DepartmentResponsibilities { get; }

    ITechnicalManagerSectionHeadRepository TechnicalManagerSectionHeads { get; }

    ITechnicalManagerScopeRepository TechnicalManagerScopes { get; }

    IEmployeeTechnicalManagerScopeRepository EmployeeTechnicalManagerScopes { get; }

    ITechnicalManagerScopeDepartmentRepository TechnicalManagerScopeDepartments { get; }

    ITechnicalManagerRoutingRuleRepository TechnicalManagerRoutingRules { get; }

    IReportTemplateRepository ReportTemplates { get; }

    IIssuedReportRepository IssuedReports { get; }
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
