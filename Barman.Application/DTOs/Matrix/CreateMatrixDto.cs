namespace Barman.Application.DTOs.Matrix;

public class CreateMatrixDto
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public Guid SampleCategoryId { get; set; }

    public string? Description { get; set; }
}