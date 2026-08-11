using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Seed;

public static class CustomerSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Customers.AnyAsync())
            return;

        var customers = new List<Customer>
        {
            new Customer
            {
                Code = "C001",
                DisplayName = "شرکت بارمان"
            },
            new Customer
            {
                Code = "C002",
                DisplayName = "دانشگاه مازندران"
            },
            new Customer
            {
                Code = "C003",
                DisplayName = "سازمان غذا و دارو"
            },
            new Customer
            {
                Code = "C004",
                DisplayName = "سازمان دامپزشکی"
            }
        };

        context.Customers.AddRange(customers);

        await context.SaveChangesAsync();
    }
}