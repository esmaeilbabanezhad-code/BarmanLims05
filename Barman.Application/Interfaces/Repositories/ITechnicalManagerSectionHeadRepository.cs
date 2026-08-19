using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ITechnicalManagerSectionHeadRepository
{
    Task<List<TechnicalManagerSectionHead>> GetByTechnicalManagerIdAsync(
        Guid technicalManagerId);

    Task<List<TechnicalManagerSectionHead>> GetBySectionHeadIdAsync(
        Guid sectionHeadId);

    Task<TechnicalManagerSectionHead?> GetByIdAsync(
        Guid id);

    Task AddAsync(
        TechnicalManagerSectionHead relation);

    void Update(
        TechnicalManagerSectionHead relation);

    void Delete(
        TechnicalManagerSectionHead relation);
}
