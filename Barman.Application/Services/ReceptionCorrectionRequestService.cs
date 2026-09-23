using Barman.Application.DTOs.CustomField;
using Barman.Application.DTOs.Reception;
using Barman.Application.Interfaces;
using Barman.Domain.Entities;
using Barman.Domain.Enums;

namespace Barman.Application.Services;

public class ReceptionCorrectionRequestService
    : IReceptionCorrectionRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ReceptionCorrectionRequestService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ReceptionCorrectionRequest>
        CreateRequestAsync(
            Guid receptionId,
            Guid sampleId,
            string reason)
    {
        if (receptionId == Guid.Empty)
            throw new InvalidOperationException(
                "پذیرش مشخص نشده است.");

        if (sampleId == Guid.Empty)
            throw new InvalidOperationException(
                "نمونه مشخص نشده است.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException(
                "دلیل درخواست اصلاح باید مشخص شود.");

        var currentEmployeeId =
            _currentUserService.EmployeeId;

        if (!currentEmployeeId.HasValue ||
            currentEmployeeId.Value == Guid.Empty)
        {
            throw new InvalidOperationException(
                "کاربر جاری مشخص نشده است.");
        }

        var reception = await _unitOfWork
            .Receptions
            .GetByIdAsync(receptionId);

        if (reception is null)
            throw new InvalidOperationException(
                "پذیرش موردنظر پیدا نشد.");

        var sample = await _unitOfWork
            .Samples
            .GetByIdAsync(sampleId);

        if (sample is null)
            throw new InvalidOperationException(
                "نمونه موردنظر پیدا نشد.");

        if (sample.ReceptionId != receptionId)
            throw new InvalidOperationException(
                "نمونه انتخاب‌شده متعلق به این پذیرش نیست.");

        var pendingRequest = await _unitOfWork
            .ReceptionCorrectionRequests
            .GetPendingByReceptionIdAsync(
                receptionId);

        if (pendingRequest is not null)
            throw new InvalidOperationException(
                "برای این پذیرش قبلاً یک درخواست اصلاح در انتظار بررسی وجود دارد.");

        var request = new ReceptionCorrectionRequest
        {
            ReceptionId = receptionId,
            SampleId = sampleId,
            RequestedByEmployeeId =
                currentEmployeeId.Value,
            RequestedAt =
                DateTimeOffset.UtcNow,
            Reason =
                reason.Trim(),
            Status =
                ReceptionCorrectionRequestStatus.Pending
        };

        await _unitOfWork
            .ReceptionCorrectionRequests
            .AddAsync(request);

        await _unitOfWork.SaveChangesAsync();

        return request;
    }

    public async Task<ReceptionCorrectionRequest?>
        GetByIdAsync(Guid requestId)
    {
        if (requestId == Guid.Empty)
            return null;

        return await _unitOfWork
            .ReceptionCorrectionRequests
            .GetByIdAsync(requestId);
    }

    public async Task<ReceptionCorrectionDto?>
        GetForCorrectionAsync(Guid requestId)
    {
        if (requestId == Guid.Empty)
            return null;

        var request = await _unitOfWork
            .ReceptionCorrectionRequests
            .GetByIdAsync(requestId);

        if (request is null)
            return null;

        if (!request.SampleId.HasValue ||
            request.SampleId.Value == Guid.Empty)
        {
            return null;
        }

        var reception = await _unitOfWork
            .Receptions
            .GetForCorrectionAsync(
                request.ReceptionId);

        if (reception is null)
            return null;

        var sample = reception.Samples
            .FirstOrDefault(x =>
                x.Id == request.SampleId.Value);

        if (sample is null)
            return null;

        var customFields =
          await _unitOfWork.CustomFieldValues
        .GetBySampleIdAsync(sample.Id);

        var dto = new ReceptionCorrectionDto
        {
            RequestId = request.Id,
            CorrectionReason = request.Reason,

            ReceptionId = reception.Id,
            ReceptionNumber = reception.ReceptionNumber,
            ReceptionDate = reception.ReceptionDate,
            CustomerName =
              reception.Customer?.DisplayName ?? "",

            SampleId = sample.Id,
            SampleCode = sample.SampleCode,

            SampleName = sample.SampleName,
            CustomerSampleName =
                sample.CustomerSampleName,

            ProductionDate =
                sample.ProductionDate.HasValue
                    ? sample.ProductionDate.Value
                        .ToDateTime(TimeOnly.MinValue)
                    : null,

            ExpiryDate =
                sample.ExpiryDate.HasValue
                    ? sample.ExpiryDate.Value
                        .ToDateTime(TimeOnly.MinValue)
                    : null,

            BatchLotNumber =
                sample.BatchLotNumber,

            QuotaNumber =
                sample.QuotaNumber,

            ShipmentNumber =
                sample.ShipmentNumber,

            SampleCategoryId =
                sample.SampleCategoryId,

            MatrixId =
                sample.MatrixId,

            StandardSampleId =
                sample.StandardSampleId,

            Quantity =
                sample.Quantity,

            Unit =
                sample.Unit,

            ContainerType =
                sample.ContainerType,

            Description =
                sample.Description,

            CustomFields = customFields
                .Where(x =>
                    !x.IsDeleted &&
                    x.CustomFieldDefinition != null)
                .OrderBy(x => x.CustomFieldDefinition.DisplayOrder)
                .Select(x => new CustomFieldValueDto
                            {
                    Id = x.Id,
                    DefinitionId = x.CustomFieldDefinitionId,
                    Title = x.CustomFieldDefinition.Title,
                    DataType = x.CustomFieldDefinition.DataType,
                    IsRequired = x.CustomFieldDefinition.IsRequired,
                    Value = x.Value,
                    IsAdHoc = !x.CustomFieldDefinition.IsReusable,
                    DisplayOrder = x.CustomFieldDefinition.DisplayOrder
                })
                .ToList(),


            TestIds = sample.TestAssignments
                .Where(x => x.TestId != Guid.Empty)
                .Select(x => x.TestId)
                .Distinct()
                .ToList(),

            TestPanelIds = sample.TestAssignments
                .Where(x =>
                    x.TestId != Guid.Empty &&
                    x.TestPanelId.HasValue &&
                    x.TestPanelId.Value != Guid.Empty)
                .GroupBy(x => x.TestId)
                .ToDictionary(
                    g => g.Key,
                    g => g.First().TestPanelId!.Value)
        };

        return dto;
    }

    public async Task UpdateCorrectionAsync(
    ReceptionCorrectionUpdateDto dto)
    {
        if (dto.RequestId == Guid.Empty)
            throw new InvalidOperationException(
                "درخواست اصلاح مشخص نشده است.");

        if (dto.SampleId == Guid.Empty)
            throw new InvalidOperationException(
                "نمونه مشخص نشده است.");

        var request = await _unitOfWork
            .ReceptionCorrectionRequests
            .GetByIdAsync(dto.RequestId);

        if (request is null)
            throw new InvalidOperationException(
                "درخواست اصلاح پیدا نشد.");

        if (request.Status !=
            ReceptionCorrectionRequestStatus.Pending)
        {
            throw new InvalidOperationException(
                "این درخواست اصلاح دیگر در وضعیت قابل ویرایش نیست.");
        }

        if (request.SampleId != dto.SampleId)
            throw new InvalidOperationException(
                "نمونه با درخواست اصلاح مطابقت ندارد.");

        var sample = await _unitOfWork
            .Samples
            .GetByIdAsync(dto.SampleId);

        if (sample is null)
            throw new InvalidOperationException(
                "نمونه موردنظر پیدا نشد.");

        if (sample.ReceptionId != request.ReceptionId)
            throw new InvalidOperationException(
                "نمونه متعلق به پذیرش درخواست اصلاح نیست.");

        // =========================================================
        // Update Sample
        // =========================================================

        sample.SampleName =
            dto.SampleName.Trim();

        sample.CustomerSampleName =
            string.IsNullOrWhiteSpace(dto.CustomerSampleName)
                ? null
                : dto.CustomerSampleName.Trim();

        sample.ProductionDate =
            dto.ProductionDate.HasValue
                ? DateOnly.FromDateTime(dto.ProductionDate.Value)
                : null;

        sample.ExpiryDate =
            dto.ExpiryDate.HasValue
                ? DateOnly.FromDateTime(dto.ExpiryDate.Value)
                : null;

        sample.BatchLotNumber =
            string.IsNullOrWhiteSpace(dto.BatchLotNumber)
                ? null
                : dto.BatchLotNumber.Trim();

        sample.QuotaNumber =
            string.IsNullOrWhiteSpace(dto.QuotaNumber)
                ? null
                : dto.QuotaNumber.Trim();

        sample.ShipmentNumber =
            string.IsNullOrWhiteSpace(dto.ShipmentNumber)
                ? null
                : dto.ShipmentNumber.Trim();

        sample.SampleCategoryId =
            dto.SampleCategoryId;

        sample.MatrixId =
            dto.MatrixId;

        sample.StandardSampleId =
            dto.StandardSampleId;

        sample.Quantity =
            dto.Quantity;

        sample.Unit =
            string.IsNullOrWhiteSpace(dto.Unit)
                ? null
                : dto.Unit.Trim();

        sample.ContainerType =
            string.IsNullOrWhiteSpace(dto.ContainerType)
                ? null
                : dto.ContainerType.Trim();

        sample.Description =
            string.IsNullOrWhiteSpace(dto.Description)
                ? null
                : dto.Description.Trim();

        _unitOfWork.Samples.Update(sample);

        // =========================================================
        // Update Test Assignments
        // =========================================================

        var existingAssignments =
            await _unitOfWork.TestAssignments
                .GetBySampleIdAsync(sample.Id);

        var requestedTestIds =
            dto.TestIds
                .Where(x => x != Guid.Empty)
                .Distinct()
                .ToHashSet();

        foreach (var assignment in existingAssignments)
        {
            if (!requestedTestIds.Contains(assignment.TestId))
            {
                _unitOfWork.TestAssignments.Delete(
                    assignment);
            }
        }

        var existingTestIds =
            existingAssignments
                .Where(x =>
                    requestedTestIds.Contains(x.TestId))
                .Select(x => x.TestId)
                .ToHashSet();

        foreach (var testId in requestedTestIds)
        {
            if (existingTestIds.Contains(testId))
                continue;

            var test =
                await _unitOfWork.Tests
                    .GetByIdAsync(testId);

            if (test is null)
                continue;

            Guid? testPanelId = null;

            if (dto.TestPanelIds.TryGetValue(
                    testId,
                    out var panelId) &&
                panelId != Guid.Empty)
            {
                testPanelId = panelId;
            }

            var applicableRules =
    await _unitOfWork.TestLimitRules
        .GetApplicableAsync(
            test.Id,
            request.Reception.CustomerId,
            sample.MatrixId,
            sample.SampleCategoryId);

            var selectedRule =
                applicableRules.FirstOrDefault();

            var testResultSet =
                await _unitOfWork.TestResultSets.ResolveAsync(
                    test.Id,
                    request.Reception.CustomerId,
                    sample.SampleCategoryId,
                    sample.MatrixId,
                    sample.StandardSampleId,
                    CancellationToken.None);

            Guid? defaultAnalystId = test.DefaultAnalystId;

            if (testPanelId.HasValue &&
                testPanelId.Value != Guid.Empty)
            {
                var panelItem =
                    await _unitOfWork.TestPanelItems
                        .GetByPanelAndTestAsync(
                            testPanelId.Value,
                            test.Id,
                            CancellationToken.None);

                defaultAnalystId =
                    panelItem?.DefaultAnalystId
                    ?? test.DefaultAnalystId;
            }


            var assignment = new TestAssignment
            {
                SampleId = sample.Id,
                TestId = test.Id,
                TestPanelId = testPanelId,
                AnalystId = defaultAnalystId,

                TestResultSetId = testResultSet?.Id,
                TestResultSet = testResultSet,

                Unit = test.Unit,

                WorkflowStage =
                    TestAssignmentWorkflowStage.TechnicalManagerAssignment,

                DepartmentId = test.DepartmentId,

                SelectedLimitRuleId =
                    selectedRule?.Id,

                SelectedLimitRule =
                    selectedRule,

                IsApprovedBySection = false,
                IsRejectedBySection = false,
                IsApprovedByTechManager = false,
                IsApprovedByDirector = false
            };

            await _unitOfWork.TestAssignments
                .AddAsync(assignment);
        }

        // =========================================================
        // Update Panel Mapping for Existing Assignments
        // =========================================================

        foreach (var assignment in existingAssignments)
        {
            if (!requestedTestIds.Contains(assignment.TestId))
                continue;

            if (dto.TestPanelIds.TryGetValue(
                    assignment.TestId,
                    out var panelId) &&
                panelId != Guid.Empty)
            {
                assignment.TestPanelId = panelId;
            }
            else
            {
                assignment.TestPanelId = null;
            }

            assignment.WorkflowStage =
                TestAssignmentWorkflowStage.TechnicalManagerAssignment;

            assignment.IsApprovedBySection = false;
            assignment.IsRejectedBySection = false;
            assignment.IsApprovedByTechManager = false;
            assignment.IsApprovedByDirector = false;

            _unitOfWork.TestAssignments
                .Update(assignment);
        }

        // =========================================================
        // Custom Fields
        // =========================================================

        var existingCustomFields =
            await _unitOfWork.CustomFieldValues
                .GetBySampleIdAsync(sample.Id);

        var incomingCustomFields =
            dto.CustomFields ?? new List<CustomFieldValueDto>();

        foreach (var existingValue in existingCustomFields)
        {
            var incoming =
                incomingCustomFields.FirstOrDefault(x =>
                    x.Id.HasValue &&
                    x.Id.Value == existingValue.Id);

            if (incoming is null)
            {
                _unitOfWork.CustomFieldValues
                    .Delete(existingValue);

                continue;
            }

            existingValue.Value =
                string.IsNullOrWhiteSpace(incoming.Value)
                    ? null
                    : incoming.Value.Trim();

            _unitOfWork.CustomFieldValues
                .Update(existingValue);
        }

        foreach (var fieldDto in incomingCustomFields)
        {
            if (!fieldDto.Id.HasValue &&
                fieldDto.IsAdHoc &&
                !string.IsNullOrWhiteSpace(fieldDto.Title))
            {
                var definition = new CustomFieldDefinition
                {
                    Title = fieldDto.Title.Trim(),
                    DataType =
                        string.IsNullOrWhiteSpace(fieldDto.DataType)
                            ? "Text"
                            : fieldDto.DataType,
                    IsRequired = fieldDto.IsRequired,
                    IsActive = true,
                    IsReusable = false,
                    DisplayOrder = fieldDto.DisplayOrder
                };

                await _unitOfWork.CustomFieldDefinitions
                    .AddAsync(definition);

                var value = new CustomFieldValue
                {
                    SampleId = sample.Id,
                    CustomFieldDefinitionId =
                        definition.Id,
                    Value =
                        string.IsNullOrWhiteSpace(fieldDto.Value)
                            ? null
                            : fieldDto.Value.Trim()
                };

                await _unitOfWork.CustomFieldValues
                    .AddAsync(value);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task<List<ReceptionCorrectionRequest>>
    GetPendingAsync()
    {
        return await _unitOfWork
            .ReceptionCorrectionRequests
            .GetPendingAsync();
    }

    public async Task<List<ReceptionCorrectionRequest>>
        GetByReceptionIdAsync(Guid receptionId)
    {
        if (receptionId == Guid.Empty)
            return new List<ReceptionCorrectionRequest>();

        return await _unitOfWork
            .ReceptionCorrectionRequests
            .GetByReceptionIdAsync(
                receptionId);
    }

    public async Task<ReceptionCorrectionRequest?>
        GetPendingByReceptionIdAsync(
            Guid receptionId)
    {
        if (receptionId == Guid.Empty)
            return null;

        return await _unitOfWork
            .ReceptionCorrectionRequests
            .GetPendingByReceptionIdAsync(
                receptionId);
    }

    public async Task ResolveAsync(Guid requestId)
    {
        if (requestId == Guid.Empty)
            throw new InvalidOperationException(
                "درخواست اصلاح مشخص نشده است.");

        var currentEmployeeId =
            _currentUserService.EmployeeId;

        if (!currentEmployeeId.HasValue ||
            currentEmployeeId.Value == Guid.Empty)
        {
            throw new InvalidOperationException(
                "کاربر جاری مشخص نشده است.");
        }

        var request = await _unitOfWork
            .ReceptionCorrectionRequests
            .GetByIdAsync(requestId);

        if (request is null)
            throw new InvalidOperationException(
                "درخواست اصلاح موردنظر پیدا نشد.");

        if (request.Status !=
            ReceptionCorrectionRequestStatus.Pending)
        {
            throw new InvalidOperationException(
                "این درخواست قبلاً بررسی شده است.");
        }

        request.Status =
            ReceptionCorrectionRequestStatus.Resolved;

        request.ResolvedByEmployeeId =
            currentEmployeeId.Value;

        request.ResolvedAt =
            DateTimeOffset.UtcNow;

        _unitOfWork
            .ReceptionCorrectionRequests
            .Update(request);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CancelAsync(Guid requestId)
    {
        if (requestId == Guid.Empty)
            throw new InvalidOperationException(
                "درخواست اصلاح مشخص نشده است.");

        var currentEmployeeId =
            _currentUserService.EmployeeId;

        if (!currentEmployeeId.HasValue ||
            currentEmployeeId.Value == Guid.Empty)
        {
            throw new InvalidOperationException(
                "کاربر جاری مشخص نشده است.");
        }

        var request = await _unitOfWork
            .ReceptionCorrectionRequests
            .GetByIdAsync(requestId);

        if (request is null)
            throw new InvalidOperationException(
                "درخواست اصلاح موردنظر پیدا نشد.");

        if (request.Status !=
            ReceptionCorrectionRequestStatus.Pending)
        {
            throw new InvalidOperationException(
                "این درخواست قبلاً بررسی شده است.");
        }

        request.Status =
            ReceptionCorrectionRequestStatus.Cancelled;

        request.ResolvedByEmployeeId =
            currentEmployeeId.Value;

        request.ResolvedAt =
            DateTimeOffset.UtcNow;

        _unitOfWork
            .ReceptionCorrectionRequests
            .Update(request);

        await _unitOfWork.SaveChangesAsync();
    }
}