using Barman.Application.Interfaces;
using Barman.Application.Services;
using Barman.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Barman.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<INumberGenerator, NumberGenerator>();
        services.AddScoped<IReceptionService, ReceptionService>();
        services.AddScoped<ISampleCategoryService, SampleCategoryService>();
        services.AddScoped<IMatrixService, MatrixService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ITestService, TestService>();
        services.AddScoped<TestAssignmentService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<ITestPanelService, TestPanelService>();
        services.AddScoped<ICustomerTestPanelService, CustomerTestPanelService>();
        services.AddScoped<ITestPanelItemService, TestPanelItemService>();

        return services;
    }
}
