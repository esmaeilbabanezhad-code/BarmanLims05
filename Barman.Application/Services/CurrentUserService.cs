using Barman.Application.Interfaces;

namespace Barman.Application.Services;

public class CurrentUserService : ICurrentUserService
{
    public Guid InstanceId { get; } = Guid.NewGuid();

    private Guid? _employeeId;

    public Guid? EmployeeId => _employeeId;

    public bool IsAuthenticated =>
        _employeeId.HasValue &&
        _employeeId.Value != Guid.Empty;

    public void SetUser(Guid employeeId)
    {
        if (employeeId == Guid.Empty)
            throw new ArgumentException(
                "EmployeeId cannot be empty.",
                nameof(employeeId));

        _employeeId = employeeId;
    }

    public void ClearUser()
    {
        _employeeId = null;
    }
}