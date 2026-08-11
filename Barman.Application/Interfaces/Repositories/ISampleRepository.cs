using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ISampleRepository
{
    Task AddAsync(Sample sample);

    Task<Sample?> GetByIdAsync(Guid id);

    Task<List<Sample>> GetByReceptionIdAsync(Guid receptionId);

    void Update(Sample sample);

    void Delete(Sample sample);
}