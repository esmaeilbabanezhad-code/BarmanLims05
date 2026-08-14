namespace Barman.Application.DTOs.Customer;

public class CustomerTestPanelDto
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public Guid? SampleCategoryId { get; set; }

    public Guid? MatrixId { get; set; }

    public Guid TestPanelId { get; set; }

    public int Priority { get; set; }

    public string? Description { get; set; }
}
