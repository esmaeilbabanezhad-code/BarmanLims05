using Barman.Application.DTOs.Customer;

namespace Barman.Application.Interfaces;

public interface ICustomerService
{
    Task<List<CustomerLookupDto>> GetLookupAsync();
}