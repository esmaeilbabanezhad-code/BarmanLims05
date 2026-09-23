using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Seed;

public static class ApplicationDbInitializer
{
    public static async Task InitializeAsync(
        ApplicationDbContext context)
    {
        await PermissionSeeder.SeedAsync(context);
        await RolePermissionSeeder.SeedAsync(context);
        await NumberSequenceSeeder.SeedAsync(context);
        await CustomerSeeder.SeedAsync(context);
        await SampleCategorySeeder.SeedAsync(context);
        await OrganizationTypeSeeder.SeedAsync(context);
        await ReportTemplateSeeder.SeedAsync(context);
    }
}