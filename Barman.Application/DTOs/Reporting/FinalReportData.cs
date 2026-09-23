namespace Barman.Application.DTOs.Reporting;

public class FinalReportData
{
    // =========================
    // Reception
    // =========================

    public Guid ReceptionId { get; set; }

    public string ReceptionNumber { get; set; } = "";

    public DateTime ReceptionDate { get; set; }

    public string ReceptionStatus { get; set; } = "";

    public bool IsUrgent { get; set; }

    public string? ReceptionDescription { get; set; }

    public decimal TotalPrice { get; set; }

    public bool IsPaid { get; set; }


    // =========================
    // Report
    // =========================

    public string ReportNumber { get; set; } = "";

    public string ReportVersion { get; set; } = "1.0";

    public DateTime IssueDate { get; set; } = DateTime.Now;

    public string? TemplateCode { get; set; }

    public string? TemplateVersion { get; set; }


    // =========================
    // Laboratory
    // =========================

    public ReportLaboratoryData Laboratory { get; set; } = new();


    // =========================
    // Customer
    // =========================

    public ReportCustomerData Customer { get; set; } = new();


    // =========================
    // Sample
    // =========================

    public ReportSampleData Sample { get; set; } = new();


    // =========================
    // Custom Fields
    // =========================

    public List<ReportCustomFieldData> CustomFields { get; set; } = new();


    // =========================
    // Tests & Results
    // =========================

    public List<FinalReportTestData> Tests { get; set; } = new();


    // =========================
    // Signatures
    // =========================

    public ReportSignatureData Signatures { get; set; } = new();
}


// ============================================================
// Laboratory
// ============================================================

public class ReportLaboratoryData
{
    public string Name { get; set; } = "";

    public string? Logo { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? AccreditationNo { get; set; }
}


// ============================================================
// Customer
// ============================================================

public class ReportCustomerData
{
    public Guid Id { get; set; }

    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? NationalId { get; set; }

    public string? EconomicCode { get; set; }

    public string? Phone { get; set; }

    public string? Mobile { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }
}


// ============================================================
// Sample
// ============================================================

public class ReportSampleData
{
    public Guid Id { get; set; }

    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? CustomerSampleName { get; set; }

    public string? SampleCategory { get; set; }

    public string? Matrix { get; set; }

    public string? StandardSample { get; set; }

    public string? BatchLotNumber { get; set; }

    public string? QuotaNumber { get; set; }

    public string? ShipmentNumber { get; set; }

    public DateOnly? ProductionDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public decimal? Quantity { get; set; }

    public string? Unit { get; set; }

    public string? ContainerType { get; set; }

    public string? Description { get; set; }
}


// ============================================================
// Custom Fields
// ============================================================

public class ReportCustomFieldData
{
    public Guid DefinitionId { get; set; }

    public string FieldCode { get; set; } = "";

    public string Title { get; set; } = "";

    public string DataType { get; set; } = "";

    public string? Value { get; set; }
}


// ============================================================
// Test
// ============================================================

public class FinalReportTestData
{
    public Guid AssignmentId { get; set; }

    public Guid TestId { get; set; }

    public string TestCode { get; set; } = "";

    public string TestName { get; set; } = "";

    public string? Method { get; set; }

    public string? Instrument { get; set; }

    public string? Result { get; set; }

    public string? Unit { get; set; }

    public string? Comment { get; set; }

    public decimal? LOD { get; set; }

    public decimal? LOQ { get; set; }

    public decimal? Min { get; set; }

    public decimal? Max { get; set; }

    public string? PermissibleLimit { get; set; }

    public string? LimitReference { get; set; }

    public bool? IsCompliant { get; set; }

    public List<FinalReportResultData> Results { get; set; } = new();
}


// ============================================================
// Result
// ============================================================

public class FinalReportResultData
{
    public Guid ResultValueId { get; set; }

    public Guid ResultDefinitionId { get; set; }

    public string ResultDefinitionCode { get; set; } = "";

    public string ResultDefinitionName { get; set; } = "";

    public int DisplayOrder { get; set; }

    public string? Value { get; set; }

    public string? Unit { get; set; }

    public string? Comment { get; set; }

    public decimal? LOD { get; set; }

    public decimal? LOQ { get; set; }

    public decimal? Min { get; set; }

    public decimal? Max { get; set; }

    public bool? IsCompliant { get; set; }
}


// ============================================================
// Signatures
// ============================================================

public class ReportSignatureData
{
    public string? SectionHeadName { get; set; }

    public string? TechnicalManagerName { get; set; }

    public string? DirectorName { get; set; }


    public string? SectionHeadSignature { get; set; }

    public string? TechnicalManagerSignature { get; set; }

    public string? DirectorSignature { get; set; }
}