using Barman.Application.DTOs.Test;
using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class TestResultSetService
    : ITestResultSetService
{
    private readonly IUnitOfWork _unitOfWork;

    public TestResultSetService(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TestResultSetDto>>
        GetByTestIdAsync(
            Guid testId,
            CancellationToken cancellationToken = default)
    {
        if (testId == Guid.Empty)
            return new List<TestResultSetDto>();

        var items =
            await _unitOfWork.TestResultSets
                .GetByTestIdAsync(
                    testId,
                    cancellationToken);

        return items
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.Code)
            .Select(ToDto)
            .ToList();
    }

    public async Task<TestResultSetDto?>
        GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        var entity =
            await _unitOfWork.TestResultSets
                .GetByIdAsync(
                    id,
                    cancellationToken);

        return entity == null
            ? null
            : ToDto(entity);
    }

    public async Task<TestResultSetDto>
        CreateAsync(
            TestResultSetDto dto,
            CancellationToken cancellationToken = default)
    {
        Validate(dto);

        var test =
            await _unitOfWork.Tests
                .GetByIdAsync(dto.TestId);

        if (test == null || test.IsDeleted)
            throw new KeyNotFoundException(
                "Test not found.");

        var entity = new TestResultSet
        {
            Id = Guid.NewGuid(),
            TestId = dto.TestId,

            CustomerId = dto.CustomerId,
            SampleCategoryId = dto.SampleCategoryId,
            MatrixId = dto.MatrixId,
            StandardSampleId = dto.StandardSampleId,

            Code = dto.Code.Trim(),
            Name = dto.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description)
                ? null
                : dto.Description.Trim(),

            Priority = dto.Priority,
            IsActive = dto.IsActive,
            IsDeleted = false
        };

        await _unitOfWork.TestResultSets
            .AddAsync(
                entity,
                cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ToDto(entity);
    }

    public async Task<TestResultSetDto>
        UpdateAsync(
            TestResultSetDto dto,
            CancellationToken cancellationToken = default)
    {
        if (dto.Id == Guid.Empty)
            throw new ArgumentException(
                "Result set Id is required.");

        Validate(dto);

        var entity =
            await _unitOfWork.TestResultSets
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

        if (entity == null)
            throw new KeyNotFoundException(
                "Result set not found.");

        var test =
            await _unitOfWork.Tests
                .GetByIdAsync(dto.TestId);

        if (test == null || test.IsDeleted)
            throw new KeyNotFoundException(
                "Test not found.");

        entity.TestId = dto.TestId;

        entity.CustomerId = dto.CustomerId;
        entity.SampleCategoryId = dto.SampleCategoryId;
        entity.MatrixId = dto.MatrixId;
        entity.StandardSampleId = dto.StandardSampleId;

        entity.Code = dto.Code.Trim();
        entity.Name = dto.Name.Trim();

        entity.Description =
            string.IsNullOrWhiteSpace(dto.Description)
                ? null
                : dto.Description.Trim();

        entity.Priority = dto.Priority;
        entity.IsActive = dto.IsActive;

        _unitOfWork.TestResultSets
            .Update(entity);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ToDto(entity);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return;

        var entity =
            await _unitOfWork.TestResultSets
                .GetByIdAsync(
                    id,
                    cancellationToken);

        if (entity == null)
            return;

        _unitOfWork.TestResultSets
            .Delete(entity);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    private static void Validate(
        TestResultSetDto dto)
    {
        if (dto.TestId == Guid.Empty)
            throw new ArgumentException(
                "TestId is required.");

        if (string.IsNullOrWhiteSpace(dto.Code))
            throw new ArgumentException(
                "Result set code is required.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException(
                "Result set name is required.");

        if (dto.Priority < 0)
            throw new ArgumentException(
                "Priority cannot be negative.");
    }

    private static TestResultSetDto ToDto(
        TestResultSet entity)
    {
        return new TestResultSetDto
        {
            Id = entity.Id,
            TestId = entity.TestId,

            CustomerId = entity.CustomerId,
            SampleCategoryId = entity.SampleCategoryId,
            MatrixId = entity.MatrixId,
            StandardSampleId = entity.StandardSampleId,

            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description,

            Priority = entity.Priority,
            IsActive = entity.IsActive
        };
    }
}