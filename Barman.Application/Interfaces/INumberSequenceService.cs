namespace Barman.Application.Interfaces;

public interface INumberSequenceService
{
    Task<string> GetNextCodeAsync(
        string entityName,
        CancellationToken cancellationToken = default);
}
