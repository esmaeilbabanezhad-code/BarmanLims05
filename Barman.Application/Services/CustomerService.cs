using Barman.Application.DTOs.Customer;
using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly INumberGenerator _numberGenerator;

    public CustomerService(
        IUnitOfWork unitOfWork,
        IPermissionService permissionService,
        INumberGenerator numberGenerator)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _numberGenerator = numberGenerator;
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        await EnsurePermissionAsync("Customer.View");

        return await _unitOfWork.Customers.GetAllAsync();
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        await EnsurePermissionAsync("Customer.View");

        return await _unitOfWork.Customers.GetByIdAsync(id);
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        await EnsurePermissionAsync("Customer.Create");

        customer.Code = await _numberGenerator.GenerateAsync("Customer");

        customer.IsActive = true;

        await _unitOfWork.Customers.AddAsync(customer);

        await _unitOfWork.SaveChangesAsync();

        return customer;
    }

    public async Task<Customer?> UpdateAsync(Customer customer)
    {
        await EnsurePermissionAsync("Customer.Edit");

        var current =
            await _unitOfWork.Customers.GetByIdAsync(customer.Id);

        if (current == null)
            return null;

        current.Code = customer.Code;
        current.DisplayName = customer.DisplayName;
        current.LegalName = customer.LegalName;
        current.NationalId = customer.NationalId;
        current.EconomicCode = customer.EconomicCode;
        current.RegistrationNo = customer.RegistrationNo;
        current.Province = customer.Province;
        current.City = customer.City;
        current.Address = customer.Address;
        current.PostalCode = customer.PostalCode;
        current.Phone = customer.Phone;
        current.Mobile = customer.Mobile;
        current.Email = customer.Email;
        current.Website = customer.Website;
        current.Description = customer.Description;
        current.IsActive = customer.IsActive;

        _unitOfWork.Customers.Update(current);

        await _unitOfWork.SaveChangesAsync();

        return current;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await EnsurePermissionAsync("Customer.Delete");

        var current =
            await _unitOfWork.Customers.GetByIdAsync(id);

        if (current == null)
            return false;

        _unitOfWork.Customers.Delete(current);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<Customer> ActivateAsync(Guid id)
    {
        await EnsurePermissionAsync("Customer.Edit");

        var customer =
            await _unitOfWork.Customers.GetByIdAsync(id);

        if (customer == null)
            throw new KeyNotFoundException(
                "Customer not found.");

        customer.IsActive = true;

        _unitOfWork.Customers.Update(customer);

        await _unitOfWork.SaveChangesAsync();

        return customer;
    }

    public async Task<Customer> DeactivateAsync(Guid id)
    {
        await EnsurePermissionAsync("Customer.Edit");

        var customer =
            await _unitOfWork.Customers.GetByIdAsync(id);

        if (customer == null)
            throw new KeyNotFoundException(
                "Customer not found.");

        customer.IsActive = false;

        _unitOfWork.Customers.Update(customer);

        await _unitOfWork.SaveChangesAsync();

        return customer;
    }

    public async Task<List<CustomerLookupDto>> GetLookupAsync()
    {
        await EnsurePermissionAsync("Customer.View");

        var customers = await _unitOfWork.Customers.GetAllAsync();

        return customers
            .Where(x => x.IsActive)
            .Select(x => new CustomerLookupDto
            {
                Id = x.Id,
                DisplayName = x.DisplayName
            })
            .ToList();
    }

    public async Task<List<CustomerLookupDto>> GetLookupWithDefaultPanelAsync()
    {
     

        var customers = await _unitOfWork.Customers.GetAllAsync();

        return customers
            .Where(x => x.IsActive)
            .Select(x => new CustomerLookupDto
            {
                Id = x.Id,
                DisplayName = x.DisplayName
            })
            .ToList();
    }

    private async Task EnsurePermissionAsync(string permissionCode)
    {
        var allowed =
            await _permissionService.HasPermissionAsync(
                permissionCode);

        if (!allowed)
        {
            throw new UnauthorizedAccessException(
                $"Permission denied: {permissionCode}");
        }
    }
}