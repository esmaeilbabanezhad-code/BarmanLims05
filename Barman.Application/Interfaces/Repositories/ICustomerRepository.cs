using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer);

    Task<Customer?> GetByIdAsync(Guid id);

    Task<Customer?> GetByCodeAsync(string code);

    Task<List<Customer>> GetAllAsync();

    void Update(Customer customer);

    void Delete(Customer customer);
}