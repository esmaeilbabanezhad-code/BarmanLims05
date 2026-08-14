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

    public IDepartmentRepository Departments { get; }

    public IEmployeeRepository Employees { get; }

    public ITestAssignmentRepository TestAssignments { get; }

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

        Departments = new DepartmentRepository(context);

        TestAssignments = new TestAssignmentRepository(context);

        Employees = new EmployeeRepository(context);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}