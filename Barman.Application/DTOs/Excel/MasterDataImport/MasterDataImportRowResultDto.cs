namespace Barman.Application.DTOs.Excel.MasterDataImport;

public class MasterDataImportRowResultDto
{
    public string SheetName { get; set; } = "";

    public int RowNumber { get; set; }

    public string? Code { get; set; }

    public string Action { get; set; } = "";

    public bool IsValid { get; set; }

    public List<string> Errors { get; set; }
        = new();
}
