using Barman.Domain.Entities;
using Barman.Application.DTOs.Customer;

namespace Barman.Application.Interfaces;

public interface ICustomerService
{
    Task<List<Customer>> GetAllAsync();

    Task<Customer?> GetByIdAsync(Guid id);

    Task<Customer> CreateAsync(Customer customer);

    Task<Customer?> UpdateAsync(Customer customer);

    Task<bool> DeleteAsync(Guid id);

    Task<Customer> ActivateAsync(Guid id);

    Task<Customer> DeactivateAsync(Guid id);

    Task<List<CustomerLookupDto>> GetLookupAsync();

    Task<List<CustomerLookupDto>> GetLookupWithDefaultPanelAsync();
}