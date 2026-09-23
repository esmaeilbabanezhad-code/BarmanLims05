namespace Barman.Application.DTOs.Excel.MasterDataImport;

public class MasterDataImportResultDto
{
    public bool IsValid { get; set; }

    public int TotalRows { get; set; }

    public int InsertCount { get; set; }

    public int UpdateCount { get; set; }

    public int SkipCount { get; set; }

    public int ErrorCount { get; set; }

    public List<MasterDataImportSheetResultDto> Sheets { get; set; }
        = new();

    public List<MasterDataImportRowResultDto> Errors { get; set; }
        = new();
}
