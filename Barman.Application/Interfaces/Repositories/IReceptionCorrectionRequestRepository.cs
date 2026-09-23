using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface IReceptionCorrectionRequestRepository
{
    Task AddAsync(ReceptionCorrectionRequest request);

    Task<ReceptionCorrectionRequest?> GetByIdAsync(Guid id);

    Task<List<ReceptionCorrectionRequest>> GetByReceptionIdAsync(
        Guid receptionId);

    Task<ReceptionCorrectionRequest?> GetPendingByReceptionIdAsync(
        Guid receptionId);

    Task<List<ReceptionCorrectionRequest>> GetPendingAsync();

    void Update(ReceptionCorrectionRequest request);
}