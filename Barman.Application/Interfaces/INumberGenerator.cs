namespace Barman.Application.Interfaces;

public interface INumberGenerator
{
    Task<string> GenerateAsync(
        string entityName,
        CancellationToken cancellationToken = default);
}