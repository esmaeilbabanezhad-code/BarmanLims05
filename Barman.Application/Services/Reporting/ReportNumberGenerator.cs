using Barman.Application.Interfaces;
using Barman.Application.Interfaces.Services.Reporting;

namespace Barman.Application.Services.Reporting;

public class ReportNumberGenerator : IReportNumberGenerator
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportNumberGenerator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<string> GenerateAsync(
        CancellationToken cancellationToken = default)
    {
        var year = DateTime.Now.Year;

        var reports =
            await _unitOfWork.IssuedReports
                .GetByYearAsync(year);

        var nextNumber = reports.Count + 1;

        return $"RPT-{year}-{nextNumber:000000}";
    }
}