using Barman.Application.DTOs.Customer;

namespace Barman.Application.Interfaces;

public interface ICustomerTestPanelService
{
    Task<List<CustomerTestPanelDto>> GetByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<CustomerTestPanelDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<CustomerTestPanelDto?> ResolveAsync(
        Guid customerId,
        Guid? sampleCategoryId,
        Guid? matrixId,
        CancellationToken cancellationToken = default);

    Task<CustomerTestPanelDto> CreateAsync(
        CustomerTestPanelDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        CustomerTestPanelDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
