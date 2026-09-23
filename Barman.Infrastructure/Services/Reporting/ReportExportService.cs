using Barman.Application.DTOs.Reporting;
using Barman.Application.Interfaces.Services.Reporting;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using W = DocumentFormat.OpenXml.Wordprocessing;

namespace Barman.Infrastructure.Services.Reporting;

public sealed class ReportExportService : IReportExportService
{
    private readonly IFinalReportDataService _finalReportDataService;
    private readonly IReportTemplateService _reportTemplateService;
    private readonly IReportTemplateRenderer _reportTemplateRenderer;

    public ReportExportService(
        IFinalReportDataService finalReportDataService,
        IReportTemplateService reportTemplateService,
        IReportTemplateRenderer reportTemplateRenderer)
    {
        _finalReportDataService = finalReportDataService;
        _reportTemplateService = reportTemplateService;
        _reportTemplateRenderer = reportTemplateRenderer;
    }

    public async Task<byte[]?> ExportWordAsync(
        Guid sampleId,
        Guid reportTemplateId,
        CancellationToken cancellationToken = default)
    {
        var model = await BuildRenderingModelAsync(
            sampleId,
            reportTemplateId,
            cancellationToken);

        if (model is null)
            return null;

        return BuildWordDocument(model);
    }

    public async Task<byte[]?> ExportPdfAsync(
        Guid sampleId,
        Guid reportTemplateId,
        CancellationToken cancellationToken = default)
    {
        var model = await BuildRenderingModelAsync(
            sampleId,
            reportTemplateId,
            cancellationToken);

        if (model is null)
            return null;

        return BuildPdfDocument(model);
    }

    private async Task<ReportRenderingModel?> BuildRenderingModelAsync(
        Guid sampleId,
        Guid reportTemplateId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var data =
            await _finalReportDataService.GetBySampleIdAsync(
                sampleId,
                cancellationToken);

        if (data is null)
            return null;

        var template =
            await _reportTemplateService.GetByIdAsync(
                reportTemplateId,
                cancellationToken);

        if (template is null)
            return null;

        return await _reportTemplateRenderer.RenderAsync(
            template,
            data,
            cancellationToken);
    }

    // ============================================================
    // WORD
    // ============================================================

    private static byte[] BuildWordDocument(
        ReportRenderingModel model)
    {
        using var stream = new MemoryStream();

        using (var document =
               WordprocessingDocument.Create(
                   stream,
                   WordprocessingDocumentType.Document,
                   true))
        {
            var mainPart =
                document.AddMainDocumentPart();

            mainPart.Document =
                new W.Document(
                    new W.Body());

            var body =
                mainPart.Document.Body!;

            ConfigureWordPage(body);

            AddWordHeader(
                body,
                model);

            AddWordTemplateSections(
                body,
                model);

            AddWordTests(
                body,
                model);

            mainPart.Document.Save();
        }

        return stream.ToArray();
    }

    private static void ConfigureWordPage(
        W.Body body)
    {
        var sectionProperties =
            new W.SectionProperties();

        sectionProperties.Append(
            new W.PageSize
            {
                Width = 11906,
                Height = 16838
            });

        sectionProperties.Append(
            new W.PageMargin
            {
                Top = 500,
                Bottom = 500,
                Left = 600,
                Right = 600
            });

        body.Append(sectionProperties);
    }

    private static void AddWordHeader(
        W.Body body,
        ReportRenderingModel model)
    {
        var laboratoryName =
            model.Data.Laboratory.Name ?? "";

        if (!string.IsNullOrWhiteSpace(
                laboratoryName))
        {
            body.Append(
                CreateWordParagraph(
                    laboratoryName,
                    "28",
                    true,
                    W.JustificationValues.Center,
                    0,
                    40));
        }

        if (!string.IsNullOrWhiteSpace(
                model.Data.ReportNumber))
        {
            body.Append(
                CreateWordParagraph(
                    $"گزارش آزمون - {model.Data.ReportNumber}",
                    "22",
                    true,
                    W.JustificationValues.Center,
                    0,
                    100));
        }
    }

