using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ICustomerTestPanelRepository
{
    Task<List<CustomerTestPanel>> GetByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<CustomerTestPanel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<CustomerTestPanel?> ResolveAsync(
        Guid customerId,
        Guid? sampleCategoryId,
        Guid? matrixId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        CustomerTestPanel entity,
        CancellationToken cancellationToken = default);

    void Update(CustomerTestPanel entity);

    void Delete(CustomerTestPanel entity);

}
