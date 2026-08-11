using Barman.Application.Interfaces.Repositories;

namespace Barman.Application.Interfaces;

public interface IUnitOfWork
{
    IReceptionRepository Receptions { get; }

    INumberSequenceRepository NumberSequences { get; }

    ICustomerRepository Customers { get; }

    ISampleRepository Samples { get; }

    ISampleCategoryRepository SampleCategories { get; }

    IMatrixRepository Matrices { get; }
    ITestRepository Tests { get; }

    IDepartmentRepository Departments { get; }

    ITestAssignmentRepository TestAssignments { get; }

    IEmployeeRepository Employees { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}