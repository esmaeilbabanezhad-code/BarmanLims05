using Barman.Application.DTOs.Reporting;
using Barman.Domain.Entities.Reporting;

namespace Barman.Application.Interfaces.Services.Reporting;

public interface IReportTemplateRenderer
{
    Task<ReportRenderingModel> RenderAsync(
        ReportTemplate template,
        FinalReportData data,
        CancellationToken cancellationToken = default);
}
