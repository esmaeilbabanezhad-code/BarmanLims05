using System.Security.Claims;
using Barman.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BarmanLims.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private Guid? _employeeId;
    private Guid _instanceId = Guid.NewGuid();

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? EmployeeId
    {
        get
        {
            var value =
                _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirstValue(
                        ClaimTypes.NameIdentifier);

            if (Guid.TryParse(value, out var employeeId))
            {
                _employeeId = employeeId;
                return employeeId;
            }

            return _employeeId;
        }
    }

    public bool IsAuthenticated
    {
        get
        {
            return _httpContextAccessor.HttpContext?
                .User?
                .Identity?
                .IsAuthenticated
                ?? false;
        }
    }

    public Guid InstanceId => _instanceId;

    public void SetUser(Guid employeeId)
    {
        if (employeeId == Guid.Empty)
        {
            _employeeId = null;
            return;
        }

        _employeeId = employeeId;
    }

    public void ClearUser()
    {
        _employeeId = null;
    }
}
