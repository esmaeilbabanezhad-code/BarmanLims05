namespace Barman.Application.DTOs.StandardSample;

public class CreateStandardSampleDto
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public Guid SampleCategoryId { get; set; }

    public Guid MatrixId { get; set; }

    public string? Description { get; set; }
}