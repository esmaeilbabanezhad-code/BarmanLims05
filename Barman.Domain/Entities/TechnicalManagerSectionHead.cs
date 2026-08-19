using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class TechnicalManagerSectionHead : BaseEntity
{
    public Guid TechnicalManagerId { get; set; }

    public Employee TechnicalManager { get; set; } = null!;

    public Guid SectionHeadId { get; set; }

    public Employee SectionHead { get; set; } = null!;

}
