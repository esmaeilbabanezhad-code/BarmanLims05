using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class CustomFieldValueRepository : ICustomFieldValueRepository
{
    private readonly ApplicationDbContext _context;

    public CustomFieldValueRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CustomFieldValue value)
    {
        await _context.CustomFieldValues.AddAsync(value);
    }

    public async Task<CustomFieldValue?> GetByIdAsync(Guid id)
    {
        return await _context.CustomFieldValues
            .Include(x => x.CustomFieldDefinition)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);
    }

    public async Task<List<CustomFieldValue>> GetBySampleIdAsync(Guid sampleId)
    {
        return await _context.CustomFieldValues
            .Include(x => x.CustomFieldDefinition)
            .Where(x =>
                x.SampleId == sampleId &&
                !x.IsDeleted)
            .OrderBy(x => x.CustomFieldDefinition.DisplayOrder)
            .ThenBy(x => x.CustomFieldDefinition.Title)
            .ToListAsync();
    }

    public async Task<CustomFieldValue?> GetBySampleAndDefinitionIdAsync(
        Guid sampleId,
        Guid customFieldDefinitionId)
    {
        return await _context.CustomFieldValues
            .Include(x => x.CustomFieldDefinition)
            .FirstOrDefaultAsync(x =>
                x.SampleId == sampleId &&
                x.CustomFieldDefinitionId == customFieldDefinitionId &&
                !x.IsDeleted);
    }

    public void Update(CustomFieldValue value)
    {
        _context.CustomFieldValues.Update(value);
    }

    public void Delete(CustomFieldValue value)
    {
        _context.CustomFieldValues.Remove(value);
    }
}