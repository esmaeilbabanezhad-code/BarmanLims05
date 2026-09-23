using Barman.Application.DTOs.Reporting;
using Barman.Application.Interfaces;
using Barman.Application.Interfaces.Services.Reporting;

namespace Barman.Application.Services.Reporting;

public class FinalReportDataService : IFinalReportDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public FinalReportDataService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FinalReportData?> GetByReceptionIdAsync(
        Guid receptionId,
        CancellationToken cancellationToken = default)
    {
        if (receptionId == Guid.Empty)
            return null;

        var assignments =
            await _unitOfWork.TestAssignments
                .GetForFinalReportByReceptionIdAsync(receptionId);

        return await BuildReportAsync(
            assignments,
            cancellationToken);
    }

    public async Task<FinalReportData?> GetBySampleIdAsync(
        Guid sampleId,
        CancellationToken cancellationToken = default)
    {
        if (sampleId == Guid.Empty)
            return null;

        var assignments =
            await _unitOfWork.TestAssignments
                .GetForFinalReportBySampleIdAsync(sampleId);

        return await BuildReportAsync(
            assignments,
            cancellationToken);
    }

    private async Task<FinalReportData?> BuildReportAsync(
        List<Barman.Domain.Entities.TestAssignment> assignments,
        CancellationToken cancellationToken = default)
    {
        if (assignments.Count == 0)
            return null;

        var firstAssignment = assignments.First();

        var reception = firstAssignment.Sample?.Reception;

        if (reception is null)
            return null;

        // ============================================================
        // Report
        // ============================================================

        var report = new FinalReportData
        {
            ReceptionId = reception.Id,

            ReceptionNumber = reception.ReceptionNumber,

            ReceptionDate = reception.ReceptionDate,

            ReceptionStatus = reception.Status.ToString(),

            IsUrgent = reception.IsUrgent,

            ReceptionDescription = reception.Description,

            TotalPrice = reception.TotalPrice,

            IsPaid = reception.IsPaid,

            // فعلاً شماره گزارش بر اساس شماره پذیرش است.
            // بعداً در مرحله Report Numbering مستقل می‌شود.
            ReportNumber = reception.ReceptionNumber,

            ReportVersion = "1.0",

            IssueDate = DateTime.Now,

            // TemplateCode و TemplateVersion
            // در زمان Render از خود ReportTemplate تعیین می‌شوند.
            TemplateCode = null,

            TemplateVersion = null
        };

        // ============================================================
        // Laboratory
        // ============================================================

        report.Laboratory = new ReportLaboratoryData
        {
            Name = "سلامت آزمای بارمان ایرانیان",

            // این مقادیر فعلاً از Entity/Setting مشخصی در
            // FinalReportDataService قابل دریافت نیستند.
            Logo = null,

            Address = null,

            Phone = null,

            Email = null,

            AccreditationNo = null
        };

        // ============================================================
        // Customer
        // ============================================================

        if (reception.Customer is not null)
        {
            report.Customer = new ReportCustomerData
            {
                Id = reception.Customer.Id,

                Code = reception.Customer.Code,

                Name = reception.Customer.DisplayName,

                NationalId = reception.Customer.NationalId,

                EconomicCode = reception.Customer.EconomicCode,

                Phone = reception.Customer.Phone,

                Mobile = reception.Customer.Mobile,

                Email = reception.Customer.Email,

                Address = reception.Customer.Address
            };
        }

        // ============================================================
        // Sample
        // ============================================================

        var sample = firstAssignment.Sample;

        if (sample is not null)
        {
            report.Sample = new ReportSampleData
            {
                Id = sample.Id,

                Code = sample.SampleCode,

                Name = sample.SampleName,

                CustomerSampleName = sample.CustomerSampleName,

                SampleCategory = sample.SampleCategory?.Name,

                Matrix = sample.Matrix?.Name,

                StandardSample = sample.StandardSample?.Name,

                BatchLotNumber = sample.BatchLotNumber,

                QuotaNumber = sample.QuotaNumber,

                ShipmentNumber = sample.ShipmentNumber,

                ProductionDate = sample.ProductionDate,

                ExpiryDate = sample.ExpiryDate,

                Quantity = sample.Quantity,

                Unit = sample.Unit,

                ContainerType = sample.ContainerType,

                Description = sample.Description
            };

            // ========================================================
            // Sample Custom Fields
            // ========================================================

            foreach (var value in sample.CustomFieldValues
                         .Where(x => x.CustomFieldDefinition is not null)
                         .OrderBy(x => x.CustomFieldDefinition.DisplayOrder)
                         .ThenBy(x => x.CustomFieldDefinition.Title))
            {
                report.CustomFields.Add(
                    new ReportCustomFieldData
                    {
                        DefinitionId =
                            value.CustomFieldDefinitionId,

                        FieldCode =
                            $"CustomField:{value.CustomFieldDefinitionId}",

                        Title =
                            value.CustomFieldDefinition.Title,

                        DataType =
                            value.CustomFieldDefinition.DataType,

                        Value =
                            value.Value
                    });
            }
        }

        // ============================================================
        // Tests
        // ============================================================

        foreach (var assignment in assignments
                     .OrderBy(x => x.Sample.SampleCode)
                     .ThenBy(x => x.TestPanelId)
                     .ThenBy(x => x.Test.Code))
        {
            var test = assignment.Test;

            if (test is null)
                continue;

            var testData = new FinalReportTestData
            {
                AssignmentId = assignment.Id,

                TestId = assignment.TestId,

                TestCode = test.Code,

                TestName = test.Name,

                Method = test.TestMethod?.Name,

                Instrument = test.Instrument?.Name,

                Result =
                    assignment.FinalResult ??
                    assignment.Result,

                Unit =
                    assignment.Unit ??
                    test.Unit,

                Comment =
                    assignment.Comment
            };

            // ========================================================
            // Selected Limit Rule
            // ========================================================

            if (assignment.SelectedLimitRule is not null)
            {
                var rule = assignment.SelectedLimitRule;

                testData.Min =
                    rule.LowerValue;

                testData.Max =
                    rule.UpperValue;

                testData.PermissibleLimit =
                    rule.LimitType.ToString();

                testData.IsCompliant =
                    assignment.FinalResult is not null ||
                    assignment.Result is not null
                        ? !IsOutOfLimit(
                            assignment,
                            rule.LowerValue,
                            rule.UpperValue,
                            rule.LowerInclusive,
                            rule.UpperInclusive,
                            rule.LimitType)
                        : null;
            }

            // ========================================================
            // Result Values
            // ========================================================

            if (assignment.ResultValues.Count > 0)
            {
                foreach (var value in assignment.ResultValues
                             .Where(x =>
                                 x.TestResultSetItem is not null &&
                                 x.TestResultSetItem.TestResultDefinition is not null)
                             .OrderBy(x =>
                                 x.TestResultSetItem.DisplayOrder))
                {
                    var item =
                        value.TestResultSetItem;

                    var definition =
                        item.TestResultDefinition;

                    testData.Results.Add(
                        new FinalReportResultData
                        {
                            ResultValueId =
                                value.Id,

                            ResultDefinitionId =
                                item.TestResultDefinitionId,

                            ResultDefinitionCode =
                                definition.Code,

                            ResultDefinitionName =
                                definition.Name,

                            DisplayOrder =
                                item.DisplayOrder,

                            Value =
                                value.Value,

                            Unit =
                                definition.Unit ??
                                assignment.Unit,

                            Comment =
                                value.Comment,

                            LOD =
                                item.LOD,

                            LOQ =
                                item.LOQ,

                            Min =
                                item.MinValue,

                            Max =
                                item.MaxValue,

                            IsCompliant =
                                EvaluateCompliance(
                                    value.Value,
                                    item.MinValue,
                                    item.MaxValue)
                        });
                }
            }

            // ========================================================
            // Result Definitions Without Saved Values
            // ========================================================

            else if (assignment.TestResultSet?.Items is not null &&
                     assignment.TestResultSet.Items.Count > 0)
            {
                foreach (var item in assignment.TestResultSet.Items
                             .OrderBy(x => x.DisplayOrder))
                {
                    var definition =
                        item.TestResultDefinition;

                    if (definition is null)
                        continue;

                    testData.Results.Add(
                        new FinalReportResultData
                        {
                            ResultValueId =
                                Guid.Empty,

                            ResultDefinitionId =
                                item.TestResultDefinitionId,

                            ResultDefinitionCode =
                                definition.Code,

                            ResultDefinitionName =
                                definition.Name,

                            DisplayOrder =
                                item.DisplayOrder,

                            Value = null,

                            Unit =
                                definition.Unit ??
                                assignment.Unit,

                            Comment = null,

                            LOD =
                                item.LOD,

                            LOQ =
                                item.LOQ,

                            Min =
                                item.MinValue,

                            Max =
                                item.MaxValue,

                            IsCompliant = null
                        });
                }
            }

            // ========================================================
            // Add Test
            // ========================================================

            report.Tests.Add(testData);
        }

        // ============================================================
        // Signatures - Technical Manager / Section Head
        // ============================================================

        var technicalManagerAssignment = assignments
            .Where(x =>
                x.TechnicalManagerId.HasValue &&
                x.TechnicalManager != null)
            .OrderBy(x => x.Sample.SampleCode)
            .ThenBy(x => x.TestPanelId)
            .ThenBy(x => x.Test.Code)
            .FirstOrDefault();

        string? technicalManagerName =
            technicalManagerAssignment?.TechnicalManager?.FullName;

        string? sectionHeadName = null;

        if (technicalManagerAssignment?.TechnicalManagerId
            is Guid technicalManagerId)
        {
            var sectionHeadRelations =
                await _unitOfWork.TechnicalManagerSectionHeads
                    .GetByTechnicalManagerIdAsync(
                        technicalManagerId);

            var activeRelation = sectionHeadRelations
                .Where(x =>
                    x.IsActive &&
                    !x.IsDeleted &&
                    x.SectionHead != null)
                .FirstOrDefault();

            sectionHeadName =
                activeRelation?.SectionHead?.FullName;
        }

        report.Signatures = new ReportSignatureData
        {
            SectionHeadName = sectionHeadName,

            TechnicalManagerName = technicalManagerName,

            DirectorName = null,

            SectionHeadSignature = null,

            TechnicalManagerSignature = null,

            DirectorSignature = null
        };

        return report;
    }

    // ================================================================
    // Compliance
    // ================================================================

    private static bool? EvaluateCompliance(
        string? value,
        decimal? min,
        decimal? max)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (!decimal.TryParse(value, out var numericValue))
            return null;

        if (min.HasValue &&
            numericValue < min.Value)
        {
            return false;
        }

        if (max.HasValue &&
            numericValue > max.Value)
        {
            return false;
        }

        if (!min.HasValue &&
            !max.HasValue)
        {
            return null;
        }

        return true;
    }

    // ================================================================
    // Limit Rule Evaluation
    // ================================================================

    private static bool IsOutOfLimit(
        Barman.Domain.Entities.TestAssignment assignment,
        decimal? lower,
        decimal? upper,
        bool lowerInclusive,
        bool upperInclusive,
        Barman.Domain.Enums.LimitType limitType)
    {
        var result =
            assignment.FinalResult ??
            assignment.Result;

        if (!decimal.TryParse(
                result,
                out var value))
        {
            return false;
        }

        return limitType switch
        {
            Barman.Domain.Enums.LimitType.Maximum =>
                upper.HasValue &&
                (
                    upperInclusive
                        ? value > upper.Value
                        : value >= upper.Value
                ),

            Barman.Domain.Enums.LimitType.Minimum =>
                lower.HasValue &&
                (
                    lowerInclusive
                        ? value < lower.Value
                        : value <= lower.Value
                ),

            Barman.Domain.Enums.LimitType.Range =>
                (
                    lower.HasValue &&
                    (
                        lowerInclusive
                            ? value < lower.Value
                            : value <= lower.Value
                    )
                )
                ||
                (
                    upper.HasValue &&
                    (
                        upperInclusive
                            ? value > upper.Value
                            : value >= upper.Value
                    )
                ),

            Barman.Domain.Enums.LimitType.Exact =>
                lower.HasValue &&
                value != lower.Value,

            _ => false
        };
    }
}