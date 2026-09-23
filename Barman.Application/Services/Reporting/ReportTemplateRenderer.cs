using Barman.Application.DTOs.Reporting;
using Barman.Application.Interfaces.Services.Reporting;
using Barman.Domain.Entities.Reporting;

namespace Barman.Application.Services.Reporting;

public sealed class ReportTemplateRenderer : IReportTemplateRenderer
{
    private readonly IReportFieldResolver _fieldResolver;

    public ReportTemplateRenderer(
        IReportFieldResolver fieldResolver)
    {
        _fieldResolver = fieldResolver;
    }

    public Task<ReportRenderingModel> RenderAsync(
        ReportTemplate template,
        FinalReportData data,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(data);

        cancellationToken.ThrowIfCancellationRequested();

        var model = new ReportRenderingModel
        {
            Data = data,
            TemplateCode = template.Code,
            TemplateVersion = template.Version
        };

        data.TemplateCode = template.Code;
        data.TemplateVersion = template.Version;

        var sections = template.Sections
            .Where(x => x.IsVisible)
            .OrderBy(x => x.DisplayOrder)
            .ToList();

        foreach (var section in sections)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var renderedSection = new RenderedReportSection
            {
                SectionId = section.Id,
                Code = section.Code,
                Name = section.Name,
                SectionType = section.SectionType,
                Position = section.Position,
                Layout = NormalizeLayout(section.Layout),
                DisplayOrder = section.DisplayOrder,
                IsVisible = section.IsVisible
            };

            RenderSection(
                section,
                data,
                renderedSection,
                model);

            ApplySectionLayout(renderedSection);

            model.Sections.Add(renderedSection);
        }

