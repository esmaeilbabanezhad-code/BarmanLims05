using Barman.Application.Interfaces;
using Barman.Application.Interfaces.Repositories;

namespace Barman.Infrastructure.Services;

public class NumberGenerator : INumberGenerator
{
    private readonly INumberSequenceRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public NumberGenerator(
        INumberSequenceRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> GenerateAsync(
        string entityName,
        CancellationToken cancellationToken = default)
    {
        var sequence = await _repository.GetByEntityNameAsync(entityName);

        if (sequence is null)
            throw new Exception($"Number sequence for '{entityName}' not found.");

        var currentYear = sequence.PersianYear
            ? DateTime.Now.Year - 621
            : DateTime.Now.Year;

        if (sequence.ResetEveryYear &&
            sequence.LastYear != currentYear)
        {
            sequence.LastYear = currentYear;
            sequence.LastNumber = 0;
        }

        sequence.LastNumber++;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var yearText = sequence.UseYear
            ? (currentYear % 100)
                .ToString()
                .PadLeft(sequence.YearDigits, '0')
            : "";

        var numberText = sequence.LastNumber
            .ToString()
            .PadLeft(sequence.SequenceDigits, '0');

        if (sequence.UseYear)
        {
            return $"{sequence.Prefix}{sequence.Separator}{yearText}{sequence.Separator}{numberText}";
        }

        return $"{sequence.Prefix}{sequence.Separator}{numberText}";
    }
}