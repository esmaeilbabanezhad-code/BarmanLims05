using Barman.Application.DTOs.Customer;
using Barman.Application.Interfaces;

namespace Barman.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CustomerLookupDto>> GetLookupAsync()
    {
        var customers = await _unitOfWork.Customers.GetAllAsync();

        return customers
            .Select(x => new CustomerLookupDto
            {
                Id = x.Id,
                DisplayName = x.DisplayName
            })
            .ToList();
    }
}