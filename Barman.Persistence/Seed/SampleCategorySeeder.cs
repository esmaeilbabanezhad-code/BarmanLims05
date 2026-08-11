using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Seed;

public static class SampleCategorySeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var categories = new[]
        {
            new SampleCategory
            {
                Code = "FOOD",
                Name = "مواد غذایی",
                Description = "نمونه‌های مربوط به مواد و محصولات غذایی"
            },

            new SampleCategory
            {
                Code = "FEED",
                Name = "خوراک دام و طیور",
                Description = "نمونه‌های خوراک دام، طیور و آبزیان"
            },

            new SampleCategory
            {
                Code = "AGRICULTURE",
                Name = "محصولات کشاورزی",
                Description = "نمونه‌های محصولات کشاورزی و گیاهی"
            },

            new SampleCategory
            {
                Code = "ENVIRONMENT",
                Name = "محیط زیست",
                Description = "نمونه‌های آب، خاک و سایر نمونه‌های محیط زیستی"
            },

            new SampleCategory
            {
                Code = "SUPPLEMENT",
                Name = "مکمل‌ها",
                Description = "نمونه‌های مکمل‌های غذایی و تغذیه‌ای"
            },

            new SampleCategory
            {
                Code = "COSMETIC",
                Name = "آرایشی و بهداشتی",
                Description = "نمونه‌های محصولات آرایشی و بهداشتی"
            },

            new SampleCategory
            {
                Code = "VETERINARY",
                Name = "دارو و محصولات دامپزشکی",
                Description = "نمونه‌های داروها و فرآورده‌های دامپزشکی"
            }
        };

        foreach (var category in categories)
        {
            var exists = await context.SampleCategories
            .AnyAsync(x =>
            x.Code == category.Code ||
            x.Name == category.Name);

            if (!exists)
            {
                await context.SampleCategories.AddAsync(category);
            }
        }

        await context.SaveChangesAsync();
    }
}