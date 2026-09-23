namespace Barman.Application.DTOs.Test;

public class TestResultDefinitionDto
{
    public Guid Id { get; set; }

    public Guid TestId { get; set; }

    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Unit { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsRequired { get; set; } = true;

    public int? DecimalPlaces { get; set; }

    public string? Description { get; set; }
}