using Barman.Application.DTOs.Test;
using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class TestResultSetItemService
    : ITestResultSetItemService
{
    private readonly IUnitOfWork _unitOfWork;

    public TestResultSetItemService(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TestResultSetItemDto>>
        GetByResultSetIdAsync(
            Guid testResultSetId,
            CancellationToken cancellationToken = default)
    {
        if (testResultSetId == Guid.Empty)
            return new List<TestResultSetItemDto>();

        var items =
            await _unitOfWork.TestResultSetItems
                .GetByResultSetIdAsync(
                    testResultSetId,
                    cancellationToken);

        return items
            .OrderBy(x => x.DisplayOrder)
            .Select(x => ToDto(x))
            .ToList();
    }

    public async Task<TestResultSetItemDto>
        AddAsync(
            Guid testResultSetId,
            Guid testResultDefinitionId,
            int displayOrder,
            decimal? lod,
            decimal? loq,
            decimal? minValue,
            decimal? maxValue,
            CancellationToken cancellationToken = default)
    {
        if (testResultSetId == Guid.Empty)
            throw new ArgumentException(
                "TestResultSetId is required.");

        if (testResultDefinitionId == Guid.Empty)
            throw new ArgumentException(
                "TestResultDefinitionId is required.");

        if (displayOrder < 0)
            throw new ArgumentException(
                "DisplayOrder cannot be negative.");

        ValidateLimits(lod, loq, minValue, maxValue);

        var resultSet =
            await _unitOfWork.TestResultSets
                .GetByIdAsync(
                    testResultSetId,
                    cancellationToken);

        if (resultSet == null)
            throw new KeyNotFoundException(
                "Test result set not found.");

        var definition =
            await _unitOfWork.TestResultDefinitions
                .GetByIdAsync(
                    testResultDefinitionId);

        if (definition == null ||
            definition.IsDeleted)
        {
            throw new KeyNotFoundException(
                "Test result definition not found.");
        }

        // Definition must belong to the same Test
        if (definition.TestId != resultSet.TestId)
        {
            throw new InvalidOperationException(
                "The result definition does not belong to the test of this result set.");
        }

        var existingItems =
            await _unitOfWork.TestResultSetItems
                .GetByResultSetIdAsync(
                    testResultSetId,
                    cancellationToken);

        if (existingItems.Any(x =>
            x.TestResultDefinitionId ==
            testResultDefinitionId))
        {
            throw new InvalidOperationException(
                "This result definition is already included in the result set.");
        }

        var entity = new TestResultSetItem
        {
            Id = Guid.NewGuid(),
            TestResultSetId = testResultSetId,
            TestResultDefinitionId = testResultDefinitionId,
            DisplayOrder = displayOrder,

            LOD = lod,
            LOQ = loq,
            MinValue = minValue,
            MaxValue = maxValue,

            IsActive = true,
            IsDeleted = false
        };

        await _unitOfWork.TestResultSetItems
            .AddAsync(
                entity,
                cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ToDto(entity, definition);
    }

    public async Task<TestResultSetItemDto>
        UpdateAsync(
            Guid id,
            int displayOrder,
            decimal? lod,
            decimal? loq,
            decimal? minValue,
            decimal? maxValue,
            CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            throw new ArgumentException(
                "Item id is required.");

        if (displayOrder < 0)
            throw new ArgumentException(
                "DisplayOrder cannot be negative.");

        ValidateLimits(lod, loq, minValue, maxValue);

        var entity =
            await _unitOfWork.TestResultSetItems
                .GetByIdAsync(
                    id,
                    cancellationToken);

        if (entity == null)
            throw new KeyNotFoundException(
                "Test result set item not found.");

        entity.DisplayOrder = displayOrder;

        entity.LOD = lod;
        entity.LOQ = loq;
        entity.MinValue = minValue;
        entity.MaxValue = maxValue;

        _unitOfWork.TestResultSetItems
            .Update(entity);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        var definition =
            await _unitOfWork.TestResultDefinitions
                .GetByIdAsync(
                    entity.TestResultDefinitionId);

        return ToDto(entity, definition);
    }

    public async Task<bool>
        UpdateDisplayOrderAsync(
            Guid id,
            int displayOrder,
            CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return false;

        if (displayOrder < 0)
            throw new ArgumentException(
                "DisplayOrder cannot be negative.");

        var entity =
            await _unitOfWork.TestResultSetItems
                .GetByIdAsync(
                    id,
                    cancellationToken);

        if (entity == null)
            return false;

        entity.DisplayOrder = displayOrder;

        _unitOfWork.TestResultSetItems
            .Update(entity);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool>
        DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return false;

        var entity =
            await _unitOfWork.TestResultSetItems
                .GetByIdAsync(
                    id,
                    cancellationToken);

        if (entity == null)
            return false;

        _unitOfWork.TestResultSetItems
            .Delete(entity);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    private static void ValidateLimits(
        decimal? lod,
        decimal? loq,
        decimal? minValue,
        decimal? maxValue)
    {
        if (lod.HasValue && lod.Value < 0)
            throw new ArgumentException(
                "LOD cannot be negative.");

        if (loq.HasValue && loq.Value < 0)
            throw new ArgumentException(
                "LOQ cannot be negative.");

        if (minValue.HasValue &&
            maxValue.HasValue &&
            minValue.Value > maxValue.Value)
        {
            throw new ArgumentException(
                "MinValue cannot be greater than MaxValue.");
        }
    }

    private static TestResultSetItemDto ToDto(
        TestResultSetItem entity,
        TestResultDefinition? definition = null)
    {
        definition ??= entity.TestResultDefinition;

        return new TestResultSetItemDto
        {
            Id = entity.Id,
            TestResultSetId =
                entity.TestResultSetId,

            TestResultDefinitionId =
                entity.TestResultDefinitionId,

            DisplayOrder =
                entity.DisplayOrder,

            LOD =
                entity.LOD,

            LOQ =
                entity.LOQ,

            MinValue =
                entity.MinValue,

            MaxValue =
                entity.MaxValue,

            Code =
                definition?.Code,

            Name =
                definition?.Name,

            Unit =
                definition?.Unit,

            IsRequired =
                definition?.IsRequired
                ?? false,

            DecimalPlaces =
                definition?.DecimalPlaces
        };
    }
}