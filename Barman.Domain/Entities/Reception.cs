using Barman.Domain.Common;
using Barman.Domain.Enums;

namespace Barman.Domain.Entities;

public class Reception : BaseEntity
{
    public string ReceptionNumber { get; set; } = "";

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public DateTime ReceptionDate { get; set; } = DateTime.Now;

    public ReceptionStatus Status { get; set; } = ReceptionStatus.Draft;

    public bool IsUrgent { get; set; }

    public string? Description { get; set; }

    public decimal TotalPrice { get; set; }

    public bool IsPaid { get; set; }

    public Guid? TechManagerId { get; set; }

    public Employee? TechManager { get; set; }

    public Guid? DirectorId { get; set; }

    public Employee? Director { get; set; }

    public DateTime? FinalizedAt { get; set; }

    public ICollection<Sample> Samples { get; set; }
       = new List<Sample>();
}