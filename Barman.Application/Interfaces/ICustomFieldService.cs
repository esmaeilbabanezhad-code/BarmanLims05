using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface ICustomFieldService
{
    Task<List<CustomFieldDefinition>> GetDefinitionsAsync(
        Guid? customerId,
        Guid? sampleCategoryId,
        Guid? matrixId);

    Task<List<CustomFieldDefinition>> GetAllDefinitionsAsync();

    Task<CustomFieldDefinition?> GetDefinitionByIdAsync(Guid id);

    Task<CustomFieldDefinition> CreateDefinitionAsync(
        CustomFieldDefinition definition);

    Task<CustomFieldDefinition?> UpdateDefinitionAsync(
        CustomFieldDefinition definition);

    Task<bool> DeleteDefinitionAsync(Guid id);

    Task<List<CustomFieldValue>> GetValuesBySampleIdAsync(
        Guid sampleId);

    Task<CustomFieldValue?> GetValueAsync(
        Guid sampleId,
        Guid customFieldDefinitionId);

    Task<CustomFieldValue> SaveValueAsync(
        CustomFieldValue value);

    Task<bool> DeleteValueAsync(Guid id);
}