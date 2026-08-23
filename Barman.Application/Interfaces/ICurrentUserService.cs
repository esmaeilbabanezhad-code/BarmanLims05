namespace Barman.Application.Interfaces;

public interface ICurrentUserService
{

    Guid InstanceId { get; }
    Guid? EmployeeId { get; }

    bool IsAuthenticated { get; }

    void SetUser(Guid employeeId);

    void ClearUser();

}