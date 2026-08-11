using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class Instrument : BaseEntity
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Model { get; set; }

    public string? Manufacturer { get; set; }

    public string? SerialNumber { get; set; }

    public bool IsActiveForTesting { get; set; } = true;

    public string? Description { get; set; }
}