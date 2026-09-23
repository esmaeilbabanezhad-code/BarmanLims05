using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Domain.Enums;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class ReceptionCorrectionRequestRepository
    : IReceptionCorrectionRequestRepository
{
    private readonly ApplicationDbContext _context;

    public ReceptionCorrectionRequestRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        ReceptionCorrectionRequest request)
    {
        await _context.ReceptionCorrectionRequests
            .AddAsync(request);
    }

    public async Task<ReceptionCorrectionRequest?> GetByIdAsync(
        Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _context.ReceptionCorrectionRequests
            .Include(x => x.Reception)
            .Include(x => x.Sample)
            .Include(x => x.RequestedByEmployee)
            .Include(x => x.ResolvedByEmployee)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);
    }

    public async Task<List<ReceptionCorrectionRequest>>
        GetByReceptionIdAsync(Guid receptionId)
    {
        if (receptionId == Guid.Empty)
            return new List<ReceptionCorrectionRequest>();

        return await _context.ReceptionCorrectionRequests
            .Include(x => x.Reception)
            .Include(x => x.Sample)
            .Include(x => x.RequestedByEmployee)
            .Include(x => x.ResolvedByEmployee)
            .Where(x =>
                x.ReceptionId == receptionId &&
                !x.IsDeleted)
            .OrderByDescending(x => x.RequestedAt)
            .ToListAsync();
    }

    public async Task<ReceptionCorrectionRequest?>
        GetPendingByReceptionIdAsync(Guid receptionId)
    {
        if (receptionId == Guid.Empty)
            return null;

        return await _context.ReceptionCorrectionRequests
            .Include(x => x.Reception)
            .Include(x => x.Sample)
            .Include(x => x.RequestedByEmployee)
            .Where(x =>
                x.ReceptionId == receptionId &&
                x.Status == ReceptionCorrectionRequestStatus.Pending &&
                !x.IsDeleted)
            .OrderByDescending(x => x.RequestedAt)
            .FirstOrDefaultAsync();
    }
    public async Task<List<ReceptionCorrectionRequest>>
    GetPendingAsync()
    {
        return await _context.ReceptionCorrectionRequests
            .Include(x => x.Reception)
            .Include(x => x.Sample)
            .Include(x => x.RequestedByEmployee)
            .Include(x => x.ResolvedByEmployee)
            .Where(x =>
                x.Status == ReceptionCorrectionRequestStatus.Pending &&
                !x.IsDeleted)
            .OrderByDescending(x => x.RequestedAt)
            .ToListAsync();
    }
    public void Update(
        ReceptionCorrectionRequest request)
    {
        _context.ReceptionCorrectionRequests.Update(request);
    }
}