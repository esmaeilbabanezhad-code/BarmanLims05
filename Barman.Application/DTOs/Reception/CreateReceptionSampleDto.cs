namespace Barman.Application.DTOs.Reception;

public class CreateReceptionSampleDto
{
    public string SampleName { get; set; } = "";

    public Guid? SampleCategoryId { get; set; }

    public Guid? MatrixId { get; set; }

    public Guid? StandardSampleId { get; set; }

    public decimal? Quantity { get; set; }

    public string? Unit { get; set; }

    public string? ContainerType { get; set; }

    public string? Description { get; set; }

    public List<Guid> TestIds { get; set; } = new();
}
