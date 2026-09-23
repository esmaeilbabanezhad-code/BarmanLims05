using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ICustomFieldDefinitionRepository
{
    Task AddAsync(CustomFieldDefinition definition);

    Task<CustomFieldDefinition?> GetByIdAsync(Guid id);

    Task<List<CustomFieldDefinition>> GetActiveAsync(
        Guid? customerId,
        Guid? sampleCategoryId,
        Guid? matrixId);

    Task<List<CustomFieldDefinition>> GetAllAsync();

    void Update(CustomFieldDefinition definition);

    void Delete(CustomFieldDefinition definition);
}