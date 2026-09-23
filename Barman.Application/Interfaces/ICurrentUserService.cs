namespace Barman.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? EmployeeId { get; }

    bool IsAuthenticated { get; }

    Guid InstanceId { get; }

    void SetUser(Guid employeeId);

    void ClearUser();
}
