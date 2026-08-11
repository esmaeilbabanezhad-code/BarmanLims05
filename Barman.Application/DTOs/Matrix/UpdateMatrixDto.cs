namespace Barman.Application.DTOs.Matrix;

public class UpdateMatrixDto
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public Guid SampleCategoryId { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}