using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class CustomFieldValue : BaseEntity
{
    public Guid SampleId { get; set; }
    public Sample Sample { get; set; } = null!;

    public Guid CustomFieldDefinitionId { get; set; }
    public CustomFieldDefinition CustomFieldDefinition { get; set; } = null!;

    public string? Value { get; set; }
}