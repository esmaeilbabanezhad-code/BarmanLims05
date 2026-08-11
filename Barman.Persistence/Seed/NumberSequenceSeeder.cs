using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Seed;

public static class NumberSequenceSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.NumberSequences.AnyAsync())
            return;

        var items = new List<NumberSequence>
        {
            new()
            {
                EntityName = "Reception",
                Prefix = "B",
                Separator = "-",
                UseYear = true,
                PersianYear = false,
                YearDigits = 2,
                SequenceDigits = 5,
                ResetEveryYear = true,
                LastNumber = 0,
                LastYear = 0
            },

            new()
            {
                EntityName = "Sample",
                Prefix = "S",
                Separator = "-",
                UseYear = true,
                PersianYear = false,
                YearDigits = 2,
                SequenceDigits = 6,
                ResetEveryYear = true,
                LastNumber = 0,
                LastYear = 0
            },

            new()
            {
                EntityName = "Customer",
                Prefix = "C",
                Separator = "-",
                UseYear = false,
                PersianYear = false,
                YearDigits = 0,
                SequenceDigits = 5,
                ResetEveryYear = false,
                LastNumber = 0,
                LastYear = 0
            },

            new()
            {
                EntityName = "Report",
                Prefix = "R",
                Separator = "-",
                UseYear = true,
                PersianYear = false,
                YearDigits = 2,
                SequenceDigits = 5,
                ResetEveryYear = true,
                LastNumber = 0,
                LastYear = 0
            }
        };

        context.NumberSequences.AddRange(items);

        await context.SaveChangesAsync();
    }
}