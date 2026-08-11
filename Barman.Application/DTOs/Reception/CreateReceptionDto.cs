namespace Barman.Application.DTOs.Reception;

public class CreateReceptionDto
{
    public Guid CustomerId { get; set; }

    public bool IsUrgent { get; set; }

    public string? Description { get; set; }

    public List<CreateReceptionSampleDto> Samples { get; set; }
        = new();
}