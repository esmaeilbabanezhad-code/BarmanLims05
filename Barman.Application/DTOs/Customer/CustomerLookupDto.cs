namespace Barman.Application.DTOs.Customer;

public class CustomerLookupDto
{
    public Guid Id { get; set; }

    public string DisplayName { get; set; } = "";

    public Guid? DefaultTestPanelId { get; set; }
}