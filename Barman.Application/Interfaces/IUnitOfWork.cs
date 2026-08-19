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

    ITestPanelRepository TestPanels { get; }

    ITestPanelItemRepository TestPanelItems { get; }

    ICustomerTestPanelRepository CustomerTestPanels { get; }

    IDepartmentRepository Departments { get; }

    ITestAssignmentRepository TestAssignments { get; }

    IEmployeeRepository Employees { get; }

    IEmployeeDepartmentRepository EmployeeDepartments { get; }

    IEmployeeRoleRepository EmployeeRoles { get; }

    IDepartmentResponsibilityRepository DepartmentResponsibilities { get; }

    ITechnicalManagerSectionHeadRepository TechnicalManagerSectionHeads { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
