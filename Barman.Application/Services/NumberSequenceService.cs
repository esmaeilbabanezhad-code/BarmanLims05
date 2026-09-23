using Barman.Application.Interfaces;
using Barman.Domain.Entities;
using System.Globalization;

namespace Barman.Application.Services;

public class NumberSequenceService : INumberSequenceService
{
    private readonly IUnitOfWork _unitOfWork;

    public NumberSequenceService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<string> GetNextCodeAsync(
        string entityName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(entityName))
            throw new ArgumentException(
                "EntityName الزامی است.",
                nameof(entityName));

        var sequence =
            await _unitOfWork.NumberSequences
                .GetByEntityNameAsync(entityName.Trim());

        if (sequence == null)
        {
            throw new InvalidOperationException(
                $"NumberSequence برای '{entityName}' تعریف نشده است.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        var currentYear =
            sequence.PersianYear
                ? GetPersianYear()
                : DateTime.Now.Year;

        if (sequence.ResetEveryYear &&
            sequence.LastYear != currentYear)
        {
            sequence.LastNumber = 0;
            sequence.LastYear = currentYear;
        }

        sequence.LastNumber++;

        sequence.LastYear = currentYear;

        await _unitOfWork.NumberSequences.SaveChangesAsync();

        return BuildCode(sequence, currentYear);
    }

    private static string BuildCode(
        NumberSequence sequence,
        int currentYear)
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(sequence.Prefix))
            parts.Add(sequence.Prefix.Trim());

        if (sequence.UseYear)
        {
            var yearText =
                currentYear.ToString(
                    CultureInfo.InvariantCulture);

            if (sequence.YearDigits > 0)
                yearText =
                    yearText.PadLeft(
                        sequence.YearDigits,
                        '0');

            if (sequence.YearDigits == 2 &&
                yearText.Length > 2)
            {
                yearText = yearText[^2..];
            }

            parts.Add(yearText);
        }

        var number =
            sequence.LastNumber.ToString(
                CultureInfo.InvariantCulture)
            .PadLeft(
                sequence.SequenceDigits,
                '0');

        parts.Add(number);

        return string.Join(
            sequence.Separator ?? "-",
            parts);
    }

    private static int GetPersianYear()
    {
        var calendar =
            new PersianCalendar();

        return calendar.GetYear(
            DateTime.Now);
    }
}
