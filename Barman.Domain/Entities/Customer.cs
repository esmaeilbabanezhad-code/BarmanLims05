using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class Customer : BaseEntity
{
    public string Code { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string? LegalName { get; set; }

    public string? NationalId { get; set; }

    public string? EconomicCode { get; set; }

    public string? RegistrationNo { get; set; }

    public string? Province { get; set; }

    public string? City { get; set; }

    public string? Address { get; set; }

    public string? PostalCode { get; set; }

    public string? Phone { get; set; }

    public string? Mobile { get; set; }

    public string? Email { get; set; }

    public string? Website { get; set; }

    public ICollection<CustomerTestPanel> TestPanelRules { get; set; }
    = new List<CustomerTestPanel>();
    public string? Description { get; set; }

   
}