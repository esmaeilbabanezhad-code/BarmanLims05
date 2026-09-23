using Barman.Application.DTOs.CustomField;


namespace Barman.Application.DTOs.Reception;

public class ReceptionCorrectionDto
{
    // =========================
    // Correction Request
    // =========================

    public Guid RequestId { get; set; }

    public string CorrectionReason { get; set; } = "";

    // =========================
    // Reception - Read Only
    // =========================

    public Guid ReceptionId { get; set; }

    public string ReceptionNumber { get; set; } = "";

    public DateTime ReceptionDate { get; set; }

    public string CustomerName { get; set; } = "";

    // =========================
    // Sample - Read Only
    // =========================

    public Guid SampleId { get; set; }

    public string SampleCode { get; set; } = "";

    // =========================
    // Sample - Editable
    // =========================

    public string SampleName { get; set; } = "";

    public string? CustomerSampleName { get; set; }

    public DateTime? ProductionDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string? BatchLotNumber { get; set; }

    public string? QuotaNumber { get; set; }

    public string? ShipmentNumber { get; set; }

    public Guid? SampleCategoryId { get; set; }

    public Guid? MatrixId { get; set; }

    public Guid? StandardSampleId { get; set; }

    public decimal? Quantity { get; set; }

    public string? Unit { get; set; }

    public string? ContainerType { get; set; }

    public string? Description { get; set; }

    // =========================
    // Custom Fields
    // =========================

    public List<CustomFieldValueDto> CustomFields { get; set; } = new();

    // =========================
    // Tests
    // =========================

    public List<Guid> TestIds { get; set; } = new();

    public Dictionary<Guid, Guid> TestPanelIds { get; set; }
        = new();
}