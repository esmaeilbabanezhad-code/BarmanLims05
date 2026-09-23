using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Seed;

public static class OrganizationTypeSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var organizationTypes = new[]
        {
            new OrganizationType
            {
                Code = "FOOD_DRUG",
                Name = "غذا و دارو",
                Description = "سازمان غذا و دارو",
                DisplayOrder = 1
            },

            new OrganizationType
            {
                Code = "STANDARD",
                Name = "استاندارد",
                Description = "سازمان ملی استاندارد",
                DisplayOrder = 2
            },

            new OrganizationType
            {
                Code = "VETERINARY",
                Name = "دامپزشکی",
                Description = "سازمان دامپزشکی",
                DisplayOrder = 3
            },

            new OrganizationType
            {
                Code = "ENVIRONMENT",
                Name = "محیط زیست",
                Description = "سازمان حفاظت محیط زیست",
                DisplayOrder = 4
            },

            new OrganizationType
            {
                Code = "AGRICULTURE",
                Name = "جهاد کشاورزی",
                Description = "وزارت جهاد کشاورزی و واحدهای مرتبط",
                DisplayOrder = 5
            },

            new OrganizationType
            {
                Code = "OCCUPATIONAL_HEALTH",
                Name = "بهداشت حرفه‌ای",
                Description = "خدمات و نمونه‌های مرتبط با بهداشت حرفه‌ای",
                DisplayOrder = 6
            },

            new OrganizationType
            {
                Code = "PRIVATE",
                Name = "مشتری خصوصی",
                Description = "مشتریان و شرکت‌های خصوصی",
                DisplayOrder = 7
            },

            new OrganizationType
            {
                Code = "STUDENT",
                Name = "دانشجویی",
                Description = "نمونه‌ها و خدمات دانشجویی",
                DisplayOrder = 8
            },

            new OrganizationType
            {
                Code = "OTHER",
                Name = "سایر",
                Description = "سایر سازمان‌ها و مراجع",
                DisplayOrder = 9
            }
        };

        foreach (var organizationType in organizationTypes)
        {
            var exists = await context.OrganizationTypes
                .AnyAsync(x =>
                    x.Code == organizationType.Code ||
                    x.Name == organizationType.Name);

            if (!exists)
            {
                await context.OrganizationTypes.AddAsync(organizationType);
            }
        }

        await context.SaveChangesAsync();
    }
}