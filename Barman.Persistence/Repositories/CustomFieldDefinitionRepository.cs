using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class CustomFieldDefinitionRepository : ICustomFieldDefinitionRepository
{
    private readonly ApplicationDbContext _context;

    public CustomFieldDefinitionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CustomFieldDefinition definition)
    {
        await _context.CustomFieldDefinitions.AddAsync(definition);
    }

    public async Task<CustomFieldDefinition?> GetByIdAsync(Guid id)
    {
        return await _context.CustomFieldDefinitions
            .Include(x => x.Customer)
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<CustomFieldDefinition>> GetActiveAsync(
        Guid? customerId,
        Guid? sampleCategoryId,
        Guid? matrixId)
    {
        return await _context.CustomFieldDefinitions
            .Include(x => x.Customer)
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Where(x =>
                x.IsActive &&
                !x.IsDeleted &&
                (
                    x.CustomerId == null ||
                    x.CustomerId == customerId
                ) &&
                (
                    x.SampleCategoryId == null ||
                    x.SampleCategoryId == sampleCategoryId
                ) &&
                (
                    x.MatrixId == null ||
                    x.MatrixId == matrixId
                ))
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Title)
            .ToListAsync();
    }

    public async Task<List<CustomFieldDefinition>> GetAllAsync()
    {
        return await _context.CustomFieldDefinitions
            .Include(x => x.Customer)
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Title)
            .ToListAsync();
    }

    public void Update(CustomFieldDefinition definition)
    {
        _context.CustomFieldDefinitions.Update(definition);
    }

    public void Delete(CustomFieldDefinition definition)
    {
        _context.CustomFieldDefinitions.Remove(definition);
    }
}