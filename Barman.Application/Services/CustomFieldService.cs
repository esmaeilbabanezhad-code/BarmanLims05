using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class CustomFieldService : ICustomFieldService
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomFieldService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CustomFieldDefinition>> GetDefinitionsAsync(
        Guid? customerId,
        Guid? sampleCategoryId,
        Guid? matrixId)
    {
        return await _unitOfWork.CustomFieldDefinitions
            .GetActiveAsync(
                customerId,
                sampleCategoryId,
                matrixId);
    }

    public async Task<List<CustomFieldDefinition>> GetAllDefinitionsAsync()
    {
        return await _unitOfWork.CustomFieldDefinitions
            .GetAllAsync();
    }

    public async Task<CustomFieldDefinition?> GetDefinitionByIdAsync(
        Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _unitOfWork.CustomFieldDefinitions
            .GetByIdAsync(id);
    }

    public async Task<CustomFieldDefinition> CreateDefinitionAsync(
        CustomFieldDefinition definition)
    {
        if (definition == null)
            throw new ArgumentNullException(nameof(definition));

        if (string.IsNullOrWhiteSpace(definition.Title))
            throw new ArgumentException(
                "عنوان فیلد الزامی است.");

        definition.Title = definition.Title.Trim();

        if (string.IsNullOrWhiteSpace(definition.DataType))
            definition.DataType = "Text";

        definition.DataType = definition.DataType.Trim();

        await _unitOfWork.CustomFieldDefinitions
            .AddAsync(definition);

        await _unitOfWork.SaveChangesAsync();

        return definition;
    }

    public async Task<CustomFieldDefinition?> UpdateDefinitionAsync(
        CustomFieldDefinition definition)
    {
        if (definition == null)
            throw new ArgumentNullException(nameof(definition));

        if (definition.Id == Guid.Empty)
            return null;

        if (string.IsNullOrWhiteSpace(definition.Title))
            throw new ArgumentException(
                "عنوان فیلد الزامی است.");

        var existing =
            await _unitOfWork.CustomFieldDefinitions
                .GetByIdAsync(definition.Id);

        if (existing == null || existing.IsDeleted)
            return null;

        existing.Title = definition.Title.Trim();

        existing.DataType =
            string.IsNullOrWhiteSpace(definition.DataType)
                ? "Text"
                : definition.DataType.Trim();

        existing.IsRequired = definition.IsRequired;
        existing.IsActive = definition.IsActive;
        existing.DisplayOrder = definition.DisplayOrder;

        existing.CustomerId = definition.CustomerId;
        existing.SampleCategoryId = definition.SampleCategoryId;
        existing.MatrixId = definition.MatrixId;

        _unitOfWork.CustomFieldDefinitions
            .Update(existing);

        await _unitOfWork.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> DeleteDefinitionAsync(Guid id)
    {
        if (id == Guid.Empty)
            return false;

        var definition =
            await _unitOfWork.CustomFieldDefinitions
                .GetByIdAsync(id);

        if (definition == null || definition.IsDeleted)
            return false;

        definition.IsDeleted = true;
        definition.IsActive = false;

        _unitOfWork.CustomFieldDefinitions
            .Update(definition);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<List<CustomFieldValue>> GetValuesBySampleIdAsync(
        Guid sampleId)
    {
        if (sampleId == Guid.Empty)
            return new List<CustomFieldValue>();

        return await _unitOfWork.CustomFieldValues
            .GetBySampleIdAsync(sampleId);
    }

    public async Task<CustomFieldValue?> GetValueAsync(
        Guid sampleId,
        Guid customFieldDefinitionId)
    {
        if (sampleId == Guid.Empty ||
            customFieldDefinitionId == Guid.Empty)
        {
            return null;
        }

        return await _unitOfWork.CustomFieldValues
            .GetBySampleAndDefinitionIdAsync(
                sampleId,
                customFieldDefinitionId);
    }

    public async Task<CustomFieldValue> SaveValueAsync(
        CustomFieldValue value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (value.SampleId == Guid.Empty)
            throw new ArgumentException(
                "Sample الزامی است.");

        if (value.CustomFieldDefinitionId == Guid.Empty)
            throw new ArgumentException(
                "Custom Field Definition الزامی است.");

        var existing =
            await _unitOfWork.CustomFieldValues
                .GetBySampleAndDefinitionIdAsync(
                    value.SampleId,
                    value.CustomFieldDefinitionId);

        if (existing == null)
        {
            value.Value = string.IsNullOrWhiteSpace(value.Value)
                ? null
                : value.Value.Trim();

            await _unitOfWork.CustomFieldValues
                .AddAsync(value);
        }
        else
        {
            existing.Value = string.IsNullOrWhiteSpace(value.Value)
                ? null
                : value.Value.Trim();

            _unitOfWork.CustomFieldValues
                .Update(existing);

            value = existing;
        }

        await _unitOfWork.SaveChangesAsync();

        return value;
    }

    public async Task<bool> DeleteValueAsync(Guid id)
    {
        if (id == Guid.Empty)
            return false;

        var value =
            await _unitOfWork.CustomFieldValues
                .GetByIdAsync(id);

        if (value == null || value.IsDeleted)
            return false;

        value.IsDeleted = true;
        value.IsActive = false;

        _unitOfWork.CustomFieldValues
            .Update(value);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}