namespace Barman.Application.Interfaces.Excel;

public interface IExcelExportService
{
    Task<byte[]> ExportTestTariffsAsync(
        CancellationToken cancellationToken = default);

    Task<byte[]> ExportTestTariffTemplateAsync(
        CancellationToken cancellationToken = default);

    Task<byte[]> ExportBarmanMasterDataAsync(
        CancellationToken cancellationToken = default);
}