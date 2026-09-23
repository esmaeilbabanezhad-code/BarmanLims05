using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ICustomFieldValueRepository
{
    Task AddAsync(CustomFieldValue value);

    Task<CustomFieldValue?> GetByIdAsync(Guid id);

    Task<List<CustomFieldValue>> GetBySampleIdAsync(Guid sampleId);

    Task<CustomFieldValue?> GetBySampleAndDefinitionIdAsync(
        Guid sampleId,
        Guid customFieldDefinitionId);

    void Update(CustomFieldValue value);

    void Delete(CustomFieldValue value);
}