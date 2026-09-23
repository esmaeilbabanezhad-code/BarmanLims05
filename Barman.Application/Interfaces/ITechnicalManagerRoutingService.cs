using Barman.Application.DTOs.TechnicalManager;

namespace Barman.Application.Interfaces;

public interface ITechnicalManagerRoutingService
{
    Task<TechnicalManagerRoutingResultDto?> ResolveAsync(
        Guid customerId,
        Guid? sampleCategoryId,
        Guid? matrixId,
        Guid? testPanelId,
        Guid testId,
        Guid? departmentId,
        CancellationToken cancellationToken = default);
}