using Barman.Domain.Entities;

namespace Barman.Application.Models;

public class BulkTestResultInput
{
    public TestAssignment Assignment { get; set; } = null!;

    public string Result { get; set; } = "";

    public string Comment { get; set; } = "";

    public decimal? RequestedLOD { get; set; }

    public decimal? RequestedLOQ { get; set; }

    public decimal? RequestedMinValue { get; set; }

    public decimal? RequestedMaxValue { get; set; }

    public string LimitChangeReason { get; set; } = "";
}