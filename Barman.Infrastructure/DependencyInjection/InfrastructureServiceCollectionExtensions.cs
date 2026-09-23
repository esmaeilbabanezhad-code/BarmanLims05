using Barman.Application.Interfaces;
using Barman.Application.Services;
using Barman.Application.Services.Resolvers;
using Barman.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Barman.Application.Interfaces.Excel;
using Barman.Infrastructure.Services.Excel;
using Barman.Application.Interfaces.Services.Reporting;
using Barman.Infrastructure.Services.Reporting;



namespace Barman.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<INumberGenerator, NumberGenerator>();
        services.AddScoped<INumberSequenceService, NumberSequenceService>();
        services.AddScoped<IReceptionService, ReceptionService>();
        services.AddScoped<ISampleCategoryService, SampleCategoryService>();
        services.AddScoped<IMatrixService, MatrixService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICustomFieldService, CustomFieldService>();
        services.AddScoped<ITestService, TestService>();
        services.AddScoped<TestAssignmentService>();
        services.AddScoped<ITestAssignmentWorkflowService, TestAssignmentWorkflowService>();
        services.AddScoped<ITestLimitChangeRequestService, TestLimitChangeRequestService>();
        services.AddScoped<IReceptionCorrectionRequestService,
                 ReceptionCorrectionRequestService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IUserAccountService, UserAccountService>();
        services.AddScoped<IEmployeeRoleService, EmployeeRoleService>();
        services.AddScoped<IDepartmentResponsibilityService, DepartmentResponsibilityService>();

        services.AddScoped<ITechnicalManagerScopeService,
            TechnicalManagerScopeService>();

        services.AddScoped<IEmployeeTechnicalManagerScopeService,
            EmployeeTechnicalManagerScopeService>();

        services.AddScoped<ITechnicalManagerScopeDepartmentService,
            TechnicalManagerScopeDepartmentService>();

        services.AddScoped<ITechnicalManagerSectionHeadService, TechnicalManagerSectionHeadService>();
        services.AddScoped<ITechnicalManagerRoutingRuleService,
    TechnicalManagerRoutingRuleService>();
        services.AddScoped<ITestPanelService, TestPanelService>();
        services.AddScoped<IReferenceLimitService, ReferenceLimitService>();
        services.AddScoped<ILimitReferenceService, LimitReferenceService>();
        services.AddScoped<ITestLimitRuleService, TestLimitRuleService>();
        services.AddScoped<ICustomerTestPanelService, CustomerTestPanelService>();
        services.AddScoped<ITestPanelItemService, TestPanelItemService>();
        services.AddScoped<ITestResultSetService, TestResultSetService>();
        services.AddScoped<
                ITechnicalManagerRoutingService,
                TechnicalManagerRoutingService>();
        services.AddScoped<ITestResultSetItemService, TestResultSetItemService>();
        services.AddScoped<
            ITestAssignmentResultValueService,
            TestAssignmentResultValueService>();
        services.AddScoped<
        ITestResultDefinitionService,
        TestResultDefinitionService>();

        services.AddScoped<
            IScopedTemplateResolver,
            ScopedTemplateResolver>();

        services.AddScoped<
            IExcelExportService,
            ExcelExportService>();

        services.AddScoped<IExcelImportService, ExcelImportService>();

        services.AddScoped<
                IReportExportService,
                ReportExportService>();


        return services;
    }
}
