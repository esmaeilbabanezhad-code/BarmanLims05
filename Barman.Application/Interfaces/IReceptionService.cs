using Barman.Application.DTOs.Reception;
using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface IReceptionService
{
    Task<Reception> CreateReceptionAsync(
        CreateReceptionDto dto,
        CancellationToken cancellationToken = default);

    Task<List<Reception>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Reception?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<ReceptionHistoryLookupDto>> GetHistoryByCustomerIdAsync(
    Guid customerId,
    CancellationToken cancellationToken = default);

    Task<Reception?> GetForCorrectionAsync(
    Guid id,
    CancellationToken cancellationToken = default);
}