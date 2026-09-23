using Barman.Application.DTOs.Reporting;
using Barman.Application.Interfaces.Services.Reporting;
using Barman.Application.Reporting;

namespace Barman.Application.Services.Reporting;

public sealed class ReportFieldResolver : IReportFieldResolver
{
    public ResolvedReportField Resolve(
        string fieldCode,
        FinalReportData data,
        FinalReportTestData? test = null,
        FinalReportResultData? result = null)
    {
        if (string.IsNullOrWhiteSpace(fieldCode))
        {
            return Unresolved(fieldCode);
        }

        fieldCode = fieldCode.Trim();

        // ============================================
        // Custom Field
        // Format:
        // CustomField:{Guid}
        // ============================================

        if (fieldCode.StartsWith(
                "CustomField:",
                StringComparison.OrdinalIgnoreCase))
        {
            return ResolveCustomField(fieldCode, data);
        }

        var definition = ReportFieldCatalog.Find(fieldCode);

        if (definition is null)
        {
            return Unresolved(fieldCode);
        }

        var value = ResolveStandardField(
            definition.Code,
            data,
            test,
            result);

        return new ResolvedReportField(
            definition.Code,
            definition.Caption,
            definition.DataType,
            value,
            value is not null);
    }


    public IReadOnlyList<ResolvedReportField> ResolveAll(
        IEnumerable<string> fieldCodes,
        FinalReportData data,
        FinalReportTestData? test = null,
        FinalReportResultData? result = null)
    {
        if (fieldCodes is null)
            return Array.Empty<ResolvedReportField>();

        return fieldCodes
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => Resolve(
                x,
                data,
                test,
                result))
            .ToList();
    }


    private static object? ResolveStandardField(
        string fieldCode,
        FinalReportData data,
        FinalReportTestData? test,
        FinalReportResultData? result)
    {
        switch (fieldCode)
        {
            // ============================================
            // Report
            // ============================================

            case "ReportNumber":
                return data.ReportNumber;

            case "ReportVersion":
                return data.ReportVersion;

            case "IssueDate":
                return data.IssueDate;

            case "TemplateCode":
                return data.TemplateCode;

            case "TemplateVersion":
                return data.TemplateVersion;


            // ============================================
            // Laboratory
            // ============================================

            case "LabName":
                return data.Laboratory.Name;

            case "LabLogo":
                return data.Laboratory.Logo;

            case "LabAddress":
                return data.Laboratory.Address;

            case "LabPhone":
                return data.Laboratory.Phone;

            case "LabEmail":
                return data.Laboratory.Email;

            case "AccreditationNo":
                return data.Laboratory.AccreditationNo;


            // ============================================
            // Reception
            // ============================================

            case "ReceptionNumber":
                return data.ReceptionNumber;

            case "ReceptionDate":
                return data.ReceptionDate;

            case "ReceptionStatus":
                return data.ReceptionStatus;

            case "IsUrgent":
                return data.IsUrgent;

            case "ReceptionDescription":
                return data.ReceptionDescription;

            case "TotalPrice":
                return data.TotalPrice;

            case "IsPaid":
                return data.IsPaid;


            // ============================================
            // Customer
            // ============================================

            case "CustomerCode":
                return data.Customer.Code;

            case "CustomerName":
                return data.Customer.Name;

            case "CustomerNationalId":
                return data.Customer.NationalId;

            case "CustomerEconomicCode":
                return data.Customer.EconomicCode;

            case "CustomerPhone":
                return data.Customer.Phone;

            case "CustomerMobile":
                return data.Customer.Mobile;

            case "CustomerEmail":
                return data.Customer.Email;

            case "CustomerAddress":
                return data.Customer.Address;


            // ============================================
            // Sample
            // ============================================

            case "SampleCode":
                return data.Sample.Code;

            case "SampleName":
                return data.Sample.Name;

            case "CustomerSampleName":
                return data.Sample.CustomerSampleName;

            case "SampleCategory":
                return data.Sample.SampleCategory;

            case "Matrix":
                return data.Sample.Matrix;

            case "StandardSample":
                return data.Sample.StandardSample;

            case "BatchLotNumber":
                return data.Sample.BatchLotNumber;

            case "QuotaNumber":
                return data.Sample.QuotaNumber;

            case "ShipmentNumber":
                return data.Sample.ShipmentNumber;

            case "ProductionDate":
                return data.Sample.ProductionDate;

            case "ExpiryDate":
                return data.Sample.ExpiryDate;

            case "Quantity":
                return data.Sample.Quantity;

            case "SampleUnit":
                return data.Sample.Unit;

            case "ContainerType":
                return data.Sample.ContainerType;

            case "SampleDescription":
                return data.Sample.Description;


            // ============================================
            // Test
            // ============================================

            case "TestCode":
                return test?.TestCode;

            case "TestName":
                return test?.TestName;

            case "TestMethod":
            

            case "TestInstrument":
           

            case "TestEnglishName":
                return null;

            case "TestUnit":
                return test?.Unit;


            // ============================================
            // Result
            //
            // Priority:
            // 1. Explicit multi-result context
            // 2. Single-result fields on Test
            // ============================================

            case "ResultDefinitionCode":
                return result?.ResultDefinitionCode;

            case "ResultDefinitionName":
                return result?.ResultDefinitionName;

            case "Result":
                return result?.Value ?? test?.Result;

            case "Unit":
                return result?.Unit ?? test?.Unit;

            case "LOD":
                return result?.LOD ?? test?.LOD;

            case "LOQ":
                return result?.LOQ ?? test?.LOQ;

            case "Min":
                return result?.Min ?? test?.Min;

            case "Max":
                return result?.Max ?? test?.Max;

            case "ComplianceStatus":
                return GetComplianceStatus(
                    result?.IsCompliant ??
                    test?.IsCompliant);

            case "ResultComment":
                return result?.Comment ??
                       test?.Comment;


            // ============================================
            // Limit
            // ============================================

            case "PermissibleLimit":
                return test?.PermissibleLimit;

            case "LimitReference":
                return test?.LimitReference;

            case "LowerLimit":
                return result?.Min ?? test?.Min;

            case "UpperLimit":
                return result?.Max ?? test?.Max;


            // ============================================
            // Technical Information
            // ============================================

            case "Method":
                return test?.Method;

            case "Instrument":
                return test?.Instrument;

            case "SOP":
                return null;

            case "ReferenceStandard":
                return null;

            case "AnalyticalProcedure":
                return null;

            case "TechnicalNotes":
                return test?.Comment;


            // ============================================
            // Signatures
            // ============================================

            case "SectionHeadName":
                return data.Signatures.SectionHeadName;

            case "SectionHeadSignature":
                return data.Signatures.SectionHeadSignature;

            case "TechnicalManagerName":
                return data.Signatures.TechnicalManagerName;

            case "TechnicalManagerSignature":
                return data.Signatures.TechnicalManagerSignature;

            case "DirectorName":
                return data.Signatures.DirectorName;

            case "DirectorSignature":
                return data.Signatures.DirectorSignature;


            // ============================================
            // Footer
            // ============================================

            case "PageNumber":
                return null;

            case "TotalPages":
                return null;

            case "GeneratedAt":
                return null;


            // ============================================
            // Unknown
            // ============================================

            default:
                return null;
        }
    }


    private static ResolvedReportField ResolveCustomField(
        string fieldCode,
        FinalReportData data)
    {
        const string prefix = "CustomField:";

        var idText =
            fieldCode[prefix.Length..].Trim();

        if (!Guid.TryParse(
                idText,
                out var definitionId))
        {
            return Unresolved(fieldCode);
        }

        var customField =
            data.CustomFields.FirstOrDefault(
                x => x.DefinitionId == definitionId);

        if (customField is null)
        {
            return new ResolvedReportField(
                fieldCode,
                null,
                null,
                null,
                false);
        }

        return new ResolvedReportField(
            fieldCode,
            customField.Title,
            customField.DataType,
            customField.Value,
            true);
    }


    private static string? GetComplianceStatus(
        bool? isCompliant)
    {
        return isCompliant switch
        {
            true => "Compliant",
            false => "NonCompliant",
            null => null
        };
    }


    private static ResolvedReportField Unresolved(
        string? fieldCode)
    {
        return new ResolvedReportField(
            fieldCode ?? "",
            null,
            null,
            null,
            false);
    }
}