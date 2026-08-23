namespace Barman.Application.DTOs.DefaultTestSet;

public class DefaultTestSetItemDto
{
    public Guid Id { get; set; }

    public Guid DefaultTestSetId { get; set; }

    public bool IsPanel { get; set; }

    public Guid? TestId { get; set; }

    public Guid? TestPanelId { get; set; }

    public int SortOrder { get; set; }

    // برای نمایش در UI
    public string? TestCode { get; set; }

    public string? TestName { get; set; }

    public string? TestPanelCode { get; set; }

    public string? TestPanelName { get; set; }
}