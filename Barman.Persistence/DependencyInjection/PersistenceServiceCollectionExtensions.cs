using Barman.Application.Interfaces;
using Barman.Application.Interfaces.Repositories;
using Barman.Application.Interfaces.Repositories.Reporting;
using Barman.Application.Interfaces.Services;
using Barman.Application.Interfaces.Services.Reporting;
using Barman.Application.Services;
using Barman.Application.Services.Reporting;
using Barman.Persistence.Contexts;
using Barman.Persistence.Repositories;
using Barman.Persistence.Repositories.Reporting;
using Barman.Persistence.Seed;
using Barman.Persistence.Services;
using Barman.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;




namespace Barman.Persistence.DependencyInjection;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<INumberSequenceRepository, NumberSequenceRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUnitOfWork, ApplicationUnitOfWork>();
        services.AddScoped<IReportTemplateRepository, ReportTemplateRepository>();
        services.AddScoped<
            IReportTemplateSectionRepository,
            ReportTemplateSectionRepository>();

        services.AddScoped<
            IReportTemplateFieldRepository,
            ReportTemplateFieldRepository>();

        services.AddScoped<
    IReportTemplateRenderer,
    ReportTemplateRenderer>();



        services.AddScoped<ISampleCategoryService, SampleCategoryService>();
        services.AddScoped<IMatrixService, MatrixService>();
        services.AddScoped<IOrganizationTypeService, OrganizationTypeService>();
        services.AddScoped<ITestTariffService, TestTariffService>();
        services.AddScoped<IStandardSampleRepository, StandardSampleRepository>();
        services.AddScoped<IStandardSampleService, StandardSampleService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IDefaultTestSetService, DefaultTestSetService>();
        services.AddScoped<IReportTemplateService, ReportTemplateService>();
        services.AddScoped<
                IReportTemplateSectionService,
                ReportTemplateSectionService>();

        services.AddScoped<
                IReportTemplateFieldService,
                ReportTemplateFieldService>();

        services.AddScoped<
    IFinalReportDataService,
    FinalReportDataService>();

        services.AddScoped<
    IIssuedReportService,
    IssuedReportService>();

        services.AddScoped<
            IReportNumberGenerator,
            ReportNumberGenerator>();

        services.AddScoped<
            IReportFieldResolver,
            ReportFieldResolver>();

        services.AddScoped<
           ITestLimitChangeRequestRepository,
           TestLimitChangeRequestRepository>();

        services.AddScoped<IApplicationDatabaseUpgradeService, ApplicationDatabaseUpgradeService>();
        services.AddScoped<IDatabaseBackupService, PostgreSQLDatabaseBackupService>();
        services.AddSingleton<DatabaseBackupSettingsService>();
        services.AddHostedService<DatabaseBackupScheduler>();
        services.AddScoped<IDatabaseRestoreService, PostgreSQLDatabaseRestoreService>();
        services.AddScoped<IDatabaseRestoreRequestService, DatabaseRestoreRequestService>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context =
            scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

        await PermissionSeeder.SeedAsync(context);

        await NumberSequenceSeeder.SeedAsync(context);

        await ReportTemplateSeeder.SeedAsync(context);
    }
}



