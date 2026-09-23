using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Seed;

public static class NumberSequenceSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var definitions = new List<NumberSequence>
        {
            // Existing
            new()
            {
                EntityName = "Reception",
                Prefix = "B",
                Separator = "-",
                UseYear = true,
                PersianYear = false,
                YearDigits = 2,
                SequenceDigits = 5,
                ResetEveryYear = true
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
                ResetEveryYear = true
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
                ResetEveryYear = false
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
                ResetEveryYear = true
            },

            // Master Data
            new()
            {
                EntityName = "Test",
                Prefix = "T",
                Separator = "-",
                UseYear = false,
                SequenceDigits = 6,
                ResetEveryYear = false
            },

            new()
            {
                EntityName = "TestPanel",
                Prefix = "P",
                Separator = "-",
                UseYear = false,
                SequenceDigits = 6,
                ResetEveryYear = false
            },

            new()
            {
                EntityName = "SampleCategory",
                Prefix = "SC",
                Separator = "-",
                UseYear = false,
                SequenceDigits = 6,
                ResetEveryYear = false
            },

            new()
            {
                EntityName = "Matrix",
                Prefix = "M",
                Separator = "-",
                UseYear = false,
                SequenceDigits = 6,
                ResetEveryYear = false
            },

            new()
            {
                EntityName = "StandardSample",
                Prefix = "SS",
                Separator = "-",
                UseYear = false,
                SequenceDigits = 6,
                ResetEveryYear = false
            },

            new()
            {
                EntityName = "ResultDefinition",
                Prefix = "RD",
                Separator = "-",
                UseYear = false,
                SequenceDigits = 6,
                ResetEveryYear = false
            },

            new()
            {
                EntityName = "ResultSet",
                Prefix = "RS",
                Separator = "-",
                UseYear = false,
                SequenceDigits = 6,
                ResetEveryYear = false
            },

            new()
            {
                EntityName = "ResultItem",
                Prefix = "RI",
                Separator = "-",
                UseYear = false,
                SequenceDigits = 6,
                ResetEveryYear = false
            },

            new()
            {
                EntityName = "DefaultTestSet",
                Prefix = "DTS",
                Separator = "-",
                UseYear = false,
                SequenceDigits = 6,
                ResetEveryYear = false
            },

            new()
            {
                EntityName = "LimitReference",
                Prefix = "LR",
                Separator = "-",
                UseYear = false,
                SequenceDigits = 6,
                ResetEveryYear = false
            },

            new()
            {
                EntityName = "Tariff",
                Prefix = "TR",
                Separator = "-",
                UseYear = false,
                SequenceDigits = 6,
                ResetEveryYear = false
            },

            new()
            {
                EntityName = "Employee",
                Prefix = "E",
                Separator = "-",
                UseYear = false,
                SequenceDigits = 6,
                ResetEveryYear = false
            }
        };

        var existingNames =
            await context.NumberSequences
                .Select(x => x.EntityName)
                .ToListAsync();

        var newItems =
            definitions
                .Where(x =>
                    !existingNames.Contains(x.EntityName))
                .ToList();

        if (newItems.Count == 0)
            return;

        context.NumberSequences.AddRange(newItems);

        await context.SaveChangesAsync();
    }
}