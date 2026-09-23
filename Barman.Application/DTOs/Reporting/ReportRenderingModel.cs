namespace Barman.Application.DTOs.Reporting;

public sealed class ReportRenderingModel
{
    public FinalReportData Data { get; set; } = new();

    public string? TemplateCode { get; set; }

    public string? TemplateVersion { get; set; }

    public List<RenderedReportSection> Sections { get; set; } = new();

    public List<RenderedReportTest> Tests { get; set; } = new();
}

public sealed class RenderedReportSection
{
    public Guid SectionId { get; set; }

    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string SectionType { get; set; } = "";

    public string Position { get; set; } = "";

    public string Layout { get; set; } = "Stack";

    public int DisplayOrder { get; set; }

    public bool IsVisible { get; set; } = true;

    // Layout metadata
    public int ColumnCount { get; set; } = 1;

    public string? Gap { get; set; }

    public string? Padding { get; set; }

    public string? BackgroundColor { get; set; }

    public string? HeaderBackgroundColor { get; set; }

    public string? BorderColor { get; set; }

    public string? HeaderTextColor { get; set; }

    public bool ShowBorder { get; set; } = true;

    public bool ShowSeparator { get; set; } = false;

    public List<RenderedReportField> Fields { get; set; } = new();

    public List<RenderedReportField> Columns { get; set; } = new();
}

public sealed class RenderedReportTest
{
    public Guid AssignmentId { get; set; }

    public Guid TestId { get; set; }

    public string TestCode { get; set; } = "";

    public string TestName { get; set; } = "";

    public List<RenderedReportField> Fields { get; set; } = new();

    public List<RenderedReportField> Columns { get; set; } = new();

    public List<RenderedReportResult> Results { get; set; } = new();
}

public sealed class RenderedReportResult
{
    public Guid ResultValueId { get; set; }

    public Guid ResultDefinitionId { get; set; }

    public string ResultDefinitionCode { get; set; } = "";

    public string ResultDefinitionName { get; set; } = "";

    public int DisplayOrder { get; set; }

    public List<RenderedReportField> Fields { get; set; } = new();
}

public sealed class RenderedReportField
{
    public Guid FieldId { get; set; }

    public string Source { get; set; } = "";

    public string FieldCode { get; set; } = "";

    public string FieldType { get; set; } = "Value";

    public string? Caption { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsVisible { get; set; } = true;

    public string? Width { get; set; }

    public string? Alignment { get; set; }

    public string? Format { get; set; }

    public string? Description { get; set; }

    public object? Value { get; set; }

    public string? DataType { get; set; }

    public bool IsResolved { get; set; }
}