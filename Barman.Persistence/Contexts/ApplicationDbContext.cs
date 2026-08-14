using Microsoft.EntityFrameworkCore;

using Barman.Domain.Entities;

namespace Barman.Persistence.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Reception> Receptions => Set<Reception>();

    public DbSet<Sample> Samples => Set<Sample>();

    public DbSet<Test> Tests => Set<Test>();

    public DbSet<TestAssignment> TestAssignments => Set<TestAssignment>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<SampleCategory> SampleCategories => Set<SampleCategory>();

    public DbSet<Matrix> Matrices => Set<Matrix>();

    public DbSet<TestPanel> TestPanels => Set<TestPanel>();

    public DbSet<TestPanelItem> TestPanelItems => Set<TestPanelItem>();

    public DbSet<CustomerTestPanel> CustomerTestPanels => Set<CustomerTestPanel>();

    public DbSet<TestMethod> TestMethods => Set<TestMethod>();

    public DbSet<Instrument> Instruments => Set<Instrument>();

    public DbSet<ReferenceLimit> ReferenceLimits => Set<ReferenceLimit>();

   
    public DbSet<NumberSequence> NumberSequences
    => Set<NumberSequence>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}