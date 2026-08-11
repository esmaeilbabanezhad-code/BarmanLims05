namespace Barman.Application.DTOs.Matrix;

public class MatrixLookupDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public Guid SampleCategoryId { get; set; }

    public string SampleCategoryName { get; set; } = "";

    public string? Description { get; set; }
}