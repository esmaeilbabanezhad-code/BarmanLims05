namespace Barman.Application.DTOs.DefaultTestSet;

public class DefaultTestSetDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? SampleCategoryId { get; set; }

    public Guid? MatrixId { get; set; }

    public Guid? StandardSampleId { get; set; }

    public int Priority { get; set; } = 100;

    public List<DefaultTestSetItemDto> Items { get; set; } = new();
}