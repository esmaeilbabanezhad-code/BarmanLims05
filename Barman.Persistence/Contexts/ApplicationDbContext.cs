using Microsoft.EntityFrameworkCore;

using Barman.Domain.Entities;
using Barman.Domain.Entities.Reporting;

namespace Barman.Persistence.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<OrganizationType> OrganizationTypes
    => Set<OrganizationType>();

    public DbSet<TestTariff> TestTariffs
        => Set<TestTariff>();

    public DbSet<Reception> Receptions => Set<Reception>();

    public DbSet<Sample> Samples => Set<Sample>();

    public DbSet<Test> Tests => Set<Test>();

    public DbSet<TestResultDefinition> TestResultDefinitions
    => Set<TestResultDefinition>();

    public DbSet<TestResultSet> TestResultSets
    => Set<TestResultSet>();

    public DbSet<TestResultSetItem> TestResultSetItems
        => Set<TestResultSetItem>();

    public DbSet<TestAssignment> TestAssignments => Set<TestAssignment>();

    public DbSet<TestAssignmentResultValue> TestAssignmentResultValues
    => Set<TestAssignmentResultValue>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<UserAccount> UserAccounts
    => Set<UserAccount>();

    public DbSet<EmployeeDepartment> EmployeeDepartments
        => Set<EmployeeDepartment>();

    public DbSet<EmployeeRole> EmployeeRoles
        => Set<EmployeeRole>();

    public DbSet<EmployeeTechnicalManagerScope> EmployeeTechnicalManagerScopes
    => Set<EmployeeTechnicalManagerScope>();

    public DbSet<TechnicalManagerScope> TechnicalManagerScopes
    => Set<TechnicalManagerScope>();

    public DbSet<TechnicalManagerScopeDepartment> TechnicalManagerScopeDepartments
    => Set<TechnicalManagerScopeDepartment>();

    public DbSet<TechnicalManagerRoutingRule> TechnicalManagerRoutingRules
    => Set<TechnicalManagerRoutingRule>();
    public DbSet<DepartmentResponsibility> DepartmentResponsibilities
        => Set<DepartmentResponsibility>();

    public DbSet<TechnicalManagerSectionHead> TechnicalManagerSectionHeads
        => Set<TechnicalManagerSectionHead>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<RolePermission> RolePermissions
        => Set<RolePermission>();

    public DbSet<SampleCategory> SampleCategories
        => Set<SampleCategory>();

    public DbSet<Matrix> Matrices
        => Set<Matrix>();

    public DbSet<StandardSample> StandardSamples
    => Set<StandardSample>();

    public DbSet<TestPanel> TestPanels
        => Set<TestPanel>();

    public DbSet<TestPanelItem> TestPanelItems
        => Set<TestPanelItem>();

    public DbSet<CustomerTestPanel> CustomerTestPanels
        => Set<CustomerTestPanel>();


    public DbSet<DefaultTestSet> DefaultTestSets
    => Set<DefaultTestSet>();

    public DbSet<DefaultTestSetItem> DefaultTestSetItems
        => Set<DefaultTestSetItem>();
    public DbSet<TestMethod> TestMethods
        => Set<TestMethod>();

    public DbSet<Instrument> Instruments
        => Set<Instrument>();

    public DbSet<ReferenceLimit> ReferenceLimits
        => Set<ReferenceLimit>();

    public DbSet<LimitReference> LimitReferences
    => Set<LimitReference>();

    public DbSet<TestLimitRule> TestLimitRules
    => Set<TestLimitRule>();


    public DbSet<TestLimitChangeRequest> TestLimitChangeRequests
    => Set<TestLimitChangeRequest>();

    public DbSet<TestResultReview> TestResultReviews
    => Set<TestResultReview>();

    public DbSet<ReceptionCorrectionRequest> ReceptionCorrectionRequests
    => Set<ReceptionCorrectionRequest>();

    public DbSet<CustomFieldDefinition> CustomFieldDefinitions
    => Set<CustomFieldDefinition>();

    public DbSet<CustomFieldValue> CustomFieldValues
        => Set<CustomFieldValue>();
    public DbSet<NumberSequence> NumberSequences
        => Set<NumberSequence>();
    public DbSet<ReportTemplate> ReportTemplates
    => Set<ReportTemplate>();

    public DbSet<ReportTemplateSection> ReportTemplateSections
        => Set<ReportTemplateSection>();

    public DbSet<ReportTemplateField> ReportTemplateFields
        => Set<ReportTemplateField>();

    public DbSet<IssuedReport> IssuedReports
    => Set<IssuedReport>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}
