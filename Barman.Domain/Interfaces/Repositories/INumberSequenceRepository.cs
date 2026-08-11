using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface INumberSequenceRepository
{
    Task<NumberSequence?> GetByEntityNameAsync(string entityName);

    Task SaveChangesAsync();
}