        return Task.FromResult(model);
    }

    private void RenderSection(
        ReportTemplateSection section,
        FinalReportData data,
        RenderedReportSection renderedSection,
        ReportRenderingModel model)
    {
        var fields = section.Fields
            .Where(x => x.IsVisible)
            .OrderBy(x => x.DisplayOrder)
            .ToList();

        foreach (var field in fields)
        {
            var source = NormalizeSource(field.Source);

            switch (source)
            {
                case "Report":
                case "CustomField":
                    RenderReportField(
                        field,
                        data,
                        renderedSection);
                    break;

                case "Test":
                case "Limit":
                    RenderTestFields(
                        field,
                        data,
                        model);
                    break;

                case "Result":
                    RenderResultFields(
                        field,
                        data,
                        model);
                    break;

                default:
                    RenderReportField(
                        field,
                        data,
                        renderedSection);
                    break;
            }
        }
    }

    private void RenderReportField(
        ReportTemplateField field,
        FinalReportData data,
        RenderedReportSection section)
    {
        var resolved = _fieldResolver.Resolve(
            field.FieldCode,
            data);

        section.Fields.Add(
            CreateRenderedField(
                field,
                resolved));
    }

    private void RenderTestFields(
        ReportTemplateField field,
        FinalReportData data,
        ReportRenderingModel model)
    {
        foreach (var test in data.Tests)
        {
            var renderedTest =
                GetOrCreateRenderedTest(
                    model,
                    test);

            var resolved = _fieldResolver.Resolve(
                field.FieldCode,
                data,
                test);

            var renderedField = CreateRenderedField(
                field,
                resolved);

            renderedTest.Fields.Add(renderedField);

            AddColumnIfNeeded(
                renderedTest,
                renderedField);
        }
    }

    private void RenderResultFields(
        ReportTemplateField field,
        FinalReportData data,
        ReportRenderingModel model)
    {
        foreach (var test in data.Tests)
        {
            var renderedTest =
                GetOrCreateRenderedTest(
                    model,
                    test);

            if (test.Results.Count == 0)
            {
                var resolved = _fieldResolver.Resolve(
                    field.FieldCode,
                    data,
                    test);

                var renderedField = CreateRenderedField(
                    field,
                    resolved);

                var renderedResult =
                    GetOrCreateSingleResult(
                        renderedTest,
                        test);

                renderedResult.Fields.Add(renderedField);

                AddColumnIfNeeded(
                    renderedTest,
                    renderedField);

                continue;
            }

            foreach (var result in test.Results
                         .OrderBy(x => x.DisplayOrder))
            {
                var renderedResult =
                    GetOrCreateRenderedResult(
                        renderedTest,
                        result);

                var resolved = _fieldResolver.Resolve(
                    field.FieldCode,
                    data,
                    test,
                    result);

                var renderedField = CreateRenderedField(
                    field,
                    resolved);

                renderedResult.Fields.Add(renderedField);

                AddColumnIfNeeded(
                    renderedTest,
                    renderedField);
            }
        }
    }

    private static void ApplySectionLayout(
        RenderedReportSection section)
    {
        var fields = section.Fields
            .Where(x => x.IsVisible)
            .OrderBy(x => x.DisplayOrder)
            .ToList();

        section.Fields = fields;

        switch (NormalizeLayout(section.Layout))
        {
            case "Grid":
                ApplyGridLayout(section);
                break;

            case "Table":
                ApplyTableLayout(section);
                break;

            default:
                ApplyStackLayout(section);
                break;
        }
    }

    private static void ApplyStackLayout(
        RenderedReportSection section)
    {
        var fields = section.Fields
            .Where(x => x.IsVisible)
            .OrderBy(x => x.DisplayOrder)
            .ToList();

        foreach (var field in fields)
        {
            if (string.IsNullOrWhiteSpace(field.Alignment))
                field.Alignment = "Right";
        }

        section.Fields = fields;
    }

    private static void ApplyGridLayout(
    RenderedReportSection section)
    {
        var fields = section.Fields
            .Where(x => x.IsVisible)
            .OrderBy(x => x.DisplayOrder)
            .ToList();

        // Professional default:
        // information sections use four fields per row.
        if (section.ColumnCount <= 1)
            section.ColumnCount = 4;

        if (string.IsNullOrWhiteSpace(section.Gap))
            section.Gap = "8";

        if (string.IsNullOrWhiteSpace(section.Padding))
            section.Padding = "6";

        foreach (var field in fields)
        {
            if (string.IsNullOrWhiteSpace(field.Alignment))
                field.Alignment = GetDefaultAlignment(field);

            if (string.IsNullOrWhiteSpace(field.Width))
                field.Width = "1fr";
        }

        section.Fields = fields;
    }

    private static void ApplyTableLayout(
        RenderedReportSection section)
    {
        var fields = section.Fields
            .Where(x => x.IsVisible)
            .OrderBy(x => x.DisplayOrder)
            .ToList();

        foreach (var field in fields)
        {
            if (string.IsNullOrWhiteSpace(field.Alignment))
                field.Alignment = GetDefaultAlignment(field);

            if (string.IsNullOrWhiteSpace(field.Width))
                field.Width = "1fr";
        }

        section.Columns = fields;
        section.Fields = fields;
    }

    private static string GetDefaultAlignment(
        RenderedReportField field)
    {
        if (string.Equals(
                field.DataType,
                "Number",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Center";
        }

        if (string.Equals(
                field.DataType,
                "Date",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Center";
        }

        return "Right";
    }

    private static void AddColumnIfNeeded(
        RenderedReportTest test,
        RenderedReportField field)
    {
        var exists = test.Columns.Any(
            x =>
                x.Source.Equals(
                    field.Source,
                    StringComparison.OrdinalIgnoreCase)
                &&
                x.FieldCode.Equals(
                    field.FieldCode,
                    StringComparison.OrdinalIgnoreCase));

        if (exists)
            return;

        test.Columns.Add(field);
    }

    private static RenderedReportTest GetOrCreateRenderedTest(
        ReportRenderingModel model,
        FinalReportTestData test)
    {
        var existing = model.Tests.FirstOrDefault(
            x => x.AssignmentId == test.AssignmentId);

        if (existing is not null)
            return existing;

        var rendered = new RenderedReportTest
        {
            AssignmentId = test.AssignmentId,
            TestId = test.TestId,
            TestCode = test.TestCode,
            TestName = test.TestName
        };

        model.Tests.Add(rendered);

        return rendered;
    }

    private static RenderedReportResult GetOrCreateRenderedResult(
        RenderedReportTest test,
        FinalReportResultData result)
    {
        var existing = test.Results.FirstOrDefault(
            x => x.ResultValueId == result.ResultValueId);

        if (existing is not null)
            return existing;

        var rendered = new RenderedReportResult
        {
            ResultValueId = result.ResultValueId,
            ResultDefinitionId = result.ResultDefinitionId,
            ResultDefinitionCode = result.ResultDefinitionCode,
            ResultDefinitionName = result.ResultDefinitionName,
            DisplayOrder = result.DisplayOrder
        };

        test.Results.Add(rendered);

        return rendered;
    }

    private static RenderedReportResult GetOrCreateSingleResult(
        RenderedReportTest test,
        FinalReportTestData sourceTest)
    {
        var existing = test.Results.FirstOrDefault(
            x => x.ResultValueId == Guid.Empty);

        if (existing is not null)
            return existing;

        var rendered = new RenderedReportResult
        {
            ResultValueId = Guid.Empty,
            ResultDefinitionId = Guid.Empty,
            ResultDefinitionCode = sourceTest.TestCode,
            ResultDefinitionName = sourceTest.TestName,
            DisplayOrder = 0
        };

        test.Results.Add(rendered);

        return rendered;
    }

    private static RenderedReportField CreateRenderedField(
        ReportTemplateField field,
        ResolvedReportField resolved)
    {
        return new RenderedReportField
        {
            FieldId = field.Id,
            Source = NormalizeSource(field.Source),
            FieldCode = field.FieldCode,
            FieldType = field.FieldType,
            Caption = field.Caption ?? resolved.Caption,
            DisplayOrder = field.DisplayOrder,
            IsVisible = field.IsVisible,
            Width = NormalizeWidth(field.Width),
            Alignment = NormalizeAlignment(field.Alignment),
            Format = field.Format,
            Description = field.Description,
            Value = resolved.Value,
            DataType = resolved.DataType,
            IsResolved = resolved.IsResolved
        };
    }

    private static string NormalizeLayout(
        string? layout)
    {
        if (string.IsNullOrWhiteSpace(layout))
            return "Stack";

        return layout.Trim().ToUpperInvariant() switch
        {
            "GRID" => "Grid",
            "TABLE" => "Table",
            "STACK" => "Stack",
            _ => "Stack"
        };
    }

    private static string NormalizeAlignment(
        string? alignment)
    {
        if (string.IsNullOrWhiteSpace(alignment))
            return "Right";

        return alignment.Trim().ToUpperInvariant() switch
        {
            "RIGHT" => "Right",
            "CENTER" => "Center",
            "LEFT" => "Left",
            _ => "Right"
        };
    }

    private static string NormalizeWidth(
        string? width)
    {
        if (string.IsNullOrWhiteSpace(width))
            return "1fr";

        return width.Trim();
    }

    private static string NormalizeSource(
    string? source)
    {
        if (string.IsNullOrWhiteSpace(source))
            return "Report";

        var value = source.Trim();

        if (value.Equals(
                "CustomField",
                StringComparison.OrdinalIgnoreCase))
        {
            return "CustomField";
        }

        if (value.Equals(
                "Test",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Test";
        }

        if (value.Equals(
                "Limit",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Limit";
        }

        if (value.Equals(
                "Result",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Result";
        }

        return value switch
        {
            _ => "Report"
        };
    }
}