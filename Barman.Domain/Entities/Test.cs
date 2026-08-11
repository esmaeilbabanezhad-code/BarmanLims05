using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class Test : BaseEntity
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? EnglishName { get; set; }

    public string? Unit { get; set; }

    public string? Method { get; set; }

    public string? InstrumentName { get; set; }

    public decimal? LOD { get; set; }

    public decimal? LOQ { get; set; }

    public string? DefaultResult { get; set; }

    public bool IsQuantitative { get; set; }

    public bool IsActiveForReception { get; set; } = true;

    public string? Description { get; set; }

    // =========================
    // Department
    // =========================

    public Guid? DepartmentId { get; set; }

    public Department? Department { get; set; }

    public Guid? SampleCategoryId { get; set; }

    public SampleCategory? SampleCategory { get; set; }

    public Guid? MatrixId { get; set; }

    public Matrix? Matrix { get; set; }

    // =========================
    // Test Method
    // =========================

    public Guid? TestMethodId { get; set; }

    public TestMethod? TestMethod { get; set; }

    // =========================
    // Instrument
    // =========================

    public Guid? InstrumentId { get; set; }

    public Instrument? Instrument { get; set; }

    // =========================
    // Relations
    // =========================

    public ICollection<TestAssignment> Assignments { get; set; }
        = new List<TestAssignment>();

    public ICollection<TestPanelItem> PanelItems { get; set; }
        = new List<TestPanelItem>();

    public ICollection<ReferenceLimit> ReferenceLimits { get; set; }
        = new List<ReferenceLimit>();
}