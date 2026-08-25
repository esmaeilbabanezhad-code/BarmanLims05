using Barman.Domain.Entities;

namespace Barman.Application.DTOs.Test;

public class TestLookupDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public Guid? DepartmentId { get; set; }

    public Guid? SampleCategoryId { get; set; }

    public Guid? MatrixId { get; set; }

    public string? EnglishName { get; set; }

    public string? Unit { get; set; }

    public bool IsQuantitative { get; set; }

    public bool IsActiveForReception { get; set; }

    public string? Method { get; set; }

    public string? InstrumentName { get; set; }

    public decimal? LOD { get; set; }

    public decimal? LOQ { get; set; }

    public string? DefaultResult { get; set; }

    public string? Description { get; set; }

    public List<ReferenceLimit> ReferenceLimits { get; set; } = new();

    public string DisplayName =>
        string.IsNullOrWhiteSpace(Code)
            ? Name
            : $"{Code} - {Name}";
}