    private static void AddWordTemplateSections(
        W.Body body,
        ReportRenderingModel model)
    {
        foreach (var section in model.Sections
                     .Where(x => x.IsVisible)
                     .OrderBy(x => x.DisplayOrder))
        {
            var fields =
                section.Fields
                    .Where(x => x.IsVisible)
                    .OrderBy(x => x.DisplayOrder)
                    .ToList();

            if (fields.Count == 0)
                continue;

            AddWordSectionTitle(
                body,
                section);

            switch (NormalizeLayout(section.Layout))
            {
                case "Grid":
                    AddWordGridSection(
                        body,
                        section,
                        fields);
                    break;

                case "Table":
                    AddWordTableSection(
                        body,
                        section,
                        fields);
                    break;

                default:
                    AddWordStackSection(
                        body,
                        section,
                        fields);
                    break;
            }
        }
    }

    private static void AddWordSectionTitle(
        W.Body body,
        RenderedReportSection section)
    {
        if (string.IsNullOrWhiteSpace(
                section.Name))
            return;

        body.Append(
            CreateWordParagraph(
                section.Name,
                "18",
                true,
                W.JustificationValues.Right,
                60,
                30));
    }

    private static void AddWordStackSection(
        W.Body body,
        RenderedReportSection section,
        List<RenderedReportField> fields)
    {
        foreach (var field in fields)
        {
            if (field.FieldType.Equals(
                    "Image",
                    StringComparison.OrdinalIgnoreCase))
                continue;

            var caption =
                field.Caption ??
                field.FieldCode;

            var value =
                FormatFieldValue(field);

            var text =
                string.IsNullOrWhiteSpace(caption)
                    ? value
                    : $"{caption}: {value}";

            AddWordAlignedParagraph(
                body,
                text,
                field.Alignment);
        }

        AddWordSeparator(
            body,
            section);
    }

    private static void AddWordGridSection(
        W.Body body,
        RenderedReportSection section,
        List<RenderedReportField> fields)
    {
        var columnCount =
            section.ColumnCount > 0
                ? section.ColumnCount
                : 2;

        columnCount =
            Math.Max(
                1,
                Math.Min(
                    columnCount,
                    4));

        var table =
            CreateWordTable(
                section.ShowBorder);

        for (
            var index = 0;
            index < fields.Count;
            index += columnCount)
        {
            var row =
                new W.TableRow();

            for (
                var column = 0;
                column < columnCount;
                column++)
            {
                var fieldIndex =
                    index + column;

                if (fieldIndex >= fields.Count)
                {
                    row.Append(
                        CreateWordCell(""));
                    continue;
                }

                var field =
                    fields[fieldIndex];

                var caption =
                    field.Caption ??
                    field.FieldCode;

                var value =
                    FormatFieldValue(field);

                var text =
                    string.IsNullOrWhiteSpace(caption)
                        ? value
                        : $"{caption}: {value}";

                row.Append(
                    CreateWordCell(
                        text,
                        field.Alignment));
            }

            table.Append(row);
        }

        body.Append(table);

        AddWordSeparator(
            body,
            section);
    }

    private static void AddWordTableSection(
        W.Body body,
        RenderedReportSection section,
        List<RenderedReportField> fields)
    {
        var table =
            CreateWordTable(
                section.ShowBorder);

        foreach (var field in fields)
        {
            var row =
                new W.TableRow();

            var caption =
                field.Caption ??
                field.FieldCode;

            var value =
                FormatFieldValue(field);

            row.Append(
                CreateWordCell(
                    caption,
                    "Right",
                    true));

            row.Append(
                CreateWordCell(
                    value,
                    field.Alignment));

            table.Append(row);
        }

        body.Append(table);

        AddWordSeparator(
            body,
            section);
    }

    private static void AddWordSeparator(
        W.Body body,
        RenderedReportSection section)
    {
        if (!section.ShowSeparator)
            return;

        var paragraph =
            new W.Paragraph(
                new W.ParagraphProperties(
                    new W.ParagraphBorders(
                        new W.BottomBorder
                        {
                            Val =
                                W.BorderValues.Single,
                            Size = 4,
                            Color = "D9D9D9"
                        }),
                    new W.SpacingBetweenLines
                    {
                        Before = "20",
                        After = "60"
                    }));

        body.Append(paragraph);
    }

