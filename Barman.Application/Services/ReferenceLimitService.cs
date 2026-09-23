using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class ReferenceLimitService : IReferenceLimitService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReferenceLimitService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ReferenceLimit>> GetByTestIdAsync(
        Guid testId,
        CancellationToken cancellationToken = default)
    {
        if (testId == Guid.Empty)
            return new List<ReferenceLimit>();

        return await _unitOfWork.ReferenceLimits
            .GetByTestIdAsync(testId);
    }

    public async Task<ReferenceLimit?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        return await _unitOfWork.ReferenceLimits
            .GetByIdAsync(id);
    }

    public async Task<ReferenceLimit> CreateAsync(
        ReferenceLimit referenceLimit,
        CancellationToken cancellationToken = default)
    {
        if (referenceLimit.TestId == Guid.Empty)
            throw new ArgumentException(
                "آزمون الزامی است.");

        referenceLimit.Id = Guid.NewGuid();

        await _unitOfWork.ReferenceLimits
            .AddAsync(referenceLimit);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return referenceLimit;
    }

    public async Task<ReferenceLimit?> UpdateAsync(
        ReferenceLimit referenceLimit,
        CancellationToken cancellationToken = default)
    {
        if (referenceLimit.Id == Guid.Empty)
            return null;

        var current =
            await _unitOfWork.ReferenceLimits
                .GetByIdAsync(referenceLimit.Id);

        if (current == null)
            return null;

        current.MatrixId = referenceLimit.MatrixId;
        current.ProductName = referenceLimit.ProductName;
        current.OrganizationName =
            referenceLimit.OrganizationName;

        current.MinValue = referenceLimit.MinValue;
        current.MaxValue = referenceLimit.MaxValue;

        current.WarningLow = referenceLimit.WarningLow;
        current.WarningHigh = referenceLimit.WarningHigh;

        current.Unit = referenceLimit.Unit;
        current.Priority = referenceLimit.Priority;
        current.IsTemporary = referenceLimit.IsTemporary;

        current.ValidFrom = referenceLimit.ValidFrom;
        current.ValidTo = referenceLimit.ValidTo;

        current.Description =
            referenceLimit.Description;

        _unitOfWork.ReferenceLimits.Update(current);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return current;
    }
    public async Task<bool> DeleteAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return false;

        var referenceLimit =
            await _unitOfWork.ReferenceLimits
                .GetByIdAsync(id);

        if (referenceLimit is null)
            return false;

        _unitOfWork.ReferenceLimits
            .Delete(referenceLimit);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}