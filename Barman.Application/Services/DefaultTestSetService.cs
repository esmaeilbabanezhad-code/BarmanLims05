using Barman.Application.DTOs.DefaultTestSet;
using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class DefaultTestSetService : IDefaultTestSetService
{
    private readonly IUnitOfWork _unitOfWork;

    public DefaultTestSetService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<DefaultTestSetDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.DefaultTestSets
            .GetAllAsync(cancellationToken);

        return entities
            .Select(MapToDto)
            .ToList();
    }

    public async Task<List<DefaultTestSetDto>> GetByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        if (customerId == Guid.Empty)
            return new List<DefaultTestSetDto>();

        var entities = await _unitOfWork.DefaultTestSets
            .GetByCustomerAsync(
                customerId,
                cancellationToken);

        return entities
            .Select(MapToDto)
            .ToList();
    }

    public async Task<DefaultTestSetDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        var entity = await _unitOfWork.DefaultTestSets
            .GetByIdAsync(
                id,
                cancellationToken);

        return entity == null
            ? null
            : MapToDto(entity);
    }

    public async Task<DefaultTestSetDto?> ResolveAsync(
        Guid? customerId,
        Guid? sampleCategoryId,
        Guid? matrixId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.DefaultTestSets
            .ResolveAsync(
                customerId,
                sampleCategoryId,
                matrixId,
                cancellationToken);

        return entity == null
            ? null
            : MapToDto(entity);
    }

    public async Task<DefaultTestSetDto> CreateAsync(
        DefaultTestSetDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Code))
            throw new ArgumentException(
                "کد مجموعه آزمون الزامی است.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException(
                "نام مجموعه آزمون الزامی است.");

        var code = dto.Code.Trim();

        var existing = await _unitOfWork.DefaultTestSets
            .GetAllAsync(cancellationToken);

        if (existing.Any(x =>
            x.Code.Equals(
                code,
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                $"کد «{code}» قبلاً ثبت شده است.");
        }

        var entity = new DefaultTestSet
        {
            Id = Guid.NewGuid(),

            Code = code,

            Name = dto.Name.Trim(),

            Description = dto.Description,

            CustomerId = dto.CustomerId,

            SampleCategoryId = dto.SampleCategoryId,

            MatrixId = dto.MatrixId,

            Priority = dto.Priority,

            IsActive = true,

            IsDeleted = false
        };

        ValidateItems(dto.Items);

        foreach (var itemDto in dto.Items.OrderBy(x => x.SortOrder))
        {
            entity.Items.Add(
                MapToEntity(itemDto));
        }

        await _unitOfWork.DefaultTestSets
            .AddAsync(
                entity,
                cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return MapToDto(entity);
    }

    public async Task<bool> UpdateAsync(
        DefaultTestSetDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Id == Guid.Empty)
            return false;

        var entity = await _unitOfWork.DefaultTestSets
            .GetByIdAsync(
                dto.Id,
                cancellationToken);

        if (entity == null)
            return false;

        if (string.IsNullOrWhiteSpace(dto.Code))
            throw new ArgumentException(
                "کد مجموعه آزمون الزامی است.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException(
                "نام مجموعه آزمون الزامی است.");

        var code = dto.Code.Trim();

        var existing = await _unitOfWork.DefaultTestSets
            .GetAllAsync(cancellationToken);

        if (existing.Any(x =>
            x.Id != dto.Id &&
            x.Code.Equals(
                code,
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                $"کد «{code}» قبلاً ثبت شده است.");
        }

        ValidateItems(dto.Items);

        entity.Code = code;

        entity.Name = dto.Name.Trim();

        entity.Description = dto.Description;

        entity.CustomerId = dto.CustomerId;

        entity.SampleCategoryId = dto.SampleCategoryId;

        entity.MatrixId = dto.MatrixId;

        entity.Priority = dto.Priority;

        entity.Items.Clear();

        foreach (var itemDto in dto.Items.OrderBy(x => x.SortOrder))
        {
            entity.Items.Add(
                MapToEntity(itemDto));
        }

        _unitOfWork.DefaultTestSets.Update(entity);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return false;

        var entity = await _unitOfWork.DefaultTestSets
            .GetByIdAsync(
                id,
                cancellationToken);

        if (entity == null)
            return false;

        entity.IsDeleted = true;

        _unitOfWork.DefaultTestSets.Update(entity);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    // =========================================================
    // Validation
    // =========================================================

    private static void ValidateItems(
        IEnumerable<DefaultTestSetItemDto> items)
    {
        foreach (var item in items)
        {
            if (item.IsPanel)
            {
                if (!item.TestPanelId.HasValue ||
                    item.TestPanelId.Value == Guid.Empty)
                {
                    throw new ArgumentException(
                        "برای آیتم Panel باید TestPanel انتخاب شود.");
                }

                if (item.TestId.HasValue)
                {
                    throw new ArgumentException(
                        "یک آیتم Panel نمی‌تواند همزمان Test نیز داشته باشد.");
                }
            }
            else
            {
                if (!item.TestId.HasValue ||
                    item.TestId.Value == Guid.Empty)
                {
                    throw new ArgumentException(
                        "برای آیتم آزمون باید Test انتخاب شود.");
                }

                if (item.TestPanelId.HasValue)
                {
                    throw new ArgumentException(
                        "یک آزمون مستقل نمی‌تواند همزمان TestPanel داشته باشد.");
                }
            }
        }
    }

    // =========================================================
    // Mapping
    // =========================================================

    private static DefaultTestSetDto MapToDto(
        DefaultTestSet entity)
    {
        return new DefaultTestSetDto
        {
            Id = entity.Id,

            Code = entity.Code,

            Name = entity.Name,

            Description = entity.Description,

            CustomerId = entity.CustomerId,

            SampleCategoryId = entity.SampleCategoryId,

            MatrixId = entity.MatrixId,

            Priority = entity.Priority,

            Items = entity.Items
                .OrderBy(x => x.SortOrder)
                .Select(MapToDto)
                .ToList()
        };
    }

    private static DefaultTestSetItemDto MapToDto(
        DefaultTestSetItem entity)
    {
        return new DefaultTestSetItemDto
        {
            Id = entity.Id,

            DefaultTestSetId =
                entity.DefaultTestSetId,

            IsPanel = entity.IsPanel,

            TestId = entity.TestId,

            TestPanelId = entity.TestPanelId,

            SortOrder = entity.SortOrder,

            TestCode = entity.Test?.Code,

            TestName = entity.Test?.Name,

            TestPanelCode = entity.TestPanel?.Code,

            TestPanelName = entity.TestPanel?.Name
        };
    }

    private static DefaultTestSetItem MapToEntity(
        DefaultTestSetItemDto dto)
    {
        return new DefaultTestSetItem
        {
            Id = dto.Id == Guid.Empty
                ? Guid.NewGuid()
                : dto.Id,

            IsPanel = dto.IsPanel,

            TestId = dto.IsPanel
                ? null
                : dto.TestId,

            TestPanelId = dto.IsPanel
                ? dto.TestPanelId
                : null,

            SortOrder = dto.SortOrder,

            IsActive = true,

            IsDeleted = false
        };
    }
}