    // ============================================================
    // WORD RESULTS
    // ============================================================

    private static void AddWordTests(
        W.Body body,
        ReportRenderingModel model)
    {
        if (model.Tests.Count == 0)
            return;

        var columns = model.Tests
    .SelectMany(x => x.Columns)
    .Where(x => x.IsVisible)
    .Where(x =>
        !(
            x.Source.Equals(
                "Test",
                StringComparison.OrdinalIgnoreCase)
            &&
            (
                x.FieldCode.Equals(
                    "TestName",
                    StringComparison.OrdinalIgnoreCase)
                ||
                x.FieldCode.Equals(
                    "TestCode",
                    StringComparison.OrdinalIgnoreCase)
            )
        ))
    .GroupBy(
        x => $"{x.Source}|{x.FieldCode}",
        StringComparer.OrdinalIgnoreCase)
    .Select(x => x.First())
    .OrderBy(x => x.DisplayOrder)
    .ToList();

        if (columns.Count == 0)
            return;

        body.Append(
            CreateWordParagraph(
                "نتایج آزمون‌ها",
                "20",
                true,
                W.JustificationValues.Right,
                80,
                40));

        var table =
            CreateWordTable(true);

        var header =
            new W.TableRow();

        header.Append(
            CreateWordCell(
                "آزمون",
                "Center",
                true));

        foreach (var column in columns)
        {
            header.Append(
                CreateWordCell(
                    column.Caption ??
                    column.FieldCode,
                    column.Alignment ??
                    "Center",
                    true));
        }

        table.Append(header);

        foreach (var test in model.Tests)
        {
            var results =
                test.Results.Count == 0
                    ? new List<RenderedReportResult>
                    {
                        new()
                    }
                    : test.Results
                        .OrderBy(
                            x => x.DisplayOrder)
                        .ToList();

            foreach (var result in results)
            {
                var row =
                    new W.TableRow();

                row.Append(
                    CreateWordCell(
                        test.TestName,
                        "Right"));

                foreach (var column in columns)
                {
                    var field =
                        FindResultField(
                            test,
                            result,
                            column);

                    row.Append(
                        CreateWordCell(
                            FormatFieldValue(field),
                            column.Alignment ??
                            "Center"));
                }

                table.Append(row);
            }
        }

        body.Append(table);

        body.Append(
            new W.Paragraph(
                new W.ParagraphProperties(
                    new W.SpacingBetweenLines
                    {
                        After = "80"
                    })));
    }

    // ============================================================
    // WORD HELPERS
    // ============================================================

    private static W.Table CreateWordTable(
        bool showBorders)
    {
        var table =
            new W.Table();

        W.TableBorders borders;

        if (showBorders)
        {
            borders =
                new W.TableBorders(
                    new W.TopBorder
                    {
                        Val =
                            W.BorderValues.Single,
                        Size = 4,
                        Color = "D9D9D9"
                    },
                    new W.BottomBorder
                    {
                        Val =
                            W.BorderValues.Single,
                        Size = 4,
                        Color = "D9D9D9"
                    },
                    new W.LeftBorder
                    {
                        Val =
                            W.BorderValues.Single,
                        Size = 4,
                        Color = "D9D9D9"
                    },
                    new W.RightBorder
                    {
                        Val =
                            W.BorderValues.Single,
                        Size = 4,
                        Color = "D9D9D9"
                    },
                    new W.InsideHorizontalBorder
                    {
                        Val =
                            W.BorderValues.Single,
                        Size = 4,
                        Color = "E5E5E5"
                    },
                    new W.InsideVerticalBorder
                    {
                        Val =
                            W.BorderValues.Single,
                        Size = 4,
                        Color = "E5E5E5"
                    });
        }
        else
        {
            borders =
                new W.TableBorders(
                    new W.TopBorder
                    {
                        Val =
                            W.BorderValues.Nil
                    },
                    new W.BottomBorder
                    {
                        Val =
                            W.BorderValues.Nil
                    },
                    new W.LeftBorder
                    {
                        Val =
                            W.BorderValues.Nil
                    },
                    new W.RightBorder
                    {
                        Val =
                            W.BorderValues.Nil
                    },
                    new W.InsideHorizontalBorder
                    {
                        Val =
                            W.BorderValues.Nil
                    },
                    new W.InsideVerticalBorder
                    {
                        Val =
                            W.BorderValues.Nil
                    });
        }

        table.AppendChild(
            new W.TableProperties(
                borders,
                new W.TableWidth
                {
                    Type =
                        W.TableWidthUnitValues.Pct,
                    Width = "5000"
                },
                new W.TableLayout
                {
                    Type =
                        W.TableLayoutValues.Fixed
                }));

        return table;
    }

