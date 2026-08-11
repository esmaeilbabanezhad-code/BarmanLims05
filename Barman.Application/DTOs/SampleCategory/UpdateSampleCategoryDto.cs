namespace Barman.Application.DTOs.SampleCategory;

public class UpdateSampleCategoryDto
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}