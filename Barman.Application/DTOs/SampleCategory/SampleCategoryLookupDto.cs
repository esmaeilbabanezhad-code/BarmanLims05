namespace Barman.Application.DTOs.SampleCategory;

public class SampleCategoryLookupDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Description { get; set; }
}