    private static W.TableCell CreateWordCell(
        string? value,
        string? alignment = null,
        bool bold = false)
    {
        var paragraph =
            new W.Paragraph(
                new W.ParagraphProperties(
                    new W.Justification
                    {
                        Val =
                            ParseWordAlignment(
                                alignment)
                    },
                    new W.SpacingBetweenLines
                    {
                        Before = "0",
                        After = "0"
                    }));

        var run =
            new W.Run();

        if (bold)
        {
            run.Append(
                new W.RunProperties(
                    new W.Bold()));
        }

        run.Append(
            new W.Text(
                value ?? "")
            );

        paragraph.Append(run);

        return new W.TableCell(
            new W.TableCellProperties(
                new W.TableCellVerticalAlignment
                {
                    Val =
                        W.TableVerticalAlignmentValues
                            .Center
                },
                new W.TableCellMargin(
                    new W.TopMargin
                    {
                        Width = "40",
                        Type =
                            W.TableWidthUnitValues.Dxa
                    },
                    new W.BottomMargin
                    {
                        Width = "40",
                        Type =
                            W.TableWidthUnitValues.Dxa
                    },
                    new W.LeftMargin
                    {
                        Width = "60",
                        Type =
                            W.TableWidthUnitValues.Dxa
                    },
                    new W.RightMargin
                    {
                        Width = "60",
                        Type =
                            W.TableWidthUnitValues.Dxa
                    })),
            paragraph);
    }

    private static W.Paragraph CreateWordParagraph(
        string text,
        string fontSize,
        bool bold,
        W.JustificationValues alignment,
        int spacingBefore,
        int spacingAfter)
    {
        var runProperties =
            new W.RunProperties(
                new W.FontSize
                {
                    Val = fontSize
                });

        if (bold)
            runProperties.Append(
                new W.Bold());

        return new W.Paragraph(
            new W.ParagraphProperties(
                new W.Justification
                {
                    Val = alignment
                },
                new W.SpacingBetweenLines
                {
                    Before =
                        spacingBefore.ToString(
                            CultureInfo.InvariantCulture),
                    After =
                        spacingAfter.ToString(
                            CultureInfo.InvariantCulture)
                }),
            new W.Run(
                runProperties,
                new W.Text(
                    text ?? "")
                ));
    }

    private static void AddWordAlignedParagraph(
        W.Body body,
        string text,
        string? alignment)
    {
        body.Append(
            CreateWordParagraph(
                text,
                "18",
                false,
                ParseWordAlignment(alignment),
                0,
                20));
    }

    private static W.JustificationValues ParseWordAlignment(
        string? alignment)
    {
        return alignment?
            .Trim()
            .ToLowerInvariant() switch
        {
            "left" =>
                W.JustificationValues.Left,

            "center" =>
                W.JustificationValues.Center,

            _ =>
                W.JustificationValues.Right
        };
    }

    // ============================================================
    // PDF
    // ============================================================

    private static byte[] BuildPdfDocument(
        ReportRenderingModel model)
    {
        QuestPDF.Settings.License =
            LicenseType.Community;

        QuestPDF.Settings.UseSystemFonts =
            true;

        var document =
            QuestPDF.Fluent.Document.Create(
                container =>
                {
                    container.Page(
                        page =>
                        {
                            page.Size(
                                QuestPDF.Helpers.PageSizes.A4);

                            page.Margin(28);

                            page.ContentFromRightToLeft();

                            page.DefaultTextStyle(
                                style =>
                                    style
                                        .FontFamily(
                                            "Tahoma",
                                            "Arial",
                                            "Lato")
                                        .FontSize(9)
                                        .FontColor(
                                            "#1F2937"));

                            page.Header()
                                .Element(
                                    container =>
                                        BuildPdfHeader(
                                            container,
                                            model));

                            page.Content()
                                .Element(
                                    container =>
                                        BuildPdfContent(
                                            container,
                                            model));

                            page.Footer()
                                .PaddingTop(4)
                                .AlignCenter()
                                .Text(
                                    "Barman Laboratory")
                                .FontSize(7.5f)
                                .FontColor(
                                    "#6B7280");
                        });
                });

        return document.GeneratePdf();
    }

