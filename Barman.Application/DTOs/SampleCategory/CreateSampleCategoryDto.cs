namespace Barman.Application.DTOs.SampleCategory;

public class CreateSampleCategoryDto
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Description { get; set; }
}