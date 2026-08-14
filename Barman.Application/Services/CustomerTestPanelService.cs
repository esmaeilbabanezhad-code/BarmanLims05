using Barman.Application.DTOs.Customer;
using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class CustomerTestPanelService : ICustomerTestPanelService
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerTestPanelService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CustomerTestPanelDto>> GetByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        if (customerId == Guid.Empty)
            return new List<CustomerTestPanelDto>();

        var rules = await _unitOfWork.CustomerTestPanels
            .GetByCustomerAsync(customerId, cancellationToken);

        return rules
            .Select(MapToDto)
            .ToList();
    }

    public async Task<CustomerTestPanelDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        var rule = await _unitOfWork.CustomerTestPanels
            .GetByIdAsync(id, cancellationToken);

        return rule == null
            ? null
            : MapToDto(rule);
    }

    public async Task<CustomerTestPanelDto?> ResolveAsync(
    Guid customerId,
    Guid? sampleCategoryId,
    Guid? matrixId,
    CancellationToken cancellationToken = default)
    {
        if (customerId == Guid.Empty)
            return null;

        var rule = await _unitOfWork.CustomerTestPanels
            .ResolveAsync(
                customerId,
                sampleCategoryId,
                matrixId,
                cancellationToken);

        return rule == null
            ? null
            : MapToDto(rule);
    }
    public async Task<CustomerTestPanelDto> CreateAsync(
        CustomerTestPanelDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.CustomerId == Guid.Empty)
            throw new ArgumentException(
                "CustomerId is required.");

        if (dto.TestPanelId == Guid.Empty)
            throw new ArgumentException(
                "TestPanelId is required.");

        var entity = new CustomerTestPanel
        {
            Id = Guid.NewGuid(),

            CustomerId = dto.CustomerId,

            SampleCategoryId = dto.SampleCategoryId,

            MatrixId = dto.MatrixId,

            TestPanelId = dto.TestPanelId,

            Priority = dto.Priority,

            Description = dto.Description,

            IsActive = true,

            IsDeleted = false
        };

        await _unitOfWork.CustomerTestPanels
            .AddAsync(entity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<bool> UpdateAsync(
        CustomerTestPanelDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Id == Guid.Empty)
            return false;

        var entity = await _unitOfWork.CustomerTestPanels
            .GetByIdAsync(dto.Id, cancellationToken);

        if (entity == null)
            return false;

        entity.CustomerId = dto.CustomerId;

        entity.SampleCategoryId = dto.SampleCategoryId;

        entity.MatrixId = dto.MatrixId;

        entity.TestPanelId = dto.TestPanelId;

        entity.Priority = dto.Priority;

        entity.Description = dto.Description;

        _unitOfWork.CustomerTestPanels.Update(entity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return false;

        var entity = await _unitOfWork.CustomerTestPanels
            .GetByIdAsync(id, cancellationToken);

        if (entity == null)
            return false;

        _unitOfWork.CustomerTestPanels.Delete(entity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static CustomerTestPanelDto MapToDto(
        CustomerTestPanel entity)
    {
        return new CustomerTestPanelDto
        {
            Id = entity.Id,

            CustomerId = entity.CustomerId,

            SampleCategoryId = entity.SampleCategoryId,

            MatrixId = entity.MatrixId,

            TestPanelId = entity.TestPanelId,

            Priority = entity.Priority,

            Description = entity.Description
        };
    }
}