    private static void BuildPdfHeader(
        IContainer container,
        ReportRenderingModel model)
    {
        container
            .PaddingBottom(6)
            .Column(
                column =>
                {
                    column.Item()
                        .AlignCenter()
                        .Text(
                            model.Data.Laboratory.Name ?? "")
                        .Bold()
                        .FontSize(15)
                        .FontColor("#17365D");

                    column.Item()
                        .PaddingTop(1)
                        .AlignCenter()
                        .Text(
                            $"گزارش آزمون - {model.Data.ReportNumber}")
                        .Bold()
                        .FontSize(10.5f)
                        .FontColor("#374151");

                    column.Item()
                        .PaddingTop(4)
                        .LineHorizontal(1)
                        .LineColor("#2F75B5");
                });
    }

    private static void BuildPdfContent(
        IContainer container,
        ReportRenderingModel model)
    {
        container.Column(
            column =>
            {
                foreach (var section in model.Sections
                             .Where(x => x.IsVisible)
                             .OrderBy(x => x.DisplayOrder))
                {
                    AddPdfSection(
                        column,
                        section);
                }

                AddPdfTests(
                    column,
                    model);
            });
    }

    private static void AddPdfSection(
        ColumnDescriptor column,
        RenderedReportSection section)
    {
        var fields =
            section.Fields
                .Where(x => x.IsVisible)
                .OrderBy(x => x.DisplayOrder)
                .ToList();

        if (fields.Count == 0)
            return;

        var sectionBackground =
            string.IsNullOrWhiteSpace(
                section.BackgroundColor)
                ? "#FFFFFF"
                : section.BackgroundColor;

        var headerBackground =
            string.IsNullOrWhiteSpace(
                section.HeaderBackgroundColor)
                ? "#EAF2F8"
                : section.HeaderBackgroundColor;

        var borderColor =
            string.IsNullOrWhiteSpace(
                section.BorderColor)
                ? "#C9D4DF"
                : section.BorderColor;

        var headerTextColor =
            string.IsNullOrWhiteSpace(
                section.HeaderTextColor)
                ? "#17365D"
                : section.HeaderTextColor;

        column.Item()
            .PaddingTop(3)
            .PaddingBottom(4)
            .Element(
                container =>
                {
                    var sectionContainer =
                        container
                            .Background(
                                sectionBackground);

                    if (section.ShowBorder)
                    {
                        sectionContainer =
                            sectionContainer
                                .Border(0.5f)
                                .BorderColor(
                                    borderColor);
                    }

                    sectionContainer
                        .Column(
                            sectionColumn =>
                            {
                                if (!string.IsNullOrWhiteSpace(
                                        section.Name))
                                {
                                    sectionColumn.Item()
                                        .Background(
                                            headerBackground)
                                        .PaddingVertical(3)
                                        .PaddingHorizontal(6)
                                        .AlignRight()
                                        .Text(
                                            section.Name)
                                        .Bold()
                                        .FontSize(9.5f)
                                        .FontColor(
                                            headerTextColor);
                                }

                                sectionColumn.Item()
                                    .PaddingTop(3)
                                    .PaddingBottom(2)
                                    .PaddingHorizontal(5)
                                    .Element(
                                        content =>
                                        {
                                            switch (
                                                NormalizeLayout(
                                                    section.Layout))
                                            {
                                                case "Grid":
                                                    AddPdfGridSection(
                                                        content,
                                                        section,
                                                        fields);
                                                    break;

                                                case "Table":
                                                    AddPdfTableSection(
                                                        content,
                                                        section,
                                                        fields);
                                                    break;

                                                default:
                                                    AddPdfStackSection(
                                                        content,
                                                        section,
                                                        fields);
                                                    break;
                                            }
                                        });
                            });
                });
    }

