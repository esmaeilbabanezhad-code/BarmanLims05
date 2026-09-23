namespace Barman.Application.DTOs.TestTariff;

public class TariffImportRowDto
{
    public int RowNumber { get; set; }

    public string OrganizationCode { get; set; } = "";

    public string? CustomerCode { get; set; }

    public string? SampleCategoryCode { get; set; }

    public string? StandardSampleCode { get; set; }

    public string? MatrixCode { get; set; }

    public string TargetType { get; set; } = "";

    public string? TestCode { get; set; }

    public string? PanelCode { get; set; }

    public string TariffCode { get; set; } = "";

    public string TariffName { get; set; } = "";

    public decimal? Price { get; set; }

    public string? Currency { get; set; }

    public string? ValidFrom { get; set; }

    public string? ValidTo { get; set; }

    public int? Priority { get; set; }

    public bool? IsActive { get; set; }

    public string? Description { get; set; }
}