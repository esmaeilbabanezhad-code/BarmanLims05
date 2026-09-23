using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class Sample : BaseEntity
{
    public Guid ReceptionId { get; set; }

    public Reception Reception { get; set; } = null!;

    public string SampleCode { get; set; } = "";

    public string SampleName { get; set; } = "";

    public string? CustomerSampleName { get; set; }

    public DateOnly? ProductionDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public string? BatchLotNumber { get; set; }

    public string? QuotaNumber { get; set; }

    public string? ShipmentNumber { get; set; }

    public Guid? SampleCategoryId { get; set; }

    public SampleCategory? SampleCategory { get; set; }

    public Guid? MatrixId { get; set; }

    public Matrix? Matrix { get; set; }

    public Guid? StandardSampleId { get; set; }

    public StandardSample? StandardSample { get; set; }

    public decimal? Quantity { get; set; }

    public string? Unit { get; set; }

    public string? ContainerType { get; set; }

    public string? Description { get; set; }

    public ICollection<TestAssignment> TestAssignments { get; set; }
        = new List<TestAssignment>();

    public ICollection<CustomFieldValue> CustomFieldValues { get; set; }
    = new List<CustomFieldValue>();
}