    private static void AddPdfStackSection(
        IContainer container,
        RenderedReportSection section,
        List<RenderedReportField> fields)
    {
        container.Column(
            column =>
            {
                foreach (var field in fields)
                {
                    if (field.FieldType.Equals(
                            "Image",
                            StringComparison.OrdinalIgnoreCase))
                        continue;

                    var caption =
                        field.Caption ??
                        field.FieldCode;

                    var value =
                        FormatFieldValue(field);

                    column.Item()
                        .PaddingVertical(1)
                        .Element(
                            item =>
                            {
                                item.Row(
                                    row =>
                                    {
                                        row.RelativeItem(1)
                                            .AlignRight()
                                            .Text(
                                                caption)
                                            .Bold()
                                            .FontSize(8.5f);

                                        row.RelativeItem(2)
                                            .AlignRight()
                                            .Text(
                                                value)
                                            .FontSize(8.5f);
                                    });
                            });
                }
            });
    }

    private static void AddPdfGridSection(
        IContainer container,
        RenderedReportSection section,
        List<RenderedReportField> fields)
    {
        var columnCount =
            section.ColumnCount > 0
                ? section.ColumnCount
                : 2;

        columnCount =
            Math.Max(
                1,
                Math.Min(
                    columnCount,
                    4));

        container
            .Table(
                table =>
                {
                    table.ColumnsDefinition(
                        definitions =>
                        {
                            for (
                                var i = 0;
                                i < columnCount;
                                i++)
                            {
                                definitions.RelativeColumn();
                            }
                        });

                    foreach (var field in fields)
                    {
                        var caption =
                            field.Caption ??
                            field.FieldCode;

                        var value =
                            FormatFieldValue(field);

                        var text =
                            string.IsNullOrWhiteSpace(caption)
                                ? value
                                : $"{caption}: {value}";

                        table.Cell()
                            .Element(
                                PdfCompactCell)
                            .AlignRight()
                            .Text(text)
                            .FontSize(8.5f);
                    }
                });
    }

    private static void AddPdfTableSection(
        IContainer container,
        RenderedReportSection section,
        List<RenderedReportField> fields)
    {
        container
            .Table(
                table =>
                {
                    table.ColumnsDefinition(
                        definitions =>
                        {
                            definitions.RelativeColumn(1);
                            definitions.RelativeColumn(2);
                        });

                    foreach (var field in fields)
                    {
                        var caption =
                            field.Caption ??
                            field.FieldCode;

                        var value =
                            FormatFieldValue(field);

                        table.Cell()
                            .Element(
                                PdfLabelCell)
                            .AlignRight()
                            .Text(caption)
                            .Bold()
                            .FontSize(8.5f);

                        table.Cell()
                            .Element(
                                PdfValueCell)
                            .AlignRight()
                            .Text(value)
                            .FontSize(8.5f);
                    }
                });
    }

    // ============================================================
    // PDF RESULTS
    // ============================================================

