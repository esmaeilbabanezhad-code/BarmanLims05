using Barman.Application.DTOs.Test;

namespace Barman.Application.Interfaces;

public interface ITestResultDefinitionService
{
    Task<List<TestResultDefinitionDto>> GetByTestIdAsync(
        Guid testId);

    Task<TestResultDefinitionDto?> GetByIdAsync(
        Guid id);

    Task<TestResultDefinitionDto> CreateAsync(
        TestResultDefinitionDto dto);

    Task<TestResultDefinitionDto> UpdateAsync(
        TestResultDefinitionDto dto);

    Task DeleteAsync(
        Guid id);
}