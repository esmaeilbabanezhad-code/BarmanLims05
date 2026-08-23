using Barman.Application.Interfaces;
using Barman.Application.Interfaces.Repositories;
using Barman.Persistence.Contexts;
using Barman.Persistence.Repositories;

namespace Barman.Persistence.UnitOfWork;

public class ApplicationUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IReceptionRepository Receptions { get; }

    public INumberSequenceRepository NumberSequences { get; }

    public ICustomerRepository Customers { get; }

    public ISampleRepository Samples { get; }

    public ISampleCategoryRepository SampleCategories { get; }

    public IMatrixRepository Matrices { get; }

    public ITestRepository Tests { get; }

    public ITestPanelRepository TestPanels { get; }

    public ITestPanelItemRepository TestPanelItems { get; }

    public ICustomerTestPanelRepository CustomerTestPanels { get; }

    public IDefaultTestSetRepository DefaultTestSets { get; }

    public IDepartmentRepository Departments { get; }

    public IRoleRepository Roles { get; }

    public IRolePermissionRepository RolePermissions { get; }

    public IPermissionRepository Permissions { get; }

    public ITestAssignmentRepository TestAssignments { get; }

    public IEmployeeRepository Employees { get; }

    public IEmployeeDepartmentRepository EmployeeDepartments { get; }

    public IEmployeeRoleRepository EmployeeRoles { get; }

    public IDepartmentResponsibilityRepository DepartmentResponsibilities { get; }

    public ITechnicalManagerSectionHeadRepository TechnicalManagerSectionHeads { get; }

    public ApplicationUnitOfWork(ApplicationDbContext context)
    {
        _context = context;

        Receptions = new ReceptionRepository(context);

        NumberSequences = new NumberSequenceRepository(context);

        Customers = new CustomerRepository(context);

        Samples = new SampleRepository(context);

        SampleCategories = new SampleCategoryRepository(context);

        Matrices = new MatrixRepository(context);

        Tests = new TestRepository(context);

        TestPanels = new TestPanelRepository(context);

        TestPanelItems = new TestPanelItemRepository(context);

        CustomerTestPanels = new CustomerTestPanelRepository(context);

        DefaultTestSets = new DefaultTestSetRepository(context);

        Departments = new DepartmentRepository(context);

        Roles = new RoleRepository(context);

        RolePermissions = new RolePermissionRepository(context);

        Permissions = new PermissionRepository(context);

        Employees = new EmployeeRepository(context);

        EmployeeDepartments = new EmployeeDepartmentRepository(context);

        EmployeeRoles = new EmployeeRoleRepository(context);

        TestAssignments = new TestAssignmentRepository(context);

        DepartmentResponsibilities =
            new DepartmentResponsibilityRepository(context);

        TechnicalManagerSectionHeads =
            new TechnicalManagerSectionHeadRepository(context);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}