namespace Barman.Application.Interfaces.Services.Reporting;

public interface IReportExportService
{
    Task<byte[]?> ExportWordAsync(
        Guid sampleId,
        Guid reportTemplateId,
        CancellationToken cancellationToken = default);

    Task<byte[]?> ExportPdfAsync(
        Guid sampleId,
        Guid reportTemplateId,
        CancellationToken cancellationToken = default);
}
