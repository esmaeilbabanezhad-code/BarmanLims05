using Barman.Application.DTOs.Test;
using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class TestResultDefinitionService
    : ITestResultDefinitionService
{
    private readonly IUnitOfWork _unitOfWork;

    public TestResultDefinitionService(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TestResultDefinitionDto>>
        GetByTestIdAsync(Guid testId)
    {
        if (testId == Guid.Empty)
            return new List<TestResultDefinitionDto>();

        var items =
            await _unitOfWork.TestResultDefinitions
                .GetByTestIdAsync(testId);

        return items
            .OrderBy(x => x.DisplayOrder)
            .Select(ToDto)
            .ToList();
    }

    public async Task<TestResultDefinitionDto?>
        GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        var entity =
            await _unitOfWork.TestResultDefinitions
                .GetByIdAsync(id);

        return entity == null
            ? null
            : ToDto(entity);
    }

    public async Task<TestResultDefinitionDto>
        CreateAsync(TestResultDefinitionDto dto)
    {
        if (dto.TestId == Guid.Empty)
            throw new ArgumentException(
                "TestId is required.");

        if (string.IsNullOrWhiteSpace(dto.Code))
            throw new ArgumentException(
                "Result definition code is required.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException(
                "Result definition name is required.");

        if (dto.DisplayOrder < 0)
            throw new ArgumentException(
                "DisplayOrder cannot be negative.");

        if (dto.DecimalPlaces < 0)
            throw new ArgumentException(
                "DecimalPlaces cannot be negative.");

        var test =
            await _unitOfWork.Tests
                .GetByIdAsync(dto.TestId);

        if (test == null || test.IsDeleted)
            throw new KeyNotFoundException(
                "Test not found.");

        var existing =
            await _unitOfWork.TestResultDefinitions
                .GetByTestIdAsync(dto.TestId);

        if (existing.Any(x =>
            x.Code.Equals(
                dto.Code.Trim(),
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "A result definition with this code already exists for this test.");
        }

        var entity = new TestResultDefinition
        {
            Id = Guid.NewGuid(),
            TestId = dto.TestId,
            Code = dto.Code.Trim(),
            Name = dto.Name.Trim(),
            Unit = string.IsNullOrWhiteSpace(dto.Unit)
                ? null
                : dto.Unit.Trim(),
            DisplayOrder = dto.DisplayOrder,
            IsRequired = dto.IsRequired,
            DecimalPlaces = dto.DecimalPlaces,
            Description = string.IsNullOrWhiteSpace(dto.Description)
                ? null
                : dto.Description.Trim(),
            IsActive = true,
            IsDeleted = false
        };

        await _unitOfWork.TestResultDefinitions
            .AddAsync(entity);

        await _unitOfWork.SaveChangesAsync();

        return ToDto(entity);
    }

    public async Task<TestResultDefinitionDto>
        UpdateAsync(TestResultDefinitionDto dto)
    {
        if (dto.Id == Guid.Empty)
            throw new ArgumentException(
                "Result definition Id is required.");

        if (dto.TestId == Guid.Empty)
            throw new ArgumentException(
                "TestId is required.");

        if (string.IsNullOrWhiteSpace(dto.Code))
            throw new ArgumentException(
                "Result definition code is required.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException(
                "Result definition name is required.");

        if (dto.DisplayOrder < 0)
            throw new ArgumentException(
                "DisplayOrder cannot be negative.");

        if (dto.DecimalPlaces < 0)
            throw new ArgumentException(
                "DecimalPlaces cannot be negative.");

        var entity =
            await _unitOfWork.TestResultDefinitions
                .GetByIdAsync(dto.Id);

        if (entity == null)
            throw new KeyNotFoundException(
                "Result definition not found.");

        var existing =
            await _unitOfWork.TestResultDefinitions
                .GetByTestIdAsync(dto.TestId);

        if (existing.Any(x =>
            x.Id != dto.Id &&
            x.Code.Equals(
                dto.Code.Trim(),
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "A result definition with this code already exists for this test.");
        }

        entity.TestId = dto.TestId;
        entity.Code = dto.Code.Trim();
        entity.Name = dto.Name.Trim();
        entity.Unit = string.IsNullOrWhiteSpace(dto.Unit)
            ? null
            : dto.Unit.Trim();
        entity.DisplayOrder = dto.DisplayOrder;
        entity.IsRequired = dto.IsRequired;
        entity.DecimalPlaces = dto.DecimalPlaces;
        entity.Description = string.IsNullOrWhiteSpace(dto.Description)
            ? null
            : dto.Description.Trim();

        _unitOfWork.TestResultDefinitions
            .Update(entity);

        await _unitOfWork.SaveChangesAsync();

        return ToDto(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            return;

        var entity =
            await _unitOfWork.TestResultDefinitions
                .GetByIdAsync(id);

        if (entity == null)
            return;

        _unitOfWork.TestResultDefinitions
            .Delete(entity);

        await _unitOfWork.SaveChangesAsync();
    }

    private static TestResultDefinitionDto ToDto(
        TestResultDefinition entity)
    {
        return new TestResultDefinitionDto
        {
            Id = entity.Id,
            TestId = entity.TestId,
            Code = entity.Code,
            Name = entity.Name,
            Unit = entity.Unit,
            DisplayOrder = entity.DisplayOrder,
            IsRequired = entity.IsRequired,
            DecimalPlaces = entity.DecimalPlaces,
            Description = entity.Description
        };
    }
}