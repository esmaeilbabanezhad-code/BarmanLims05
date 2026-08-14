using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class TestPanelService : ITestPanelService
{
    private readonly IUnitOfWork _unitOfWork;

    public TestPanelService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TestPanel>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.TestPanels
            .GetAllAsync(cancellationToken);
    }

    public async Task<TestPanel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        return await _unitOfWork.TestPanels
            .GetByIdAsync(id, cancellationToken);
    }

    public async Task<TestPanel> CreateAsync(
        TestPanel panel,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(panel.Code))
            throw new ArgumentException("کد Panel الزامی است.");

        if (string.IsNullOrWhiteSpace(panel.Name))
            throw new ArgumentException("نام Panel الزامی است.");

        panel.Code = panel.Code.Trim();
        panel.Name = panel.Name.Trim();

        var existing = await _unitOfWork.TestPanels
            .GetAllAsync(cancellationToken);

        if (existing.Any(x =>
            x.Code.Equals(panel.Code, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                $"Panel با کد «{panel.Code}» قبلاً ثبت شده است.");
        }

        panel.Id = Guid.NewGuid();
        panel.IsActive = true;
        panel.IsDeleted = false;

        await _unitOfWork.TestPanels
            .AddAsync(panel, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return panel;
    }

    public async Task<TestPanel?> UpdateAsync(
        TestPanel panel,
        CancellationToken cancellationToken = default)
    {
        if (panel.Id == Guid.Empty)
            return null;

        if (string.IsNullOrWhiteSpace(panel.Code))
            throw new ArgumentException("کد Panel الزامی است.");

        if (string.IsNullOrWhiteSpace(panel.Name))
            throw new ArgumentException("نام Panel الزامی است.");

        var current = await _unitOfWork.TestPanels
            .GetByIdAsync(panel.Id, cancellationToken);

        if (current == null)
            return null;

        var existing = await _unitOfWork.TestPanels
            .GetAllAsync(cancellationToken);

        if (existing.Any(x =>
            x.Id != panel.Id &&
            x.Code.Equals(
                panel.Code.Trim(),
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                $"Panel با کد «{panel.Code.Trim()}» قبلاً ثبت شده است.");
        }

        current.Code = panel.Code.Trim();
        current.Name = panel.Name.Trim();
        current.Description = panel.Description;

        _unitOfWork.TestPanels.Update(current);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return current;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return false;

        var panel = await _unitOfWork.TestPanels
            .GetByIdAsync(id, cancellationToken);

        if (panel == null)
            return false;

        panel.IsDeleted = true;

        _unitOfWork.TestPanels.Update(panel);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<TestPanel> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var panel = await _unitOfWork.TestPanels
            .GetByIdAsync(id, cancellationToken);

        if (panel == null)
            throw new KeyNotFoundException(
                "Test Panel not found.");

        panel.IsActive = true;

        _unitOfWork.TestPanels.Update(panel);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return panel;
    }

    public async Task<TestPanel> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var panel = await _unitOfWork.TestPanels
            .GetByIdAsync(id, cancellationToken);

        if (panel == null)
            throw new KeyNotFoundException(
                "Test Panel not found.");

        panel.IsActive = false;

        _unitOfWork.TestPanels.Update(panel);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return panel;
    }
}