    private static void AddPdfTests(
        ColumnDescriptor column,
        ReportRenderingModel model)
    {
        if (model.Tests.Count == 0)
            return;

        var columns = model.Tests
            .SelectMany(x => x.Columns)
            .Where(x => x.IsVisible)
            .Where(x =>
                !(
                    x.Source.Equals(
                        "Test",
                        StringComparison.OrdinalIgnoreCase)
                    &&
                    (
                        x.FieldCode.Equals(
                            "TestName",
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        x.FieldCode.Equals(
                            "TestCode",
                            StringComparison.OrdinalIgnoreCase)
                    )
                ))
            .GroupBy(
                x => $"{x.Source}|{x.FieldCode}",
                StringComparer.OrdinalIgnoreCase)
            .Select(x => x.First())
            .OrderBy(x => x.DisplayOrder)
            .ToList();

        if (columns.Count == 0)
            return;

        column.Item()
            .PaddingTop(6)
            .PaddingBottom(4)
            .Element(
                container =>
                {
                    container
                        .Background("#17365D")
                        .PaddingVertical(4)
                        .PaddingHorizontal(7)
                        .AlignRight()
                        .Text("نتایج آزمون‌ها")
                        .Bold()
                        .FontSize(10)
                        .FontColor("#FFFFFF");
                });

        column.Item()
            .Table(
                table =>
                {
                    table.ColumnsDefinition(
                        definitions =>
                        {
                            definitions.RelativeColumn(1.5f);

                            foreach (var columnDefinition
                                     in columns)
                            {
                                definitions.RelativeColumn(
                                    (float)ParseRelativeWidth(
                                        columnDefinition.Width));
                            }
                        });

                    table.Header(
                        header =>
                        {
                            header.Cell()
                                .Element(
                                    PdfResultsHeaderCell)
                                .AlignCenter()
                                .Text("آزمون")
                                .Bold()
                                .FontSize(8)
                                .FontColor("#FFFFFF");

                            foreach (var columnDefinition
                                     in columns)
                            {
                                var cell =
    header.Cell()
        .Element(
            PdfResultsHeaderCell);

                                switch (
                                    columnDefinition.Alignment?
                                        .Trim()
                                        .ToLowerInvariant())
                                {
                                    case "left":
                                        cell.AlignLeft()
                                            .Text(
                                                columnDefinition.Caption ??
                                                columnDefinition.FieldCode)
                                            .Bold()
                                            .FontSize(8)
                                            .FontColor("#FFFFFF");
                                        break;

                                    case "center":
                                        cell.AlignCenter()
                                            .Text(
                                                columnDefinition.Caption ??
                                                columnDefinition.FieldCode)
                                            .Bold()
                                            .FontSize(8)
                                            .FontColor("#FFFFFF");
                                        break;

                                    default:
                                        cell.AlignRight()
                                            .Text(
                                                columnDefinition.Caption ??
                                                columnDefinition.FieldCode)
                                            .Bold()
                                            .FontSize(8)
                                            .FontColor("#FFFFFF");
                                        break;
                                }
                            }
                        });

                    foreach (var test in model.Tests)
                    {
                        var results =
                            test.Results.Count == 0
                                ? new List<RenderedReportResult>
                                {
                                new()
                                }
                                : test.Results
                                    .OrderBy(
                                        x => x.DisplayOrder)
                                    .ToList();

                        foreach (var result in results)
                        {
                            table.Cell()
                                .Element(
                                    PdfResultsBodyCell)
                                .AlignRight()
                                .Text(
                                    test.TestName)
                                .FontSize(8);

                            foreach (var columnDefinition
                                     in columns)
                            {
                                var field =
                                    FindResultField(
                                        test,
                                        result,
                                        columnDefinition);

                                var cell =
                                    table.Cell()
                                        .Element(
                                            PdfResultsBodyCell);

                                switch (
                                    columnDefinition.Alignment?
                                        .Trim()
                                        .ToLowerInvariant())
                                {
                                    case "left":
                                        cell.AlignLeft()
                                            .Text(
                                                FormatFieldValue(field))
                                            .FontSize(8);
                                        break;

                                    case "center":
                                        cell.AlignCenter()
                                            .Text(
                                                FormatFieldValue(field))
                                            .FontSize(8);
                                        break;

                                    default:
                                        cell.AlignRight()
                                            .Text(
                                                FormatFieldValue(field))
                                            .FontSize(8);
                                        break;
                                }


                            }
                        }
                    }
                });
    }

    // ============================================================
    // PDF CELLS / STYLE
    // ============================================================

    private static IContainer PdfCompactCell(
        IContainer container)
    {
        return container
            .BorderBottom(0.4f)
            .BorderColor("#D9E1E8")
            .PaddingVertical(2)
            .PaddingHorizontal(4);
    }

    private static IContainer PdfLabelCell(
        IContainer container)
    {
        return container
            .Background("#F3F6F9")
            .Border(0.4f)
            .BorderColor("#D2DCE5")
            .PaddingVertical(2)
            .PaddingHorizontal(4);
    }

    private static IContainer PdfValueCell(
        IContainer container)
    {
        return container
            .Border(0.4f)
            .BorderColor("#D2DCE5")
            .PaddingVertical(2)
            .PaddingHorizontal(4);
    }

    private static IContainer PdfResultsHeaderCell(
        IContainer container)
    {
        return container
            .Background("#2F75B5")
            .Border(0.5f)
            .BorderColor("#1F4E78")
            .PaddingVertical(3)
            .PaddingHorizontal(3)
            .AlignMiddle();
    }

    private static IContainer PdfResultsBodyCell(
        IContainer container)
    {
        return container
            .Border(0.4f)
            .BorderColor("#C9D4DF")
            .PaddingVertical(2)
            .PaddingHorizontal(3)
            .AlignMiddle();
    }

    // ============================================================
    // SHARED
    // ============================================================

    private static string NormalizeLayout(
        string? layout)
    {
        return layout?
            .Trim()
            .ToLowerInvariant() switch
        {
            "grid" =>
                "Grid",

            "table" =>
                "Table",

            _ =>
                "Stack"
        };
    }

    private static string FormatFieldValue(
        RenderedReportField? field)
    {
        if (field is null)
            return "";

        var value =
            field.Value;

        if (value is null)
            return "";

        if (value is DateTime dateTime)
        {
            if (!string.IsNullOrWhiteSpace(
                    field.Format))
            {
                return dateTime.ToString(
                    field.Format,
                    CultureInfo.InvariantCulture);
            }

            return dateTime.ToString(
                "yyyy/MM/dd",
                CultureInfo.InvariantCulture);
        }

        if (value is DateTimeOffset dateTimeOffset)
        {
            if (!string.IsNullOrWhiteSpace(
                    field.Format))
            {
                return dateTimeOffset.ToString(
                    field.Format,
                    CultureInfo.InvariantCulture);
            }

            return dateTimeOffset.ToString(
                "yyyy/MM/dd",
                CultureInfo.InvariantCulture);
        }

        if (value is decimal decimalValue)
        {
            if (!string.IsNullOrWhiteSpace(
                    field.Format))
            {
                return decimalValue.ToString(
                    field.Format,
                    CultureInfo.InvariantCulture);
            }

            return decimalValue.ToString(
                CultureInfo.InvariantCulture);
        }

        if (value is double doubleValue)
        {
            if (!string.IsNullOrWhiteSpace(
                    field.Format))
            {
                return doubleValue.ToString(
                    field.Format,
                    CultureInfo.InvariantCulture);
            }

            return doubleValue.ToString(
                CultureInfo.InvariantCulture);
        }

        if (value is float floatValue)
        {
            if (!string.IsNullOrWhiteSpace(
                    field.Format))
            {
                return floatValue.ToString(
                    field.Format,
                    CultureInfo.InvariantCulture);
            }

            return floatValue.ToString(
                CultureInfo.InvariantCulture);
        }

        if (value is bool boolean)
            return boolean
                ? "بله"
                : "خیر";

        return Convert.ToString(
                   value,
                   CultureInfo.InvariantCulture)
               ?? "";
    }

    private static double ParseRelativeWidth(
        string? width)
    {
        if (string.IsNullOrWhiteSpace(width))
            return 1;

        var value =
            width.Trim()
                .ToLowerInvariant();

        if (value.EndsWith("fr") &&
            double.TryParse(
                value[..^2],
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out var fr))
        {
            return Math.Max(
                0.1,
                fr);
        }

        if (double.TryParse(
                value,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out var numeric))
        {
            return Math.Max(
                0.1,
                numeric);
        }

        return 1;
    }

    private static RenderedReportField? FindResultField(
        RenderedReportTest test,
        RenderedReportResult result,
        RenderedReportField column)
    {
        var field =
            result.Fields.FirstOrDefault(
                x =>
                    x.Source.Equals(
                        column.Source,
                        StringComparison.OrdinalIgnoreCase)
                    &&
                    x.FieldCode.Equals(
                        column.FieldCode,
                        StringComparison.OrdinalIgnoreCase));

        if (field is not null)
            return field;

        return test.Fields.FirstOrDefault(
            x =>
                x.Source.Equals(
                    column.Source,
                    StringComparison.OrdinalIgnoreCase)
                &&
                x.FieldCode.Equals(
                    column.FieldCode,
                    StringComparison.OrdinalIgnoreCase));
    }
}