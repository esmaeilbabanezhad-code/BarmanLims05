namespace Barman.Application.DTOs.CustomField;

public class CustomFieldValueDto
{
    public Guid? Id { get; set; }

    public Guid? DefinitionId { get; set; }

    public string Title { get; set; } = "";

    public string DataType { get; set; } = "Text";

    public bool IsRequired { get; set; }

    public string? Value { get; set; }

    public bool IsAdHoc { get; set; }

    public int DisplayOrder { get; set; }
}