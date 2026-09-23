using Barman.Application.DTOs.TestTariff;
using Barman.Application.DTOs.Excel.MasterDataImport;

namespace Barman.Application.Interfaces.Excel;

public interface IExcelImportService
{
    // =========================================================
    // Test Tariffs
    // =========================================================

    Task<List<TariffImportRowDto>> ReadTestTariffsAsync(
        byte[] fileBytes,
        CancellationToken cancellationToken = default);

    Task<List<TariffImportRowResultDto>> ValidateTestTariffsAsync(
        List<TariffImportRowDto> rows,
        CancellationToken cancellationToken = default);

    Task<int> CommitTestTariffsAsync(
        List<TariffImportRowResultDto> rows,
        CancellationToken cancellationToken = default);

    // =========================================================
    // Barman Master Data
    // =========================================================

    Task<MasterDataImportResultDto> ImportBarmanMasterDataAsync(
        byte[] fileBytes,
        CancellationToken cancellationToken = default);
}