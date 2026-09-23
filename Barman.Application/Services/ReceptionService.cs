using Barman.Application.DTOs.Reception;
using Barman.Application.Interfaces;
using Barman.Domain.Entities;
using Barman.Domain.Enums;

namespace Barman.Application.Services;

public class ReceptionService : IReceptionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INumberGenerator _numberGenerator;
    private readonly ITechnicalManagerRoutingService _technicalManagerRoutingService;

    public ReceptionService(
        IUnitOfWork unitOfWork,
        INumberGenerator numberGenerator,
        ITechnicalManagerRoutingService technicalManagerRoutingService)
    {
        _unitOfWork = unitOfWork;
        _numberGenerator = numberGenerator;
        _technicalManagerRoutingService = technicalManagerRoutingService;
    }

    public async Task<Reception> CreateReceptionAsync(
        CreateReceptionDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.CustomerId == Guid.Empty)
            throw new ArgumentException("Customer is required.");

        var validSamples = dto.Samples
            .Where(x => !string.IsNullOrWhiteSpace(x.SampleName))
            .ToList();

        if (validSamples.Count == 0)
            throw new ArgumentException("At least one sample is required.");

        var receptionNumber =
            await _numberGenerator.GenerateAsync(
                "Reception",
                cancellationToken);

        var reception = new Reception
        {
            ReceptionNumber = receptionNumber,
            CustomerId = dto.CustomerId,
            ReceptionDate = dto.ReceptionDate.ToUniversalTime(),
            IsUrgent = dto.IsUrgent,
            Description = dto.Description,
            Status = Domain.Enums.ReceptionStatus.Draft,
            IsPaid = false
        };

        await _unitOfWork.Receptions.AddAsync(reception);

        var sampleIndex = 0;

        foreach (var sampleDto in validSamples)
        {
            sampleIndex++;

            var sampleCode =
                $"{receptionNumber}-S{sampleIndex:00}";

            var sample = new Sample
            {
                ReceptionId = reception.Id,

                SampleCode = sampleCode,

                SampleName = sampleDto.SampleName.Trim(),

                CustomerSampleName = string.IsNullOrWhiteSpace(sampleDto.CustomerSampleName)
                    ? null
                    : sampleDto.CustomerSampleName.Trim(),

                ProductionDate = sampleDto.ProductionDate.HasValue
                    ? DateOnly.FromDateTime(sampleDto.ProductionDate.Value)
                    : null,

                ExpiryDate = sampleDto.ExpiryDate.HasValue
                    ? DateOnly.FromDateTime(sampleDto.ExpiryDate.Value)
                    : null,

                BatchLotNumber = string.IsNullOrWhiteSpace(sampleDto.BatchLotNumber)
                    ? null
                    : sampleDto.BatchLotNumber.Trim(),

                QuotaNumber = string.IsNullOrWhiteSpace(sampleDto.QuotaNumber)
                    ? null
                    : sampleDto.QuotaNumber.Trim(),

                ShipmentNumber = string.IsNullOrWhiteSpace(sampleDto.ShipmentNumber)
                    ? null
                    : sampleDto.ShipmentNumber.Trim(),

                SampleCategoryId = sampleDto.SampleCategoryId,

                MatrixId = sampleDto.MatrixId,

                StandardSampleId = sampleDto.StandardSampleId,

                Quantity = sampleDto.Quantity,

                Unit = string.IsNullOrWhiteSpace(sampleDto.Unit)
                    ? null
                    : sampleDto.Unit.Trim(),

                ContainerType = string.IsNullOrWhiteSpace(sampleDto.ContainerType)
                    ? null
                    : sampleDto.ContainerType.Trim(),

                Description = string.IsNullOrWhiteSpace(sampleDto.Description)
                    ? null
                    : sampleDto.Description.Trim()
            };

            await _unitOfWork.Samples.AddAsync(sample);

            // Save Custom Fields
            if (sampleDto.CustomFields != null &&
                sampleDto.CustomFields.Count > 0)
            {
                foreach (var fieldDto in sampleDto.CustomFields)
                {
                    if (fieldDto.IsAdHoc)
                    {
                        if (string.IsNullOrWhiteSpace(fieldDto.Title))
                            continue;

                        var definition = new CustomFieldDefinition
                        {
                            Title = fieldDto.Title.Trim(),
                            DataType = string.IsNullOrWhiteSpace(fieldDto.DataType)
                                ? "Text"
                                : fieldDto.DataType,
                            IsRequired = fieldDto.IsRequired,
                            IsActive = true,
                            IsReusable = false,
                            DisplayOrder = fieldDto.DisplayOrder
                        };

                        await _unitOfWork.CustomFieldDefinitions
                            .AddAsync(definition);

                        var fieldValue = new CustomFieldValue
                        {
                            SampleId = sample.Id,
                            CustomFieldDefinitionId = definition.Id,
                            Value = string.IsNullOrWhiteSpace(fieldDto.Value)
                                ? null
                                : fieldDto.Value.Trim()
                        };

                        await _unitOfWork.CustomFieldValues
                            .AddAsync(fieldValue);
                    }
                    else
                    {
                        if (!fieldDto.DefinitionId.HasValue ||
                            fieldDto.DefinitionId.Value == Guid.Empty ||
                            string.IsNullOrWhiteSpace(fieldDto.Value))
                            continue;

                        var fieldValue = new CustomFieldValue
                        {
                            SampleId = sample.Id,
                            CustomFieldDefinitionId = fieldDto.DefinitionId.Value,
                            Value = fieldDto.Value.Trim()
                        };

                        await _unitOfWork.CustomFieldValues
                            .AddAsync(fieldValue);
                    }
                }
            }

            // TestAssignment
            var testIds = sampleDto.TestIds
                .Where(x => x != Guid.Empty)
                .Distinct()
                .ToList();

            Console.WriteLine(
                $"[PANEL DEBUG] Sample = {sampleDto.SampleName}");

            Console.WriteLine(
                $"[PANEL DEBUG] TestPanelIds Count = {sampleDto.TestPanelIds.Count}");

            foreach (var panelMapping in sampleDto.TestPanelIds)
            {
                Console.WriteLine(
                    $"[PANEL DEBUG] TestId = {panelMapping.Key}, PanelId = {panelMapping.Value}");
            }

            foreach (var testId in testIds)
            {
                var test = await _unitOfWork.Tests
                    .GetByIdAsync(testId);

                Console.WriteLine(
                    $"[DEPARTMENT DEBUG] TestId = {testId}");

                Console.WriteLine(
                    $"[DEPARTMENT DEBUG] Test.DepartmentId = {test?.DepartmentId}");

                if (test == null)
                    continue;

                Console.WriteLine(
                    $"[DEPARTMENT DEBUG] Test={test.Code} TestId={test.Id} DepartmentId={test.DepartmentId}");

                Console.WriteLine(
                    $"[DEPARTMENT DEBUG] DepartmentName={test.Department?.Name}");

                Console.WriteLine($"[LIMIT DEBUG] TestId = {test.Id}");
                Console.WriteLine($"[LIMIT DEBUG] CustomerId = {dto.CustomerId}");
                Console.WriteLine($"[LIMIT DEBUG] MatrixId = {sample.MatrixId}");
                Console.WriteLine($"[LIMIT DEBUG] SampleCategoryId = {sample.SampleCategoryId}");

                var applicableRules =
                    await _unitOfWork.TestLimitRules
                        .GetApplicableAsync(
                            test.Id,
                            dto.CustomerId,
                            sample.MatrixId,
                            sample.SampleCategoryId);

                var selectedRule = applicableRules.FirstOrDefault();

                Console.WriteLine(
                    $"[LIMIT DEBUG] ApplicableRules Count = {applicableRules.Count}");

                Guid? testPanelId = null;

                if (sampleDto.TestPanelIds.TryGetValue(
                        testId,
                        out var panelId) &&
                    panelId != Guid.Empty)
                {
                    testPanelId = panelId;
                }

                Console.WriteLine(
                    $"[DEPARTMENT DEBUG] Test = {test.Code} | TestId = {test.Id} | Test.DepartmentId = {test.DepartmentId}");

                Console.WriteLine(
                    $"[DEPARTMENT DEBUG] Assignment DepartmentId = {test.DepartmentId}");

                var routingResult =
                        await _technicalManagerRoutingService.ResolveAsync(
                            dto.CustomerId,
                            sample.SampleCategoryId,
                            sample.MatrixId,
                            testPanelId,
                            test.Id,
                            test.DepartmentId,
                            cancellationToken);

                var testResultSet =
                    await _unitOfWork.TestResultSets.ResolveAsync(
                        test.Id,
                        dto.CustomerId,
                        sample.SampleCategoryId,
                        sample.MatrixId,
                        sample.StandardSampleId,
                        cancellationToken);


                

                Guid? defaultAnalystId = test.DefaultAnalystId;

                if (testPanelId.HasValue &&
                    testPanelId.Value != Guid.Empty)
                {
                    var panelItem =
                        await _unitOfWork.TestPanelItems
                            .GetByPanelAndTestAsync(
                                testPanelId.Value,
                                test.Id,
                                cancellationToken);

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

                    // Default department comes from the Test.
                    // Technical Manager can change it later if needed.
                    DepartmentId = test.DepartmentId,

                    TechnicalManagerId =
                          routingResult?.TechnicalManagerId,

                    TechnicalManagerScopeId =
                          routingResult?.TechnicalManagerScopeId,

                    SelectedLimitRuleId = selectedRule?.Id,
                    SelectedLimitRule = selectedRule,

                    IsApprovedBySection = false,
                    IsApprovedByTechManager = false,
                    IsApprovedByDirector = false
                };

                await _unitOfWork.TestAssignments
                    .AddAsync(assignment);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return reception;
    }

    public async Task<Reception?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        return await _unitOfWork.Receptions
            .GetByIdAsync(id, cancellationToken);
    }

    public async Task<Reception?> GetForCorrectionAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        return await _unitOfWork.Receptions
            .GetForCorrectionAsync(id, cancellationToken);
    }

    public async Task<List<Reception>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Receptions
            .GetAllAsync(cancellationToken);
    }

    public async Task<List<ReceptionHistoryLookupDto>> GetHistoryByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        if (customerId == Guid.Empty)
            return new List<ReceptionHistoryLookupDto>();

        var receptions =
            await _unitOfWork.Receptions
                .GetHistoryByCustomerIdAsync(
                    customerId,
                    cancellationToken);

        return receptions
            .Select(reception => new ReceptionHistoryLookupDto
            {
                Id = reception.Id,
                ReceptionNumber = reception.ReceptionNumber,
                ReceptionDate = reception.ReceptionDate,
                CustomerId = reception.CustomerId,
                CustomerName = reception.Customer?.DisplayName ?? "",

                Samples = reception.Samples
                    .Select(sample => new ReceptionHistorySampleDto
                    {
                        Id = sample.Id,
                        SampleCode = sample.SampleCode,
                        SampleName = sample.SampleName,
                        CustomerSampleName = sample.CustomerSampleName,

                        Tests = sample.TestAssignments
                            .Select(assignment => new ReceptionHistoryTestDto
                            {
                                TestId = assignment.TestId,
                                TestCode = assignment.Test.Code,
                                TestName = assignment.Test.Name,

                                TestPanelId = assignment.TestPanelId,
                                TestPanelCode = assignment.TestPanel?.Code,
                                TestPanelName = assignment.TestPanel?.Name
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .ToList();
    }
}