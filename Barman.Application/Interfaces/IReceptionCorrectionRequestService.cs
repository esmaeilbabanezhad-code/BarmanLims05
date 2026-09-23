using Barman.Application.DTOs.Reception;
using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface IReceptionCorrectionRequestService
{
    Task<ReceptionCorrectionRequest> CreateRequestAsync(
    Guid receptionId,
    Guid sampleId,
    string reason);
    Task<ReceptionCorrectionRequest?> GetByIdAsync(
        Guid requestId);

    Task<ReceptionCorrectionDto?> GetForCorrectionAsync(
    Guid requestId);

    Task UpdateCorrectionAsync(
    ReceptionCorrectionUpdateDto dto);

    Task<List<ReceptionCorrectionRequest>>
    GetPendingAsync();

    Task<List<ReceptionCorrectionRequest>>
        GetByReceptionIdAsync(
            Guid receptionId);

    Task<ReceptionCorrectionRequest?>
        GetPendingByReceptionIdAsync(
            Guid receptionId);

    Task ResolveAsync(
        Guid requestId);

    Task CancelAsync(
        Guid requestId);
}