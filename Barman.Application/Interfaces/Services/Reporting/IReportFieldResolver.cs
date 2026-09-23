using Barman.Application.DTOs.Reporting;

namespace Barman.Application.Interfaces.Services.Reporting;

public interface IReportFieldResolver
{
    ResolvedReportField Resolve(
        string fieldCode,
        FinalReportData data,
        FinalReportTestData? test = null,
        FinalReportResultData? result = null);

    IReadOnlyList<ResolvedReportField> ResolveAll(
        IEnumerable<string> fieldCodes,
        FinalReportData data,
        FinalReportTestData? test = null,
        FinalReportResultData? result = null);
}

public sealed record ResolvedReportField(
    string FieldCode,
    string? Caption,
    string? DataType,
    object? Value,
    bool IsResolved);
