using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface IInstrumentRepository
{
    Task AddAsync(Instrument instrument);

    Task<Instrument?> GetByIdAsync(Guid id);

    Task<Instrument?> GetByCodeAsync(string code);

    Task<List<Instrument>> GetAllAsync();

    void Update(Instrument instrument);

    void Delete(Instrument instrument);
}
