namespace Barman.Application.DTOs.Excel.MasterDataImport;

public class MasterDataImportSheetResultDto
{
    public string SheetName { get; set; } = "";

    public int TotalRows { get; set; }

    public int InsertCount { get; set; }

    public int UpdateCount { get; set; }

    public int SkipCount { get; set; }

    public int ErrorCount { get; set; }

    public List<MasterDataImportRowResultDto> Rows { get; set; }
        = new();
}
