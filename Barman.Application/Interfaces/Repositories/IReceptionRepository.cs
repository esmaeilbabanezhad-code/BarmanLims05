using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface IReceptionRepository
{
    Task AddAsync(Reception reception);

    Task<Reception?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<Reception>> GetAllAsync(
        CancellationToken cancellationToken = default);

    IQueryable<Reception> Query();
}