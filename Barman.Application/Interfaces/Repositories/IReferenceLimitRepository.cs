using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface IReferenceLimitRepository
{
    Task<ReferenceLimit?> GetByIdAsync(Guid id);

    Task<List<ReferenceLimit>> GetByTestIdAsync(Guid testId);

    Task AddAsync(ReferenceLimit referenceLimit);

    void Update(ReferenceLimit referenceLimit);

    void Delete(ReferenceLimit referenceLimit);
}