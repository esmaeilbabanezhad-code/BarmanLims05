namespace Barman.Application.DTOs.Reception;

public class ReceptionHistoryLookupDto
{
    public Guid Id { get; set; }

    public string ReceptionNumber { get; set; } = "";

    public DateTime ReceptionDate { get; set; }

    public Guid CustomerId { get; set; }

    public string CustomerName { get; set; } = "";

    public List<ReceptionHistorySampleDto> Samples { get; set; }
        = new();
}

public class ReceptionHistorySampleDto
{
    public Guid Id { get; set; }

    public string SampleCode { get; set; } = "";

    public string SampleName { get; set; } = "";

    public string? CustomerSampleName { get; set; }

    public List<ReceptionHistoryTestDto> Tests { get; set; }
        = new();
}

public class ReceptionHistoryTestDto
{
    public Guid TestId { get; set; }

    public string TestCode { get; set; } = "";

    public string TestName { get; set; } = "";

    public Guid? TestPanelId { get; set; }

    public string? TestPanelCode { get; set; }

    public string? TestPanelName { get; set; }
}
