namespace Barman.Application.Interfaces.Services.Reporting;

public interface IReportNumberGenerator
{
    Task<string> GenerateAsync(
        CancellationToken cancellationToken = default);
}