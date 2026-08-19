using Barman.Application.DTOs.Reception;
using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class ReceptionService : IReceptionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INumberGenerator _numberGenerator;

    public ReceptionService(
        IUnitOfWork unitOfWork,
        INumberGenerator numberGenerator)
    {
        _unitOfWork = unitOfWork;
        _numberGenerator = numberGenerator;
    }

    public async Task<Reception> CreateReceptionAsync(
        CreateReceptionDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.CustomerId == Guid.Empty)
            throw new ArgumentException("Customer is required.");

        var validSamples = dto.Samples
            .Where(x => !string.IsNullOrWhiteSpace(x.SampleName))
            .ToList();

        if (validSamples.Count == 0)
            throw new ArgumentException("At least one sample is required.");

        var receptionNumber =
            await _numberGenerator.GenerateAsync(
                "Reception",
                cancellationToken);

        var reception = new Reception
        {
            ReceptionNumber = receptionNumber,
            CustomerId = dto.CustomerId,
            ReceptionDate = DateTime.UtcNow,
            IsUrgent = dto.IsUrgent,
            Description = dto.Description,
            Status = Domain.Enums.ReceptionStatus.Draft,
            IsPaid = false
        };

        await _unitOfWork.Receptions.AddAsync(reception);

        var sampleIndex = 0;

        foreach (var sampleDto in validSamples)
        {
            sampleIndex++;

            var sampleCode =
                $"{receptionNumber}-S{sampleIndex:00}";

            var sample = new Sample
            {
                ReceptionId = reception.Id,

                SampleCode = sampleCode,

                SampleName = sampleDto.SampleName.Trim(),

                SampleCategoryId = sampleDto.SampleCategoryId,

                Matrix = string.IsNullOrWhiteSpace(sampleDto.Matrix)
                    ? null
                    : sampleDto.Matrix.Trim(),

                Quantity = sampleDto.Quantity,

                Unit = string.IsNullOrWhiteSpace(sampleDto.Unit)
                    ? null
                    : sampleDto.Unit.Trim(),

                ContainerType = string.IsNullOrWhiteSpace(sampleDto.ContainerType)
                    ? null
                    : sampleDto.ContainerType.Trim(),

                Description = string.IsNullOrWhiteSpace(sampleDto.Description)
                    ? null
                    : sampleDto.Description.Trim()
            };

            await _unitOfWork.Samples.AddAsync(sample);

            // ایجاد TestAssignment برای تست‌های انتخاب‌شده
            var testIds = sampleDto.TestIds
                .Where(x => x != Guid.Empty)
                .Distinct()
                .ToList();

            foreach (var testId in testIds)
            {
                var test = await _unitOfWork.Tests
                    .GetByIdAsync(testId);

                if (test == null)
                    continue;

                var assignment = new TestAssignment
                {
                    SampleId = sample.Id,
                    TestId = test.Id,
                    DepartmentId = test.DepartmentId,

                    IsApprovedBySection = false,
                    IsApprovedByTechManager = false,
                    IsApprovedByDirector = false
                };

                await _unitOfWork.TestAssignments
                    .AddAsync(assignment);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return reception;
    }

    public async Task<Reception?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        return await _unitOfWork.Receptions
            .GetByIdAsync(id, cancellationToken);
    }

    public async Task<List<Reception>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Receptions
            .GetAllAsync(cancellationToken);
    }
}