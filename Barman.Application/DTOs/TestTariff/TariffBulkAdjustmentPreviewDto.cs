namespace Barman.Application.DTOs.TestTariff;

public class TariffBulkAdjustmentPreviewDto
{
    public Guid TariffId { get; set; }

    public Guid? TestId { get; set; }

    public Guid? TestPanelId { get; set; }

    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string TargetType { get; set; } = "";

    public decimal OldPrice { get; set; }

    public decimal Percentage { get; set; }

    public decimal NewPrice { get; set; }

    public string Currency { get; set; } = "ریال";

    public DateTime OldValidFrom { get; set; }

    public DateTime? OldValidTo { get; set; }

    public DateTime NewValidFrom { get; set; }
}