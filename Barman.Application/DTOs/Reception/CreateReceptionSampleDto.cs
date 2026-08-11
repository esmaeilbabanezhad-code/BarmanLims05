namespace Barman.Application.DTOs.Reception;

public class CreateReceptionSampleDto
{
    public string SampleName { get; set; } = "";

    public Guid? SampleCategoryId { get; set; }

    public string? Matrix { get; set; }

    public decimal? Quantity { get; set; }

    public string? Unit { get; set; }

    public string? ContainerType { get; set; }

    public string? Description { get; set; }

    public List<Guid> TestIds { get; set; } = new();
}