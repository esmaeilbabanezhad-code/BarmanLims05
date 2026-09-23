using Barman.Application.DTOs.DefaultTestSet;
using Barman.Application.DTOs.Excel.MasterDataImport;
using Barman.Application.DTOs.TestTariff;
using Barman.Application.Interfaces;
using Barman.Application.Interfaces.Excel;
using Barman.Domain.Entities;
using Barman.Domain.Enums;
using ClosedXML.Excel;
using System.Globalization;
using System.Text.RegularExpressions;


namespace Barman.Infrastructure.Services.Excel;

public class ExcelImportService : IExcelImportService
{

    private readonly IUnitOfWork _unitOfWork;

    public ExcelImportService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MasterDataImportResultDto> ImportBarmanMasterDataAsync(
    byte[] fileBytes,
    CancellationToken cancellationToken = default)
    {
        if (fileBytes == null || fileBytes.Length == 0)
            throw new ArgumentException(
                "فایل Excel خالی است.",
                nameof(fileBytes));

        cancellationToken.ThrowIfCancellationRequested();

        using var stream = new MemoryStream(fileBytes);

        using var workbook = new XLWorkbook(stream);

        var result = new MasterDataImportResultDto
        {
            IsValid = true
        };

        var expectedSheets = new[]
        {
        "Tests",
        "TestTechnicalInformation",
        "TestSpecifications",
        "ResultDefinitions",
        "TestPanels",
        "PanelItems",
        "DefaultTestSets",
        "Customers",
        "SampleCategories",
        "Matrices",
        "StandardSamples",
        "Employees",
        "Departments",
        "Roles",
        "DepartmentResponsibilities",
        "ResponsibilityTypes",
        "Instruments",
        "LaboratoryTools",
        "CalibrationPoints",
        "CalibrationCertificates",
        "InventoryItems",
        "LimitReferences",
        "TestLimitRules",
        "TestMethods",
        "Lookup"
    };

        foreach (var sheetName in expectedSheets)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var sheetResult = new MasterDataImportSheetResultDto
            {
                SheetName = sheetName
            };

            if (!workbook.Worksheets.Contains(sheetName))
            {
                sheetResult.ErrorCount = 1;

                sheetResult.Rows.Add(
                    new MasterDataImportRowResultDto
                    {
                        SheetName = sheetName,
                        RowNumber = 0,
                        Action = "Error",
                        IsValid = false,
                        Errors =
                        {
                        $"Sheet '{sheetName}' در فایل Excel پیدا نشد."
                        }
                    });

                result.IsValid = false;
                result.ErrorCount++;
            }

            result.Sheets.Add(sheetResult);
        }
        await ImportDepartmentsAsync(
    workbook.Worksheet("Departments"),
    result,
    cancellationToken);

        await ImportRolesAsync(
    workbook.Worksheet("Roles"),
    result,
    cancellationToken);

        await ImportEmployeesAsync(
    workbook.Worksheet("Employees"),
    result,
    cancellationToken);


        await ImportCustomersAsync(
      workbook.Worksheet("Customers"),
      result,
      cancellationToken);

       
        await ImportSampleCategoriesAsync(
    workbook.Worksheet("SampleCategories"),
    result,
    cancellationToken);

        await ImportMatricesAsync(
    workbook.Worksheet("Matrices"),
    result,
    cancellationToken);

        await ImportStandardSamplesAsync(
    workbook.Worksheet("StandardSamples"),
    result,
    cancellationToken);


        await ImportDepartmentResponsibilitiesAsync(
              workbook.Worksheet("DepartmentResponsibilities"),
              result,
              cancellationToken);

        await ImportInstrumentsAsync(
    workbook.Worksheet("Instruments"),
    result,
    cancellationToken);

        await ImportTestMethodsAsync(
    workbook.Worksheet("TestMethods"),
    result,
    cancellationToken);

        await ImportLimitReferencesAsync(
            workbook.Worksheet("LimitReferences"),
            result,
            cancellationToken);

        await ImportTestsAsync(
            workbook.Worksheet("Tests"),
            result,
            cancellationToken);

        await ImportResultDefinitionsAsync(
            workbook.Worksheet("ResultDefinitions"),
            result,
            cancellationToken);

        await ImportTestPanelsAsync(
    workbook.Worksheet("TestPanels"),
    result,
    cancellationToken);

        await ImportPanelItemsAsync(
            workbook.Worksheet("PanelItems"),
            result,
            cancellationToken);

        await ImportDefaultTestSetsAsync(
    workbook,
    result,
    cancellationToken);

        await ImportTestLimitRulesAsync(
    workbook.Worksheet("TestLimitRules"),
    result,
    cancellationToken);

        result.TotalRows = result.Sheets.Sum(x => x.TotalRows);

        return result;
    }

    private async Task ImportDepartmentsAsync(
    IXLWorksheet worksheet,
    MasterDataImportResultDto result,
    CancellationToken cancellationToken)
    {
        var sheetResult =
            result.Sheets.First(x => x.SheetName == "Departments");

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var code = worksheet.Cell(rowNumber, 1).GetString().Trim();
            var name = worksheet.Cell(rowNumber, 2).GetString().Trim();
            var parentDepartmentCode =
                worksheet.Cell(rowNumber, 4).GetString().Trim();
            var description =
                worksheet.Cell(rowNumber, 6).GetString().Trim();

            if (string.IsNullOrWhiteSpace(code) &&
                string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            sheetResult.TotalRows++;
            result.TotalRows++;

            var rowResult = new MasterDataImportRowResultDto
            {
                SheetName = "Departments",
                RowNumber = rowNumber,
                Code = code
            };

            if (string.IsNullOrWhiteSpace(code))
            {
                rowResult.IsValid = false;
                rowResult.Action = "Error";
                rowResult.Errors.Add("Code خالی است.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                rowResult.IsValid = false;
                rowResult.Action = "Error";
                rowResult.Errors.Add("Name خالی است.");
            }

            if (!rowResult.IsValid &&
                rowResult.Errors.Count > 0)
            {
                sheetResult.ErrorCount++;
                result.ErrorCount++;
                sheetResult.Rows.Add(rowResult);
                continue;
            }

            var existing =
                (await _unitOfWork.Departments.GetAllAsync())
                .FirstOrDefault(x =>
                    x.Code.Equals(
                        code,
                        StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                var department = new Department
                {
                    Code = code,
                    Name = name,
                    Description =
                        string.IsNullOrWhiteSpace(description)
                            ? null
                            : description
                };

                await _unitOfWork.Departments.AddAsync(department);

                rowResult.Action = "Insert";
                rowResult.IsValid = true;

                sheetResult.InsertCount++;
                result.InsertCount++;
            }
            else
            {
                existing.Name = name;
                existing.Description =
                    string.IsNullOrWhiteSpace(description)
                        ? null
                        : description;

                _unitOfWork.Departments.Update(existing);

                rowResult.Action = "Update";
                rowResult.IsValid = true;

                sheetResult.UpdateCount++;
                result.UpdateCount++;
            }

            sheetResult.Rows.Add(rowResult);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ImportRolesAsync(
    IXLWorksheet worksheet,
    MasterDataImportResultDto result,
    CancellationToken cancellationToken)
    {
        var sheetResult =
            result.Sheets.First(x => x.SheetName == "Roles");

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        var roles =
            await _unitOfWork.Roles.GetAllAsync();

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var code = worksheet.Cell(rowNumber, 1).GetString().Trim();
            var name = worksheet.Cell(rowNumber, 2).GetString().Trim();
            var englishName = worksheet.Cell(rowNumber, 3).GetString().Trim();
            var isActive = worksheet.Cell(rowNumber, 4).GetValue<bool>();
            var description = worksheet.Cell(rowNumber, 5).GetString().Trim();

            if (string.IsNullOrWhiteSpace(code) &&
                string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            sheetResult.TotalRows++;
            result.TotalRows++;

            var rowResult = new MasterDataImportRowResultDto
            {
                SheetName = "Roles",
                RowNumber = rowNumber,
                Code = code,
                IsValid = true
            };

            if (string.IsNullOrWhiteSpace(code))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("Code خالی است.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("Name خالی است.");
            }

            if (!rowResult.IsValid)
            {
                rowResult.Action = "Error";
                sheetResult.ErrorCount++;
                result.ErrorCount++;
                sheetResult.Rows.Add(rowResult);
                continue;
            }

            var existing =
                roles.FirstOrDefault(x =>
                    x.Code.Equals(
                        code,
                        StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                var role = new Role
                {
                    Code = code,
                    Name = name,
                    Description =
                        string.IsNullOrWhiteSpace(description)
                            ? null
                            : description
                };

                await _unitOfWork.Roles.AddAsync(role);

                rowResult.Action = "Insert";
                sheetResult.InsertCount++;
                result.InsertCount++;
            }
            else
            {
                existing.Name = name;
                existing.Description =
                    string.IsNullOrWhiteSpace(description)
                        ? null
                        : description;

                _unitOfWork.Roles.Update(existing);

                rowResult.Action = "Update";
                sheetResult.UpdateCount++;
                result.UpdateCount++;
            }

            sheetResult.Rows.Add(rowResult);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    public Task<List<TariffImportRowDto>> ReadTestTariffsAsync(
        byte[] fileBytes,
        CancellationToken cancellationToken = default)
    {
        if (fileBytes == null || fileBytes.Length == 0)
            throw new ArgumentException(
                "فایل Excel خالی است.",
                nameof(fileBytes));

        using var stream = new MemoryStream(fileBytes);
        using var workbook = new XLWorkbook(stream);

        var worksheet = workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
            throw new InvalidOperationException(
                "هیچ Sheetای در فایل Excel پیدا نشد.");

        var rows = new List<TariffImportRowDto>();

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;

        if (lastRow < 2)
            return Task.FromResult(rows);

        var headers = new Dictionary<string, int>(
            StringComparer.OrdinalIgnoreCase);

        var headerRow = worksheet.Row(1);

        foreach (var cell in headerRow.CellsUsed())
        {
            var header = cell.GetString().Trim();

            if (!string.IsNullOrWhiteSpace(header))
                headers[header] = cell.Address.ColumnNumber;
        }

        var requiredHeaders = new[]
                {
                    "TargetType",
                    "TariffCode",
                    "TariffName",
                    "Price",
                    "ValidFrom"
                };

        var missingHeaders = requiredHeaders
            .Where(x => !headers.ContainsKey(x))
            .ToList();

        if (!headers.ContainsKey("Organization") &&
            !headers.ContainsKey("OrganizationCode"))
        {
            missingHeaders.Insert(0, "Organization");
        }

        if (missingHeaders.Count > 0)
        {
            throw new InvalidOperationException(
                $"ستون‌های الزامی در فایل وجود ندارند: " +
                $"{string.Join(", ", missingHeaders)}");
        }

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var row = worksheet.Row(rowNumber);

            if (row.CellsUsed().All(x =>
                    string.IsNullOrWhiteSpace(x.GetString())))
            {
                continue;
            }

            var item = new TariffImportRowDto
            {
                RowNumber = rowNumber,

                OrganizationCode =
                    ExtractCode(
                        GetNullableString(row, headers, "Organization")
                        ?? GetNullableString(row, headers, "OrganizationCode")),

                CustomerCode =
                    ExtractCode(
                        GetNullableString(row, headers, "Customer")),

                StandardSampleCode =
    ExtractCode(
        GetNullableString(row, headers, "StandardSample")
        ?? GetNullableString(row, headers, "StandardSampleCode")),

                MatrixCode = ExtractCode(
    GetNullableString(row, headers, "Matrix")
    ?? GetNullableString(row, headers, "MatrixCode")),


                TargetType =
                    GetString(row, headers, "TargetType"),

                TestCode =
    ExtractCode(
        GetNullableString(row, headers, "Test")
        ?? GetNullableString(row, headers, "TestCode")),

                PanelCode =
    ExtractCode(
        GetNullableString(row, headers, "Panel")
        ?? GetNullableString(row, headers, "PanelCode")),

                TariffCode =
                    GetString(row, headers, "TariffCode"),

                TariffName =
                    GetString(row, headers, "TariffName"),

                Price =
                    GetNullableDecimal(row, headers, "Price"),

                Currency =
                    GetNullableString(row, headers, "Currency"),

                ValidFrom =
                    GetNullableString(row, headers, "ValidFrom"),

                ValidTo =
                    GetNullableString(row, headers, "ValidTo"),

                Priority =
                    GetNullableInt(row, headers, "Priority"),

                IsActive =
                    GetNullableBool(row, headers, "IsActive"),

                Description =
                    GetNullableString(row, headers, "Description")
            };

            rows.Add(item);
        }

        return Task.FromResult(rows);
    }

    public async Task<List<TariffImportRowResultDto>>
    ValidateTestTariffsAsync(
        List<TariffImportRowDto> rows,
        CancellationToken cancellationToken = default)
    {
        if (rows == null)
            throw new ArgumentNullException(nameof(rows));

        var results = rows
            .Select(row => new TariffImportRowResultDto
            {
                Row = row
            })
            .ToList();

        var organizationTypes =
            (await _unitOfWork.OrganizationTypes.GetAllAsync())
            .Where(x => !x.IsDeleted && x.IsActive)
            .ToList();

        var organizationLookup = organizationTypes
            .Where(x => !string.IsNullOrWhiteSpace(x.Code))
            .GroupBy(x => x.Code.Trim(),
                StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                x => x.Key,
                x => x.First(),
                StringComparer.OrdinalIgnoreCase);

        var tariffStandardSamples =
      (await _unitOfWork.StandardSamples.GetAllAsync(cancellationToken))
      .Where(x =>
          !x.IsDeleted &&
          !string.IsNullOrWhiteSpace(x.Code))
      .ToList();

        var standardSampleLookup = tariffStandardSamples
            .GroupBy(
                x => x.Code.Trim(),
                StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                x => x.Key,
                x => x.First(),
                StringComparer.OrdinalIgnoreCase);

        var matrices =
                (await _unitOfWork.Matrices.GetAllAsync())
                .Where(x =>
                    !x.IsDeleted &&
                    !string.IsNullOrWhiteSpace(x.Code))
                .ToList();

        var matrixLookup = matrices
            .GroupBy(
                x => x.Code.Trim(),
                StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                x => x.Key,
                x => x.First(),
                StringComparer.OrdinalIgnoreCase);

        var duplicateKeys = results
            .GroupBy(x => BuildDuplicateKey(x.Row),
                StringComparer.OrdinalIgnoreCase)
            .Where(x => !string.IsNullOrWhiteSpace(x.Key) &&
                        x.Count() > 1)
            .SelectMany(x => x)
            .ToHashSet();

        foreach (var result in results)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var row = result.Row;

            if (string.IsNullOrWhiteSpace(row.OrganizationCode))
            {
                result.Errors.Add(
                    "OrganizationCode الزامی است.");
            }
            else if (organizationLookup.TryGetValue(
                         row.OrganizationCode.Trim(),
                         out var organizationType))
            {
                result.OrganizationTypeId =
                    organizationType.Id;
            }
            else
            {
                result.Errors.Add(
                    $"OrganizationCode '{row.OrganizationCode}' پیدا نشد.");
            }

            var targetType =
                row.TargetType?.Trim();

            if (!string.Equals(targetType, "Test",
                    StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(targetType, "Panel",
                    StringComparison.OrdinalIgnoreCase))
            {
                result.Errors.Add(
                    "TargetType باید Test یا Panel باشد.");
            }

            if (string.IsNullOrWhiteSpace(row.TestCode) &&
                string.IsNullOrWhiteSpace(row.PanelCode))
            {
                result.Errors.Add(
                    "یکی از TestCode یا PanelCode باید مشخص شود.");
            }

            if (!string.IsNullOrWhiteSpace(row.TestCode) &&
                !string.IsNullOrWhiteSpace(row.PanelCode))
            {
                result.Errors.Add(
                    "TestCode و PanelCode نباید همزمان مقدار داشته باشند.");
            }

            if (string.Equals(targetType, "Test",
                    StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(row.TestCode))
                {
                    result.Errors.Add(
                        "برای TargetType=Test، TestCode الزامی است.");
                }
                else
                {
                    var test =
                        await _unitOfWork.Tests.GetByCodeAsync(
                            row.TestCode.Trim());

                    if (test == null || test.IsDeleted)
                    {
                        result.Errors.Add(
                            $"Test با کد '{row.TestCode}' پیدا نشد.");
                    }
                    else
                    {
                        result.TestId = test.Id;
                    }
                }

                if (!string.IsNullOrWhiteSpace(row.PanelCode))
                {
                    result.Errors.Add(
                        "برای TargetType=Test نباید PanelCode وارد شود.");
                }
            }

            if (string.Equals(targetType, "Panel",
                    StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(row.PanelCode))
                {
                    result.Errors.Add(
                        "برای TargetType=Panel، PanelCode الزامی است.");
                }
                else
                {
                    var panel =
                        await _unitOfWork.TestPanels.GetByCodeAsync(
                            row.PanelCode.Trim(),
                            cancellationToken);

                    if (panel == null || panel.IsDeleted)
                    {
                        result.Errors.Add(
                            $"Panel با کد '{row.PanelCode}' پیدا نشد.");
                    }
                    else
                    {
                        result.TestPanelId = panel.Id;
                    }
                }

                if (!string.IsNullOrWhiteSpace(row.TestCode))
                {
                    result.Errors.Add(
                        "برای TargetType=Panel نباید TestCode وارد شود.");
                }
            }

            if (!string.IsNullOrWhiteSpace(row.CustomerCode))
            {
                var customer =
                    await _unitOfWork.Customers.GetByCodeAsync(
                        row.CustomerCode.Trim());

                if (customer == null || customer.IsDeleted)
                {
                    result.Errors.Add(
                        $"Customer با کد '{row.CustomerCode}' پیدا نشد.");
                }
                else
                {
                    result.CustomerId = customer.Id;
                }
            }
            if (!string.IsNullOrWhiteSpace(row.StandardSampleCode))
            {
                if (standardSampleLookup.TryGetValue(
                        row.StandardSampleCode.Trim(),
                        out var standardSample))
                {
                    result.StandardSampleId = standardSample.Id;
                }
                else
                {
                    result.Errors.Add(
                        $"StandardSample با کد '{row.StandardSampleCode}' پیدا نشد.");
                }
            }

            if (!string.IsNullOrWhiteSpace(row.MatrixCode))
            {
                if (matrixLookup.TryGetValue(
                        row.MatrixCode.Trim(),
                        out var matrix))
                {
                    result.MatrixId = matrix.Id;
                }
                else
                {
                    result.Errors.Add(
                        $"Matrix با کد '{row.MatrixCode}' پیدا نشد.");
                }
            }

            if (string.IsNullOrWhiteSpace(row.TariffName))
            {
                result.Errors.Add(
                    "TariffName الزامی است.");
            }

            if (!row.Price.HasValue)
            {
                result.Errors.Add(
                    "Price الزامی و باید عددی باشد.");
            }
            else if (row.Price.Value < 0)
            {
                result.Errors.Add(
                    "Price نمی‌تواند منفی باشد.");
            }

            if (string.IsNullOrWhiteSpace(row.ValidFrom))
            {
                result.Errors.Add(
                    "ValidFrom الزامی است.");
            }
            else
            {
                result.ParsedValidFrom =
                    TryParseExcelDate(row.ValidFrom);

                if (!result.ParsedValidFrom.HasValue)
                {
                    result.Errors.Add(
                        $"ValidFrom '{row.ValidFrom}' معتبر نیست.");
                }
            }

            if (!string.IsNullOrWhiteSpace(row.ValidTo))
            {
                result.ParsedValidTo =
                    TryParseExcelDate(row.ValidTo);

                if (!result.ParsedValidTo.HasValue)
                {
                    result.Errors.Add(
                        $"ValidTo '{row.ValidTo}' معتبر نیست.");
                }
            }

            if (result.ParsedValidFrom.HasValue &&
                result.ParsedValidTo.HasValue &&
                result.ParsedValidTo.Value.Date <
                result.ParsedValidFrom.Value.Date)
            {
                result.Errors.Add(
                    "ValidTo نمی‌تواند قبل از ValidFrom باشد.");
            }

            if (row.Priority.HasValue &&
                row.Priority.Value < 0)
            {
                result.Errors.Add(
                    "Priority نمی‌تواند منفی باشد.");
            }

            if (duplicateKeys.Contains(result))
            {
                result.Errors.Add(
                    "این ردیف در فایل Excel تکراری است.");
            }

            result.IsValid = result.Errors.Count == 0;
        }

        return results;
    }

    public async Task<int> CommitTestTariffsAsync(
    List<TariffImportRowResultDto> rows,
    CancellationToken cancellationToken = default)
    {
        if (rows == null)
            throw new ArgumentNullException(nameof(rows));

        var validRows = rows
            .Where(x => x != null && x.IsValid)
            .ToList();

        if (validRows.Count == 0)
            return 0;

        cancellationToken.ThrowIfCancellationRequested();

        // تعرفه‌های موجود را یک‌بار می‌خوانیم تا هم سریع‌تر باشد
        // و هم در طول Import بر اساس همان Snapshot تصمیم بگیریم.
        var existingTariffs =
            await _unitOfWork.TestTariffs.GetAllAsync();

        var existingKeys = existingTariffs
            .Select(BuildExistingTariffKey)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var newTariffs = new List<TestTariff>();

        foreach (var result in validRows)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!result.OrganizationTypeId.HasValue)
                continue;

            if (!result.ParsedValidFrom.HasValue)
                continue;

            var targetType =
                result.Row.TargetType?.Trim();

            Guid? testId = null;
            Guid? testPanelId = null;

            if (string.Equals(
                    targetType,
                    "Test",
                    StringComparison.OrdinalIgnoreCase))
            {
                testId = result.TestId;
            }
            else if (string.Equals(
                         targetType,
                         "Panel",
                         StringComparison.OrdinalIgnoreCase))
            {
                testPanelId = result.TestPanelId;
            }

            if (!testId.HasValue && !testPanelId.HasValue)
                continue;

            var validFrom =
                DateTime.SpecifyKind(
                    result.ParsedValidFrom.Value.Date,
                    DateTimeKind.Utc);

            DateTime? validTo = null;

            if (result.ParsedValidTo.HasValue)
            {
                validTo =
                    DateTime.SpecifyKind(
                        result.ParsedValidTo.Value.Date,
                        DateTimeKind.Utc);
            }

            var tariff = new TestTariff
            {
                Id = Guid.NewGuid(),

                Code = string.IsNullOrWhiteSpace(result.Row.TariffCode)
                    ? $"TAR-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant()
                    : result.Row.TariffCode
                        .Trim()
                        .ToUpperInvariant(),

                Name =
                    result.Row.TariffName
                        .Trim(),

                TestId = testId,
                TestPanelId = testPanelId,

                OrganizationTypeId =
                    result.OrganizationTypeId.Value,

                CustomerId =
                    result.CustomerId,

                StandardSampleId = result.StandardSampleId,

                MatrixId = result.MatrixId,

                Price =
                    result.Row.Price!.Value,

                Currency =
                    string.IsNullOrWhiteSpace(result.Row.Currency)
                        ? "ریال"
                        : result.Row.Currency.Trim(),

                ValidFrom =
                    validFrom,

                ValidTo =
                    validTo,

                Priority =
                    result.Row.Priority ?? 100,

                Description =
                    result.Row.Description,

                IsDeleted = false,
                IsActive =
                    result.Row.IsActive ?? true
            };

            var key = BuildExistingTariffKey(tariff);

            // اگر دقیقاً همین تعرفه قبلاً وجود داشته،
            // دوباره ایجادش نمی‌کنیم.
            if (existingKeys.Contains(key))
                continue;

            // جلوگیری از Duplicate داخل همان فایل Import
            // حتی قبل از SaveChanges
            if (newTariffs.Any(x =>
                    string.Equals(
                        BuildExistingTariffKey(x),
                        key,
                        StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            newTariffs.Add(tariff);
            existingKeys.Add(key);
        }

        if (newTariffs.Count == 0)
            return 0;

        foreach (var tariff in newTariffs)
        {
            await _unitOfWork.TestTariffs.AddAsync(tariff);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return newTariffs.Count;
    }
    private static string BuildDuplicateKey(
    TariffImportRowDto row)
    {
        return string.Join("|",
            row.OrganizationCode?.Trim() ?? "",
            row.CustomerCode?.Trim() ?? "",
            row.TargetType?.Trim() ?? "",
            row.TestCode?.Trim() ?? "",
            row.PanelCode?.Trim() ?? "",
            row.TariffCode?.Trim() ?? "",
            row.ValidFrom?.Trim() ?? "");
    }
    private static string BuildExistingTariffKey(
    TestTariff tariff)
    {
        return string.Join("|",
            tariff.OrganizationTypeId,
            tariff.CustomerId?.ToString() ?? "",
            tariff.TestId?.ToString() ?? "",
            tariff.TestPanelId?.ToString() ?? "",
            tariff.Code?.Trim() ?? "",
            tariff.ValidFrom.Date.Ticks);
    }
    private static DateTime? TryParseExcelDate(
        string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var text = input.Trim()
            .Replace('-', '/')
            .Replace('\\', '/');

        var match = Regex.Match(
            text,
            @"^(?<y>\d{4})/(?<m>\d{1,2})/(?<d>\d{1,2})$");

        if (!match.Success)
            return null;

        if (!int.TryParse(
                match.Groups["y"].Value,
                out var year) ||
            !int.TryParse(
                match.Groups["m"].Value,
                out var month) ||
            !int.TryParse(
                match.Groups["d"].Value,
                out var day))
        {
            return null;
        }

        if (year >= 1200 && year <= 1499)
        {
            try
            {
                var calendar = new PersianCalendar();

                return calendar.ToDateTime(
                    year,
                    month,
                    day,
                    0,
                    0,
                    0,
                    0);
            }
            catch
            {
                return null;
            }
        }

        if (year >= 1600 && year <= 2500)
        {
            try
            {
                return new DateTime(year, month, day);
            }
            catch
            {
                return null;
            }
        }

        return null;
    }
    private static string? ExtractCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var text = value.Trim();

        var separatorIndex = text.IndexOf('|');

        if (separatorIndex >= 0)
            return text[..separatorIndex].Trim();

        return text;
    }
    private static string GetString(
        IXLRow row,
        Dictionary<string, int> headers,
        string header)
    {
        return headers.TryGetValue(header, out var column)
            ? row.Cell(column).GetString().Trim()
            : "";
    }

    private static string? GetNullableString(
        IXLRow row,
        Dictionary<string, int> headers,
        string header)
    {
        var value = GetString(row, headers, header);

        return string.IsNullOrWhiteSpace(value)
            ? null
            : value;
    }

    private static decimal? GetNullableDecimal(
        IXLRow row,
        Dictionary<string, int> headers,
        string header)
    {
        if (!headers.TryGetValue(header, out var column))
            return null;

        var cell = row.Cell(column);

        if (cell.IsEmpty())
            return null;

        if (cell.TryGetValue<decimal>(out var value))
            return value;

        var text = cell.GetString().Trim();

        return decimal.TryParse(
            text,
            out var parsed)
            ? parsed
            : null;
    }

    private static int? GetNullableInt(
        IXLRow row,
        Dictionary<string, int> headers,
        string header)
    {
        if (!headers.TryGetValue(header, out var column))
            return null;

        var cell = row.Cell(column);

        if (cell.IsEmpty())
            return null;

        if (cell.TryGetValue<int>(out var value))
            return value;

        var text = cell.GetString().Trim();

        return int.TryParse(
            text,
            out var parsed)
            ? parsed
            : null;
    }

    private static bool? GetNullableBool(
        IXLRow row,
        Dictionary<string, int> headers,
        string header)
    {
        if (!headers.TryGetValue(header, out var column))
            return null;

        var cell = row.Cell(column);

        if (cell.IsEmpty())
            return null;

        if (cell.TryGetValue<bool>(out var value))
            return value;

        var text = cell.GetString().Trim();

        if (bool.TryParse(text, out var parsed))
            return parsed;

        if (text is "1" or "بله" or "فعال")
            return true;

        if (text is "0" or "خیر" or "غیرفعال")
            return false;

        return null;
    }

    
    private async Task ImportEmployeesAsync(
    IXLWorksheet worksheet,
    MasterDataImportResultDto result,
    CancellationToken cancellationToken)
    {
        var sheetResult =
            result.Sheets.First(x => x.SheetName == "Employees");

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var code = worksheet.Cell(rowNumber, 1).GetString().Trim();
            var firstName = worksheet.Cell(rowNumber, 2).GetString().Trim();
            var lastName = worksheet.Cell(rowNumber, 3).GetString().Trim();
            var nationalId = worksheet.Cell(rowNumber, 4).GetString().Trim();
            var personnelCode = worksheet.Cell(rowNumber, 5).GetString().Trim();
            var mobile = worksheet.Cell(rowNumber, 9).GetString().Trim();
            var email = worksheet.Cell(rowNumber, 10).GetString().Trim();

            if (string.IsNullOrWhiteSpace(code) &&
                string.IsNullOrWhiteSpace(personnelCode) &&
                string.IsNullOrWhiteSpace(firstName) &&
                string.IsNullOrWhiteSpace(lastName))
            {
                continue;
            }

            sheetResult.TotalRows++;
            result.TotalRows++;

            var rowResult = new MasterDataImportRowResultDto
            {
                SheetName = "Employees",
                RowNumber = rowNumber,
                Code = code,
                IsValid = true
            };

            if (string.IsNullOrWhiteSpace(personnelCode))
            {
                rowResult.IsValid = false;
                rowResult.Action = "Error";
                rowResult.Errors.Add("PersonnelCode خالی است.");
            }

            if (string.IsNullOrWhiteSpace(firstName) &&
                string.IsNullOrWhiteSpace(lastName))
            {
                rowResult.IsValid = false;
                rowResult.Action = "Error";
                rowResult.Errors.Add("نام و نام خانوادگی خالی است.");
            }

            if (!rowResult.IsValid)
            {
                sheetResult.ErrorCount++;
                result.ErrorCount++;

                sheetResult.Rows.Add(rowResult);

                continue;
            }

            var fullName =
                string.Join(
                    " ",
                    new[] { firstName, lastName }
                        .Where(x => !string.IsNullOrWhiteSpace(x)));

            var existing =
                (await _unitOfWork.Employees.GetAllAsync())
                .FirstOrDefault(x =>
                    x.PersonnelCode.Equals(
                        personnelCode,
                        StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                var employee = new Employee
                {
                    PersonnelCode = personnelCode,
                    FullName = fullName,
                    NationalCode = nationalId,
                    Mobile = string.IsNullOrWhiteSpace(mobile)
                        ? null
                        : mobile,
                    Email = string.IsNullOrWhiteSpace(email)
                        ? null
                        : email
                };

                await _unitOfWork.Employees.AddAsync(employee);

                rowResult.Action = "Insert";

                sheetResult.InsertCount++;
                result.InsertCount++;
            }
            else
            {
                existing.FullName = fullName;
                existing.NationalCode = nationalId;
                existing.Mobile =
                    string.IsNullOrWhiteSpace(mobile)
                        ? null
                        : mobile;
                existing.Email =
                    string.IsNullOrWhiteSpace(email)
                        ? null
                        : email;

                _unitOfWork.Employees.Update(existing);

                rowResult.Action = "Update";

                sheetResult.UpdateCount++;
                result.UpdateCount++;
            }

            sheetResult.Rows.Add(rowResult);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ImportCustomersAsync(
    IXLWorksheet worksheet,
    MasterDataImportResultDto result,
    CancellationToken cancellationToken)
    {
        var sheetResult =
            result.Sheets.First(x => x.SheetName == "Customers");

        var lastRow =
            worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var code =
                worksheet.Cell(rowNumber, 1).GetString().Trim();

            var name =
                worksheet.Cell(rowNumber, 2).GetString().Trim();

            var nationalId =
                worksheet.Cell(rowNumber, 4).GetString().Trim();

            var economicCode =
                worksheet.Cell(rowNumber, 5).GetString().Trim();

            var phone =
                worksheet.Cell(rowNumber, 6).GetString().Trim();

            var mobile =
                worksheet.Cell(rowNumber, 7).GetString().Trim();

            var email =
                worksheet.Cell(rowNumber, 8).GetString().Trim();

            var address =
                worksheet.Cell(rowNumber, 9).GetString().Trim();

            var description =
                worksheet.Cell(rowNumber, 12).GetString().Trim();

            if (string.IsNullOrWhiteSpace(code) &&
                string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            sheetResult.TotalRows++;
            result.TotalRows++;

            var rowResult =
                new MasterDataImportRowResultDto
                {
                    SheetName = "Customers",
                    RowNumber = rowNumber,
                    Code = code,
                    IsValid = true
                };

            if (string.IsNullOrWhiteSpace(code))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("Code خالی است.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("Name خالی است.");
            }

            if (!rowResult.IsValid)
            {
                rowResult.Action = "Error";
                sheetResult.ErrorCount++;
                result.ErrorCount++;
                sheetResult.Rows.Add(rowResult);
                continue;
            }

            var existing =
                (await _unitOfWork.Customers.GetAllAsync())
                .FirstOrDefault(x =>
                    x.Code.Equals(
                        code,
                        StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                var customer = new Customer
                {
                    Code = code,
                    DisplayName = name,
                    NationalId =
                        string.IsNullOrWhiteSpace(nationalId)
                            ? null
                            : nationalId,
                    EconomicCode =
                        string.IsNullOrWhiteSpace(economicCode)
                            ? null
                            : economicCode,
                    Phone =
                        string.IsNullOrWhiteSpace(phone)
                            ? null
                            : phone,
                    Mobile =
                        string.IsNullOrWhiteSpace(mobile)
                            ? null
                            : mobile,
                    Email =
                        string.IsNullOrWhiteSpace(email)
                            ? null
                            : email,
                    Address =
                        string.IsNullOrWhiteSpace(address)
                            ? null
                            : address,
                    Description =
                        string.IsNullOrWhiteSpace(description)
                            ? null
                            : description,
                    IsActive = true
                };

                await _unitOfWork.Customers.AddAsync(customer);

                rowResult.Action = "Insert";
                sheetResult.InsertCount++;
                result.InsertCount++;
            }
            else
            {
                existing.DisplayName = name;

                existing.NationalId =
                    string.IsNullOrWhiteSpace(nationalId)
                        ? null
                        : nationalId;

                existing.EconomicCode =
                    string.IsNullOrWhiteSpace(economicCode)
                        ? null
                        : economicCode;

                existing.Phone =
                    string.IsNullOrWhiteSpace(phone)
                        ? null
                        : phone;

                existing.Mobile =
                    string.IsNullOrWhiteSpace(mobile)
                        ? null
                        : mobile;

                existing.Email =
                    string.IsNullOrWhiteSpace(email)
                        ? null
                        : email;

                existing.Address =
                    string.IsNullOrWhiteSpace(address)
                        ? null
                        : address;

                existing.Description =
                    string.IsNullOrWhiteSpace(description)
                        ? null
                        : description;

                _unitOfWork.Customers.Update(existing);

                rowResult.Action = "Update";
                sheetResult.UpdateCount++;
                result.UpdateCount++;
            }

            sheetResult.Rows.Add(rowResult);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ImportDepartmentResponsibilitiesAsync(
    IXLWorksheet worksheet,
    MasterDataImportResultDto result,
    CancellationToken cancellationToken)
    {
        var sheetResult =
            result.Sheets.First(
                x => x.SheetName == "DepartmentResponsibilities");

        var lastRow =
            worksheet.LastRowUsed()?.RowNumber() ?? 1;

        var departments =
            await _unitOfWork.Departments.GetAllAsync();

        var employees =
            await _unitOfWork.Employees.GetAllAsync();

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var code =
                worksheet.Cell(rowNumber, 1).GetString().Trim();

            var departmentCode =
                worksheet.Cell(rowNumber, 2).GetString().Trim();

            var employeeCode =
                worksheet.Cell(rowNumber, 3).GetString().Trim();

            var responsibilityTypeText =
                worksheet.Cell(rowNumber, 4).GetString().Trim();

            var isPrimaryText =
                worksheet.Cell(rowNumber, 5).GetString().Trim();

            if (string.IsNullOrWhiteSpace(departmentCode) &&
                string.IsNullOrWhiteSpace(employeeCode) &&
                string.IsNullOrWhiteSpace(responsibilityTypeText))
            {
                continue;
            }

            sheetResult.TotalRows++;
            result.TotalRows++;

            var rowResult =
                new MasterDataImportRowResultDto
                {
                    SheetName = "DepartmentResponsibilities",
                    RowNumber = rowNumber,
                    Code = code,
                    IsValid = true
                };

            if (string.IsNullOrWhiteSpace(departmentCode))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add(
                    "DepartmentCode خالی است.");
            }

            if (string.IsNullOrWhiteSpace(employeeCode))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add(
                    "EmployeeCode خالی است.");
            }

            if (string.IsNullOrWhiteSpace(
                    responsibilityTypeText))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add(
                    "ResponsibilityType خالی است.");
            }

            var department =
                departments.FirstOrDefault(x =>
                    x.Code.Equals(
                        departmentCode,
                        StringComparison.OrdinalIgnoreCase));

            if (department == null)
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add(
                    $"Department با Code '{departmentCode}' پیدا نشد.");
            }

            var employee =
                employees.FirstOrDefault(x =>
                    x.PersonnelCode.Equals(
                        employeeCode,
                        StringComparison.OrdinalIgnoreCase));

            if (employee == null)
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add(
                    $"Employee با Code '{employeeCode}' پیدا نشد.");
            }

            DepartmentResponsibilityType responsibilityType;

            if (string.Equals(
                    responsibilityTypeText,
                    "TECHNICAL_MANAGER",
                    StringComparison.OrdinalIgnoreCase))
            {
                responsibilityType =
                    DepartmentResponsibilityType.TechnicalManager;
            }
            else if (string.Equals(
                         responsibilityTypeText,
                         "SECTION_HEAD",
                         StringComparison.OrdinalIgnoreCase))
            {
                responsibilityType =
                    DepartmentResponsibilityType.SectionHead;
            }
            else
            {
                rowResult.IsValid = false;

                rowResult.Errors.Add(
                    $"ResponsibilityType '{responsibilityTypeText}' نامعتبر است.");

                responsibilityType =
                    DepartmentResponsibilityType.SectionHead;
            }

            var isPrimary = false;

            if (!string.IsNullOrWhiteSpace(isPrimaryText))
            {
                if (bool.TryParse(
                        isPrimaryText,
                        out var parsed))
                {
                    isPrimary = parsed;
                }
                else
                {
                    rowResult.IsValid = false;
                    rowResult.Errors.Add(
                        $"IsPrimary مقدار نامعتبر '{isPrimaryText}' دارد.");
                }
            }

            if (!rowResult.IsValid)
            {
                rowResult.Action = "Error";

                sheetResult.ErrorCount++;
                result.ErrorCount++;

                sheetResult.Rows.Add(rowResult);

                continue;
            }

            var existing =
                await _unitOfWork.DepartmentResponsibilities
                    .GetByDepartmentIdAsync(
                        department!.Id);

            var existingResponsibility =
                existing.FirstOrDefault(x =>
                    x.EmployeeId == employee!.Id &&
                    x.ResponsibilityType == responsibilityType);

            if (existingResponsibility == null)
            {
                var entity =
                    new DepartmentResponsibility
                    {
                        EmployeeId = employee.Id,
                        DepartmentId = department.Id,
                        ResponsibilityType =
                            responsibilityType,
                        IsPrimary = isPrimary
                    };

                await _unitOfWork.DepartmentResponsibilities
                    .AddAsync(entity);

                rowResult.Action = "Insert";

                sheetResult.InsertCount++;
                result.InsertCount++;
            }
            else
            {
                existingResponsibility.IsPrimary =
                    isPrimary;

                _unitOfWork.DepartmentResponsibilities
                    .Update(existingResponsibility);

                rowResult.Action = "Update";

                sheetResult.UpdateCount++;
                result.UpdateCount++;
            }

            sheetResult.Rows.Add(rowResult);
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }


    private async Task ImportSampleCategoriesAsync(
    IXLWorksheet worksheet,
    MasterDataImportResultDto result,
    CancellationToken cancellationToken)
    {
        var sheetResult =
            result.Sheets.First(x => x.SheetName == "SampleCategories");

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var code = worksheet.Cell(rowNumber, 1).GetString().Trim();
            var name = worksheet.Cell(rowNumber, 2).GetString().Trim();
            var description =
                worksheet.Cell(rowNumber, 5).GetString().Trim();

            if (string.IsNullOrWhiteSpace(code) &&
                string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            sheetResult.TotalRows++;
            result.TotalRows++;

            var rowResult = new MasterDataImportRowResultDto
            {
                SheetName = "SampleCategories",
                RowNumber = rowNumber,
                Code = code,
                IsValid = true
            };

            if (string.IsNullOrWhiteSpace(code))
            {
                rowResult.IsValid = false;
                rowResult.Action = "Error";
                rowResult.Errors.Add("Code خالی است.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                rowResult.IsValid = false;
                rowResult.Action = "Error";
                rowResult.Errors.Add("Name خالی است.");
            }

            if (!rowResult.IsValid)
            {
                sheetResult.ErrorCount++;
                result.ErrorCount++;

                sheetResult.Rows.Add(rowResult);

                continue;
            }

            var existing =
                (await _unitOfWork.SampleCategories.GetAllAsync(
                    cancellationToken))
                .FirstOrDefault(x =>
                    x.Code.Equals(
                        code,
                        StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                var sampleCategory = new SampleCategory
                {
                    Code = code,
                    Name = name,
                    Description =
                        string.IsNullOrWhiteSpace(description)
                            ? null
                            : description
                };

                await _unitOfWork.SampleCategories.AddAsync(
                    sampleCategory);

                rowResult.Action = "Insert";

                sheetResult.InsertCount++;
                result.InsertCount++;
            }
            else
            {
                existing.Name = name;
                existing.Description =
                    string.IsNullOrWhiteSpace(description)
                        ? null
                        : description;

                _unitOfWork.SampleCategories.Update(existing);

                rowResult.Action = "Update";

                sheetResult.UpdateCount++;
                result.UpdateCount++;
            }

            sheetResult.Rows.Add(rowResult);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ImportMatricesAsync(
    IXLWorksheet worksheet,
    MasterDataImportResultDto result,
    CancellationToken cancellationToken)
    {
        var sheetResult =
            result.Sheets.First(x => x.SheetName == "Matrices");

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        var sampleCategories =
            await _unitOfWork.SampleCategories.GetAllAsync(
                cancellationToken);

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var code = worksheet.Cell(rowNumber, 1).GetString().Trim();
            var name = worksheet.Cell(rowNumber, 2).GetString().Trim();
            var sampleCategoryCode =
                worksheet.Cell(rowNumber, 4).GetString().Trim();
            var description =
                worksheet.Cell(rowNumber, 6).GetString().Trim();

            if (string.IsNullOrWhiteSpace(code) &&
                string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            sheetResult.TotalRows++;
            result.TotalRows++;

            var rowResult = new MasterDataImportRowResultDto
            {
                SheetName = "Matrices",
                RowNumber = rowNumber,
                Code = code,
                IsValid = true
            };

            if (string.IsNullOrWhiteSpace(code))
            {
                rowResult.IsValid = false;
                rowResult.Action = "Error";
                rowResult.Errors.Add("Code خالی است.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                rowResult.IsValid = false;
                rowResult.Action = "Error";
                rowResult.Errors.Add("Name خالی است.");
            }

            SampleCategory? sampleCategory = null;

            if (!string.IsNullOrWhiteSpace(sampleCategoryCode))
            {
                sampleCategory =
                    sampleCategories.FirstOrDefault(x =>
                        x.Code.Equals(
                            sampleCategoryCode,
                            StringComparison.OrdinalIgnoreCase));

                if (sampleCategory == null)
                {
                    rowResult.IsValid = false;
                    rowResult.Action = "Error";
                    rowResult.Errors.Add(
                        $"SampleCategory با Code '{sampleCategoryCode}' پیدا نشد.");
                }
            }

            if (!rowResult.IsValid)
            {
                sheetResult.ErrorCount++;
                result.ErrorCount++;

                sheetResult.Rows.Add(rowResult);

                continue;
            }

            var existing =
                (await _unitOfWork.Matrices.GetAllAsync(
                    cancellationToken))
                .FirstOrDefault(x =>
                    x.Code.Equals(
                        code,
                        StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                var matrix = new Matrix
                {
                    Code = code,
                    Name = name,
                    SampleCategoryId =
                        sampleCategory?.Id ?? Guid.Empty,
                    Description =
                        string.IsNullOrWhiteSpace(description)
                            ? null
                            : description
                };

                await _unitOfWork.Matrices.AddAsync(matrix);

                rowResult.Action = "Insert";

                sheetResult.InsertCount++;
                result.InsertCount++;
            }
            else
            {
                existing.Name = name;
                existing.SampleCategoryId =
                     sampleCategory?.Id ?? Guid.Empty;
                existing.Description =
                    string.IsNullOrWhiteSpace(description)
                        ? null
                        : description;

                _unitOfWork.Matrices.Update(existing);

                rowResult.Action = "Update";

                sheetResult.UpdateCount++;
                result.UpdateCount++;
            }

            sheetResult.Rows.Add(rowResult);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ImportStandardSamplesAsync(
    IXLWorksheet worksheet,
    MasterDataImportResultDto result,
    CancellationToken cancellationToken)
    {
        var sheetResult =
            result.Sheets.First(x => x.SheetName == "StandardSamples");

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        var sampleCategories =
            await _unitOfWork.SampleCategories.GetAllAsync(
                cancellationToken);

        var matrices =
            await _unitOfWork.Matrices.GetAllAsync(
                cancellationToken);

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var code = worksheet.Cell(rowNumber, 1).GetString().Trim();
            var name = worksheet.Cell(rowNumber, 2).GetString().Trim();

            var sampleCategoryCode =
                worksheet.Cell(rowNumber, 4).GetString().Trim();

            var matrixCode =
                worksheet.Cell(rowNumber, 5).GetString().Trim();

            var description =
                worksheet.Cell(rowNumber, 7).GetString().Trim();

            if (string.IsNullOrWhiteSpace(code) &&
                string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            sheetResult.TotalRows++;
            result.TotalRows++;

            var rowResult = new MasterDataImportRowResultDto
            {
                SheetName = "StandardSamples",
                RowNumber = rowNumber,
                Code = code,
                IsValid = true
            };

            if (string.IsNullOrWhiteSpace(code))
            {
                rowResult.IsValid = false;
                rowResult.Action = "Error";
                rowResult.Errors.Add("Code خالی است.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                rowResult.IsValid = false;
                rowResult.Action = "Error";
                rowResult.Errors.Add("Name خالی است.");
            }

            SampleCategory? sampleCategory = null;

            if (!string.IsNullOrWhiteSpace(sampleCategoryCode))
            {
                sampleCategory =
                    sampleCategories.FirstOrDefault(x =>
                        x.Code.Equals(
                            sampleCategoryCode,
                            StringComparison.OrdinalIgnoreCase));

                if (sampleCategory == null)
                {
                    rowResult.IsValid = false;
                    rowResult.Errors.Add(
                        $"SampleCategory با Code '{sampleCategoryCode}' پیدا نشد.");
                }
            }

            Matrix? matrix = null;

            if (!string.IsNullOrWhiteSpace(matrixCode))
            {
                matrix =
                    matrices.FirstOrDefault(x =>
                        x.Code.Equals(
                            matrixCode,
                            StringComparison.OrdinalIgnoreCase));

                if (matrix == null)
                {
                    rowResult.IsValid = false;
                    rowResult.Errors.Add(
                        $"Matrix با Code '{matrixCode}' پیدا نشد.");
                }
            }

            if (!rowResult.IsValid)
            {
                rowResult.Action = "Error";

                sheetResult.ErrorCount++;
                result.ErrorCount++;

                sheetResult.Rows.Add(rowResult);

                continue;
            }

            var existing =
                (await _unitOfWork.StandardSamples.GetAllAsync(
                    cancellationToken))
                .FirstOrDefault(x =>
                    x.Code.Equals(
                        code,
                        StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                var standardSample = new StandardSample
                {
                    Code = code,
                    Name = name,
                    SampleCategoryId =
                        sampleCategory?.Id ?? Guid.Empty,
                    MatrixId =
                        matrix?.Id ?? Guid.Empty,
                    Description =
                        string.IsNullOrWhiteSpace(description)
                            ? null
                            : description
                };

                await _unitOfWork.StandardSamples.AddAsync(
                    standardSample);

                rowResult.Action = "Insert";

                sheetResult.InsertCount++;
                result.InsertCount++;
            }
            else
            {
                existing.Name = name;
                existing.SampleCategoryId =
                    sampleCategory?.Id ?? Guid.Empty;
                existing.MatrixId =
                    matrix?.Id ?? Guid.Empty;
                existing.Description =
                    string.IsNullOrWhiteSpace(description)
                        ? null
                        : description;

                _unitOfWork.StandardSamples.Update(existing);

                rowResult.Action = "Update";

                sheetResult.UpdateCount++;
                result.UpdateCount++;
            }

            sheetResult.Rows.Add(rowResult);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    private async Task ImportInstrumentsAsync(
    IXLWorksheet worksheet,
    MasterDataImportResultDto result,
    CancellationToken cancellationToken)
    {
        var sheetResult =
            result.Sheets.First(x => x.SheetName == "Instruments");

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var code = worksheet.Cell(rowNumber, 1).GetString().Trim();
            var name = worksheet.Cell(rowNumber, 2).GetString().Trim();
            var manufacturer =
                worksheet.Cell(rowNumber, 4).GetString().Trim();
            var model = worksheet.Cell(rowNumber, 5).GetString().Trim();
            var serialNumber =
                worksheet.Cell(rowNumber, 6).GetString().Trim();
            var isActiveText =
                worksheet.Cell(rowNumber, 17).GetString().Trim();
            var description =
                worksheet.Cell(rowNumber, 18).GetString().Trim();

            if (string.IsNullOrWhiteSpace(code) &&
                string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            sheetResult.TotalRows++;
            result.TotalRows++;

            var rowResult = new MasterDataImportRowResultDto
            {
                SheetName = "Instruments",
                RowNumber = rowNumber,
                Code = code,
                IsValid = true
            };

            if (string.IsNullOrWhiteSpace(code))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("Code خالی است.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("Name خالی است.");
            }

            if (!rowResult.IsValid)
            {
                rowResult.Action = "Error";

                sheetResult.ErrorCount++;
                result.ErrorCount++;

                sheetResult.Rows.Add(rowResult);

                continue;
            }

            var isActive = true;

            if (!string.IsNullOrWhiteSpace(isActiveText))
            {
                if (bool.TryParse(isActiveText, out var parsed))
                {
                    isActive = parsed;
                }
                else
                {
                    rowResult.IsValid = false;
                    rowResult.Action = "Error";
                    rowResult.Errors.Add(
                        $"IsActive مقدار نامعتبر '{isActiveText}' دارد.");

                    sheetResult.ErrorCount++;
                    result.ErrorCount++;

                    sheetResult.Rows.Add(rowResult);

                    continue;
                }
            }

            var existing =
                (await _unitOfWork.Instruments.GetAllAsync())
                .FirstOrDefault(x =>
                    x.Code.Equals(
                        code,
                        StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                var instrument = new Instrument
                {
                    Code = code,
                    Name = name,
                    Manufacturer =
                        string.IsNullOrWhiteSpace(manufacturer)
                            ? null
                            : manufacturer,
                    Model =
                        string.IsNullOrWhiteSpace(model)
                            ? null
                            : model,
                    SerialNumber =
                        string.IsNullOrWhiteSpace(serialNumber)
                            ? null
                            : serialNumber,
                    IsActiveForTesting = isActive,
                    Description =
                        string.IsNullOrWhiteSpace(description)
                            ? null
                            : description
                };

                await _unitOfWork.Instruments.AddAsync(instrument);

                rowResult.Action = "Insert";

                sheetResult.InsertCount++;
                result.InsertCount++;
            }
            else
            {
                existing.Name = name;
                existing.Manufacturer =
                    string.IsNullOrWhiteSpace(manufacturer)
                        ? null
                        : manufacturer;
                existing.Model =
                    string.IsNullOrWhiteSpace(model)
                        ? null
                        : model;
                existing.SerialNumber =
                    string.IsNullOrWhiteSpace(serialNumber)
                        ? null
                        : serialNumber;
                existing.IsActiveForTesting = isActive;
                existing.Description =
                    string.IsNullOrWhiteSpace(description)
                        ? null
                        : description;

                _unitOfWork.Instruments.Update(existing);

                rowResult.Action = "Update";

                sheetResult.UpdateCount++;
                result.UpdateCount++;
            }

            sheetResult.Rows.Add(rowResult);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ImportTestMethodsAsync(
    IXLWorksheet worksheet,
    MasterDataImportResultDto result,
    CancellationToken cancellationToken)
    {
        var sheetResult =
            result.Sheets.First(x => x.SheetName == "TestMethods");

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var code = worksheet.Cell(rowNumber, 1).GetString().Trim();
            var name = worksheet.Cell(rowNumber, 2).GetString().Trim();
            var englishName = worksheet.Cell(rowNumber, 3).GetString().Trim();
            var description = worksheet.Cell(rowNumber, 5).GetString().Trim();

            if (string.IsNullOrWhiteSpace(code) &&
                string.IsNullOrWhiteSpace(name))
                continue;

            sheetResult.TotalRows++;
            result.TotalRows++;

            var rowResult = new MasterDataImportRowResultDto
            {
                SheetName = "TestMethods",
                RowNumber = rowNumber,
                Code = code,
                IsValid = true
            };

            if (string.IsNullOrWhiteSpace(code))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("Code خالی است.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("Name خالی است.");
            }

            if (!rowResult.IsValid)
            {
                rowResult.Action = "Error";
                sheetResult.ErrorCount++;
                result.ErrorCount++;
                sheetResult.Rows.Add(rowResult);
                continue;
            }

            var existing =
                (await _unitOfWork.TestMethods.GetAllAsync())
                .FirstOrDefault(x =>
                    x.Code.Equals(
                        code,
                        StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                var entity = new TestMethod
                {
                    Code = code,
                    Name = name,
                   
                    Description =
                        string.IsNullOrWhiteSpace(description)
                            ? null
                            : description
                };

                await _unitOfWork.TestMethods.AddAsync(entity);

                rowResult.Action = "Insert";
                sheetResult.InsertCount++;
                result.InsertCount++;
            }
            else
            {
                existing.Name = name;
               
                existing.Description =
                    string.IsNullOrWhiteSpace(description)
                        ? null
                        : description;

                _unitOfWork.TestMethods.Update(existing);

                rowResult.Action = "Update";
                sheetResult.UpdateCount++;
                result.UpdateCount++;
            }

            sheetResult.Rows.Add(rowResult);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }


    private async Task ImportLimitReferencesAsync(
    IXLWorksheet worksheet,
    MasterDataImportResultDto result,
    CancellationToken cancellationToken)
    {
        var sheetResult =
            result.Sheets.First(x => x.SheetName == "LimitReferences");

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var code = worksheet.Cell(rowNumber, 1).GetString().Trim();
            var name = worksheet.Cell(rowNumber, 2).GetString().Trim();
            var version = worksheet.Cell(rowNumber, 3).GetString().Trim();
            var issueDate =
                TryParseExcelDate(
                    worksheet.Cell(rowNumber, 4).GetString());

            var validFrom =
                TryParseExcelDate(
                    worksheet.Cell(rowNumber, 5).GetString());

            var validTo =
                TryParseExcelDate(
                    worksheet.Cell(rowNumber, 6).GetString());
            var isActive = worksheet.Cell(rowNumber, 7).GetValue<bool>();
            var documentNo = worksheet.Cell(rowNumber, 8).GetString().Trim();
            var priority = worksheet.Cell(rowNumber, 9).GetValue<int>();
            var description = worksheet.Cell(rowNumber, 10).GetString().Trim();

            if (string.IsNullOrWhiteSpace(code) &&
                string.IsNullOrWhiteSpace(name))
                continue;

            sheetResult.TotalRows++;
            result.TotalRows++;

            var rowResult = new MasterDataImportRowResultDto
            {
                SheetName = "LimitReferences",
                RowNumber = rowNumber,
                Code = code,
                IsValid = true
            };

            if (string.IsNullOrWhiteSpace(code))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("Code خالی است.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("Name خالی است.");
            }

            if (!rowResult.IsValid)
            {
                rowResult.Action = "Error";
                sheetResult.ErrorCount++;
                result.ErrorCount++;
                sheetResult.Rows.Add(rowResult);
                continue;
            }

            var existing =
                (await _unitOfWork.LimitReferences.GetAllAsync(
                    cancellationToken))
                .FirstOrDefault(x =>
                    x.Code.Equals(
                        code,
                        StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                var entity = new LimitReference
                {
                    Code = code,
                    Name = name,
                    Version =
                        string.IsNullOrWhiteSpace(version)
                            ? null
                            : version,
                    IssueDate = issueDate,
                    ValidFrom = validFrom ?? DateTime.MinValue,
                    ValidTo = validTo ?? DateTime.MinValue,
                    IsActive = isActive,
                    DocumentNo =
                        string.IsNullOrWhiteSpace(documentNo)
                            ? null
                            : documentNo,
                    Priority = priority,
                    Description =
                        string.IsNullOrWhiteSpace(description)
                            ? null
                            : description
                };

                await _unitOfWork.LimitReferences.AddAsync(entity);

                rowResult.Action = "Insert";
                sheetResult.InsertCount++;
                result.InsertCount++;
            }
            else
            {
                existing.Name = name;
                existing.Version =
                    string.IsNullOrWhiteSpace(version)
                        ? null
                        : version;
                existing.IssueDate = issueDate;
                existing.ValidFrom = validFrom;
                existing.ValidTo = validTo;
                existing.IsActive = isActive;
                existing.DocumentNo =
                    string.IsNullOrWhiteSpace(documentNo)
                        ? null
                        : documentNo;
                existing.Priority = priority;
                existing.Description =
                    string.IsNullOrWhiteSpace(description)
                        ? null
                        : description;

                _unitOfWork.LimitReferences.Update(existing);

                rowResult.Action = "Update";
                sheetResult.UpdateCount++;
                result.UpdateCount++;
            }

            sheetResult.Rows.Add(rowResult);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ImportTestsAsync(
    IXLWorksheet worksheet,
    MasterDataImportResultDto result,
    CancellationToken cancellationToken)
    {
        var sheetResult =
            result.Sheets.First(x => x.SheetName == "Tests");

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        var employees =
            await _unitOfWork.Employees.GetAllAsync();

        var testMethods =
            await _unitOfWork.TestMethods.GetAllAsync();

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var code = worksheet.Cell(rowNumber, 1).GetString().Trim();
            var name = worksheet.Cell(rowNumber, 2).GetString().Trim();
            var englishName = worksheet.Cell(rowNumber, 3).GetString().Trim();
            var testType = worksheet.Cell(rowNumber, 4).GetString().Trim();
            var unit = worksheet.Cell(rowNumber, 5).GetString().Trim();
            var defaultAnalystCode =
                worksheet.Cell(rowNumber, 6).GetString().Trim();
            var methodCode =
                worksheet.Cell(rowNumber, 7).GetString().Trim();
            var referenceStandard =
                worksheet.Cell(rowNumber, 9).GetString().Trim();
            var isActiveText =
                worksheet.Cell(rowNumber, 10).GetString().Trim();
            var description =
                worksheet.Cell(rowNumber, 11).GetString().Trim();

            if (string.IsNullOrWhiteSpace(code) &&
                string.IsNullOrWhiteSpace(name))
                continue;

            sheetResult.TotalRows++;
            result.TotalRows++;

            var rowResult = new MasterDataImportRowResultDto
            {
                SheetName = "Tests",
                RowNumber = rowNumber,
                Code = code,
                IsValid = true
            };

            if (string.IsNullOrWhiteSpace(code))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("Code خالی است.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("Name خالی است.");
            }

            if (!rowResult.IsValid)
            {
                rowResult.Action = "Error";
                sheetResult.ErrorCount++;
                result.ErrorCount++;
                sheetResult.Rows.Add(rowResult);
                continue;
            }

            var isQuantitative =
                testType.Equals(
                    "Quantitative",
                    StringComparison.OrdinalIgnoreCase);

            Employee? defaultAnalyst = null;

            if (!string.IsNullOrWhiteSpace(defaultAnalystCode))
            {
                defaultAnalyst =
                    employees.FirstOrDefault(x =>
                        x.PersonnelCode.Equals(
                            defaultAnalystCode,
                            StringComparison.OrdinalIgnoreCase));

                if (defaultAnalyst == null)
                {
                    rowResult.IsValid = false;
                    rowResult.Errors.Add(
                        $"DefaultAnalyst با Code '{defaultAnalystCode}' پیدا نشد.");
                }
            }

            TestMethod? testMethod = null;

            if (!string.IsNullOrWhiteSpace(methodCode))
            {
                testMethod =
                    testMethods.FirstOrDefault(x =>
                        x.Code.Equals(
                            methodCode,
                            StringComparison.OrdinalIgnoreCase));

                if (testMethod == null)
                {
                    rowResult.IsValid = false;
                    rowResult.Errors.Add(
                        $"TestMethod با Code '{methodCode}' پیدا نشد.");
                }
            }

            if (!rowResult.IsValid)
            {
                rowResult.Action = "Error";
                sheetResult.ErrorCount++;
                result.ErrorCount++;
                sheetResult.Rows.Add(rowResult);
                continue;
            }

            var isActive = true;

            if (!string.IsNullOrWhiteSpace(isActiveText) &&
                bool.TryParse(isActiveText, out var parsedIsActive))
            {
                isActive = parsedIsActive;
            }

            var existing =
                (await _unitOfWork.Tests.GetAllAsync())
                .FirstOrDefault(x =>
                    x.Code.Equals(
                        code,
                        StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                var test = new Test
                {
                    Code = code,
                    Name = name,
                    EnglishName =
                        string.IsNullOrWhiteSpace(englishName)
                            ? null
                            : englishName,
                    Unit =
                        string.IsNullOrWhiteSpace(unit)
                            ? null
                            : unit,
                    IsQuantitative = isQuantitative,
                    IsActiveForReception = isActive,
                    Description =
                        string.IsNullOrWhiteSpace(description)
                            ? null
                            : description,
                    TestMethodId = testMethod?.Id,
                    DefaultAnalystId = defaultAnalyst?.Id
                };

                await _unitOfWork.Tests.AddAsync(test);

                rowResult.Action = "Insert";
                sheetResult.InsertCount++;
                result.InsertCount++;
            }
            else
            {
                existing.Name = name;
                existing.EnglishName =
                    string.IsNullOrWhiteSpace(englishName)
                        ? null
                        : englishName;
                existing.Unit =
                    string.IsNullOrWhiteSpace(unit)
                        ? null
                        : unit;
                existing.IsQuantitative = isQuantitative;
                existing.IsActiveForReception = isActive;
                existing.Description =
                    string.IsNullOrWhiteSpace(description)
                        ? null
                        : description;
                existing.TestMethodId = testMethod?.Id;
                existing.DefaultAnalystId = defaultAnalyst?.Id;

                _unitOfWork.Tests.Update(existing);

                rowResult.Action = "Update";
                sheetResult.UpdateCount++;
                result.UpdateCount++;
            }

            sheetResult.Rows.Add(rowResult);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    private async Task ImportResultDefinitionsAsync(
    IXLWorksheet worksheet,
    MasterDataImportResultDto result,
    CancellationToken cancellationToken)
    {
        var sheetResult =
            result.Sheets.First(x => x.SheetName == "ResultDefinitions");

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        var tests =
            await _unitOfWork.Tests.GetAllAsync();

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var testCode = worksheet.Cell(rowNumber, 1).GetString().Trim();
            var code = worksheet.Cell(rowNumber, 2).GetString().Trim();
            var name = worksheet.Cell(rowNumber, 3).GetString().Trim();
            var displayOrder =
                 worksheet.Cell(rowNumber, 4).GetValue<int>();

            var unit =
                worksheet.Cell(rowNumber, 5).GetString().Trim();

            var isRequired =
                 worksheet.Cell(rowNumber, 6).GetValue<bool>();

            var decimalPlaces =
                worksheet.Cell(rowNumber, 7).IsEmpty()
                    ? (int?)null
                    : worksheet.Cell(rowNumber, 7).GetValue<int>();

            var description =
                worksheet.Cell(rowNumber, 8).GetString().Trim();

            if (string.IsNullOrWhiteSpace(testCode) &&
                string.IsNullOrWhiteSpace(code) &&
                string.IsNullOrWhiteSpace(name))
                continue;

            sheetResult.TotalRows++;
            result.TotalRows++;

            var rowResult = new MasterDataImportRowResultDto
            {
                SheetName = "ResultDefinitions",
                RowNumber = rowNumber,
                Code = code,
                IsValid = true
            };

            if (string.IsNullOrWhiteSpace(testCode))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("TestCode خالی است.");
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("Code خالی است.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("Name خالی است.");
            }

            var test =
                tests.FirstOrDefault(x =>
                    x.Code.Equals(
                        testCode,
                        StringComparison.OrdinalIgnoreCase));

            if (test == null)
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add(
                    $"Test با Code '{testCode}' پیدا نشد.");
            }

            if (!rowResult.IsValid)
            {
                rowResult.Action = "Error";
                sheetResult.ErrorCount++;
                result.ErrorCount++;
                sheetResult.Rows.Add(rowResult);
                continue;
            }

            var definitions =
                await _unitOfWork.TestResultDefinitions
                    .GetByTestIdAsync(
                        test!.Id,
                        cancellationToken);

            var existing =
                definitions.FirstOrDefault(x =>
                    x.Code.Equals(
                        code,
                        StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                var definition = new TestResultDefinition
                {
                    TestId = test.Id,
                    Code = code,
                    Name = name,
                    Unit =
                        string.IsNullOrWhiteSpace(unit)
                            ? null
                            : unit,
                    DisplayOrder = displayOrder,
                    IsRequired = isRequired,
                    DecimalPlaces = decimalPlaces,
                    Description =
                    string.IsNullOrWhiteSpace(description)
                        ? null
                        : description
                                };

                await _unitOfWork.TestResultDefinitions.AddAsync(
                    definition);

                rowResult.Action = "Insert";
                sheetResult.InsertCount++;
                result.InsertCount++;
            }
            else
            {
                existing.Name = name;
                existing.Unit =
                    string.IsNullOrWhiteSpace(unit)
                        ? null
                        : unit;
                existing.DisplayOrder = displayOrder;

                existing.IsRequired = isRequired;
                existing.DecimalPlaces = decimalPlaces;


                existing.Description =
                    string.IsNullOrWhiteSpace(description)
                        ? null
                        : description;

                _unitOfWork.TestResultDefinitions.Update(existing);

                rowResult.Action = "Update";
                sheetResult.UpdateCount++;
                result.UpdateCount++;
            }

            sheetResult.Rows.Add(rowResult);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ImportTestPanelsAsync(
    IXLWorksheet worksheet,
    MasterDataImportResultDto result,
    CancellationToken cancellationToken)
    {
        var sheetResult =
            result.Sheets.First(x => x.SheetName == "TestPanels");

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var code = worksheet.Cell(rowNumber, 1).GetString().Trim();
            var name = worksheet.Cell(rowNumber, 2).GetString().Trim();
            var description =
                worksheet.Cell(rowNumber, 4).GetString().Trim();

            if (string.IsNullOrWhiteSpace(code) &&
                string.IsNullOrWhiteSpace(name))
                continue;

            sheetResult.TotalRows++;
            result.TotalRows++;

            var rowResult = new MasterDataImportRowResultDto
            {
                SheetName = "TestPanels",
                RowNumber = rowNumber,
                Code = code,
                IsValid = true
            };

            if (string.IsNullOrWhiteSpace(code))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("Code خالی است.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("Name خالی است.");
            }

            if (!rowResult.IsValid)
            {
                rowResult.Action = "Error";
                sheetResult.ErrorCount++;
                result.ErrorCount++;
                sheetResult.Rows.Add(rowResult);
                continue;
            }

            var existing =
                (await _unitOfWork.TestPanels.GetAllAsync())
                .FirstOrDefault(x =>
                    x.Code.Equals(
                        code,
                        StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                var entity = new TestPanel
                {
                    Code = code,
                    Name = name,
                    Description =
                        string.IsNullOrWhiteSpace(description)
                            ? null
                            : description
                };

                await _unitOfWork.TestPanels.AddAsync(entity);

                rowResult.Action = "Insert";
                sheetResult.InsertCount++;
                result.InsertCount++;
            }
            else
            {
                existing.Name = name;
                existing.Description =
                    string.IsNullOrWhiteSpace(description)
                        ? null
                        : description;

                _unitOfWork.TestPanels.Update(existing);

                rowResult.Action = "Update";
                sheetResult.UpdateCount++;
                result.UpdateCount++;
            }

            sheetResult.Rows.Add(rowResult);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ImportPanelItemsAsync(
    IXLWorksheet worksheet,
    MasterDataImportResultDto result,
    CancellationToken cancellationToken)
    {
        var sheetResult =
            result.Sheets.First(x => x.SheetName == "PanelItems");

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        var panels =
            await _unitOfWork.TestPanels.GetAllAsync();

        var tests =
            await _unitOfWork.Tests.GetAllAsync();

        var employees =
            await _unitOfWork.Employees.GetAllAsync();

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var panelCode =
                worksheet.Cell(rowNumber, 1).GetString().Trim();

            var testCode =
                worksheet.Cell(rowNumber, 2).GetString().Trim();

            var displayOrder =
                worksheet.Cell(rowNumber, 3).GetValue<int>();

            var defaultAnalystCode =
                worksheet.Cell(rowNumber, 4).GetString().Trim();

            var description =
                worksheet.Cell(rowNumber, 6).GetString().Trim();

            if (string.IsNullOrWhiteSpace(panelCode) &&
                string.IsNullOrWhiteSpace(testCode))
                continue;

            sheetResult.TotalRows++;
            result.TotalRows++;

            var rowResult = new MasterDataImportRowResultDto
            {
                SheetName = "PanelItems",
                RowNumber = rowNumber,
                Code = $"{panelCode}/{testCode}",
                IsValid = true
            };

            if (string.IsNullOrWhiteSpace(panelCode))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("PanelCode خالی است.");
            }

            if (string.IsNullOrWhiteSpace(testCode))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("TestCode خالی است.");
            }

            var panel =
                panels.FirstOrDefault(x =>
                    x.Code.Equals(
                        panelCode,
                        StringComparison.OrdinalIgnoreCase));

            if (panel == null)
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add(
                    $"Panel با Code '{panelCode}' پیدا نشد.");
            }

            var test =
                tests.FirstOrDefault(x =>
                    x.Code.Equals(
                        testCode,
                        StringComparison.OrdinalIgnoreCase));

            if (test == null)
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add(
                    $"Test با Code '{testCode}' پیدا نشد.");
            }

            Employee? defaultAnalyst = null;

            if (!string.IsNullOrWhiteSpace(defaultAnalystCode))
            {
                defaultAnalyst =
                    employees.FirstOrDefault(x =>
                        x.PersonnelCode.Equals(
                            defaultAnalystCode,
                            StringComparison.OrdinalIgnoreCase));

                if (defaultAnalyst == null)
                {
                    rowResult.IsValid = false;
                    rowResult.Errors.Add(
                        $"DefaultAnalyst با Code '{defaultAnalystCode}' پیدا نشد.");
                }
            }

            if (!rowResult.IsValid)
            {
                rowResult.Action = "Error";
                sheetResult.ErrorCount++;
                result.ErrorCount++;
                sheetResult.Rows.Add(rowResult);
                continue;
            }

            var items =
                await _unitOfWork.TestPanelItems
                    .GetByPanelIdAsync(
                        panel!.Id,
                        cancellationToken);

            var existing =
                items.FirstOrDefault(x =>
                    x.TestId == test!.Id);

            if (existing == null)
            {
                var entity = new TestPanelItem
                {
                    TestPanelId = panel.Id,
                    TestId = test.Id,
                    SortOrder = displayOrder,
                    DefaultAnalystId =
                        defaultAnalyst?.Id,
                    IsActive = true
                };

                await _unitOfWork.TestPanelItems.AddAsync(entity);

                rowResult.Action = "Insert";
                sheetResult.InsertCount++;
                result.InsertCount++;
            }
            else
            {
                existing.SortOrder = displayOrder;
                existing.DefaultAnalystId =
                    defaultAnalyst?.Id;

                _unitOfWork.TestPanelItems.Update(existing);

                rowResult.Action = "Update";
                sheetResult.UpdateCount++;
                result.UpdateCount++;
            }

            sheetResult.Rows.Add(rowResult);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }


    private async Task ImportDefaultTestSetsAsync(
    IXLWorkbook workbook,
    MasterDataImportResultDto result,
    CancellationToken cancellationToken)
    {
        const string sheetName = "DefaultTestSets";

        if (!workbook.Worksheets.Any(x => x.Name == sheetName))
            return;

        var ws = workbook.Worksheet(sheetName);
        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;

        var sheetResult = new MasterDataImportSheetResultDto
        {
            SheetName = sheetName
        };

        var customers =
            await _unitOfWork.Customers.GetAllAsync();

        var sampleCategories =
            await _unitOfWork.SampleCategories
                .GetAllAsync(cancellationToken);

        var matrices =
            await _unitOfWork.Matrices
                .GetAllAsync(cancellationToken);

        var standardSamples =
            await _unitOfWork.StandardSamples
                .GetAllAsync(cancellationToken);

        var tests =
            await _unitOfWork.Tests
                .GetAllAsync();

        var panels =
            await _unitOfWork.TestPanels
                .GetAllAsync(cancellationToken);

        var existingSets =
            await _unitOfWork.DefaultTestSets
                .GetAllAsync(cancellationToken);

        for (var row = 2; row <= lastRow; row++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var code =
                ws.Cell(row, 1).GetString().Trim();

            var name =
                ws.Cell(row, 2).GetString().Trim();

            var customerCode =
                ws.Cell(row, 3).GetString().Trim();

            var sampleCategoryCode =
                ws.Cell(row, 4).GetString().Trim();

            var matrixCode =
                ws.Cell(row, 5).GetString().Trim();

            var standardSampleCode =
                ws.Cell(row, 6).GetString().Trim();

            var testCode =
                ws.Cell(row, 7).GetString().Trim();

            var panelCode =
                ws.Cell(row, 8).GetString().Trim();

            var displayOrder =
                ws.Cell(row, 9).GetValue<int>();

            var description =
                ws.Cell(row, 11).GetString().Trim();

            var rowResult = new MasterDataImportRowResultDto
            {
                SheetName = sheetName,
                RowNumber = row,
                Code = code
            };

            // -------------------------------------------------
            // Required fields
            // -------------------------------------------------

            if (string.IsNullOrWhiteSpace(code))
                rowResult.Errors.Add("Code الزامی است.");

            if (string.IsNullOrWhiteSpace(name))
                rowResult.Errors.Add("Name الزامی است.");

            var hasTest =
                !string.IsNullOrWhiteSpace(testCode);

            var hasPanel =
                !string.IsNullOrWhiteSpace(panelCode);

            if (hasTest == hasPanel)
            {
                rowResult.Errors.Add(
                    "دقیقاً یکی از TestCode یا PanelCode باید مقدار داشته باشد.");
            }

            // -------------------------------------------------
            // Resolve Customer
            // -------------------------------------------------

            Guid? customerId = null;

            if (!string.IsNullOrWhiteSpace(customerCode))
            {
                var customer =
                    customers.FirstOrDefault(x =>
                        x.Code.Equals(
                            customerCode,
                            StringComparison.OrdinalIgnoreCase));

                if (customer == null)
                {
                    rowResult.Errors.Add(
                        $"CustomerCode «{customerCode}» پیدا نشد.");
                }
                else
                {
                    customerId = customer.Id;
                }
            }

            // -------------------------------------------------
            // Resolve Sample Category
            // -------------------------------------------------

            Guid? sampleCategoryId = null;

            if (!string.IsNullOrWhiteSpace(sampleCategoryCode))
            {
                var sampleCategory =
                    sampleCategories.FirstOrDefault(x =>
                        x.Code.Equals(
                            sampleCategoryCode,
                            StringComparison.OrdinalIgnoreCase));

                if (sampleCategory == null)
                {
                    rowResult.Errors.Add(
                        $"SampleCategoryCode «{sampleCategoryCode}» پیدا نشد.");
                }
                else
                {
                    sampleCategoryId = sampleCategory.Id;
                }
            }

            // -------------------------------------------------
            // Resolve Matrix
            // -------------------------------------------------

            Guid? matrixId = null;

            if (!string.IsNullOrWhiteSpace(matrixCode))
            {
                var matrix =
                    matrices.FirstOrDefault(x =>
                        x.Code.Equals(
                            matrixCode,
                            StringComparison.OrdinalIgnoreCase));

                if (matrix == null)
                {
                    rowResult.Errors.Add(
                        $"MatrixCode «{matrixCode}» پیدا نشد.");
                }
                else
                {
                    matrixId = matrix.Id;
                }
            }

            // -------------------------------------------------
            // Resolve Standard Sample
            // -------------------------------------------------

            Guid? standardSampleId = null;

            if (!string.IsNullOrWhiteSpace(standardSampleCode))
            {
                var standardSample =
                    standardSamples.FirstOrDefault(x =>
                        x.Code.Equals(
                            standardSampleCode,
                            StringComparison.OrdinalIgnoreCase));

                if (standardSample == null)
                {
                    rowResult.Errors.Add(
                        $"StandardSampleCode «{standardSampleCode}» پیدا نشد.");
                }
                else
                {
                    standardSampleId = standardSample.Id;
                }
            }

            // -------------------------------------------------
            // Resolve Test / Panel
            // -------------------------------------------------

            Guid? testId = null;
            Guid? testPanelId = null;

            if (hasTest)
            {
                var test =
                    tests.FirstOrDefault(x =>
                        x.Code.Equals(
                            testCode,
                            StringComparison.OrdinalIgnoreCase));

                if (test == null)
                {
                    rowResult.Errors.Add(
                        $"TestCode «{testCode}» پیدا نشد.");
                }
                else
                {
                    testId = test.Id;
                }
            }

            if (hasPanel)
            {
                var panel =
                    panels.FirstOrDefault(x =>
                        x.Code.Equals(
                            panelCode,
                            StringComparison.OrdinalIgnoreCase));

                if (panel == null)
                {
                    rowResult.Errors.Add(
                        $"PanelCode «{panelCode}» پیدا نشد.");
                }
                else
                {
                    testPanelId = panel.Id;
                }
            }

            // -------------------------------------------------
            // Validation result
            // -------------------------------------------------

            sheetResult.TotalRows++;
            result.TotalRows++;

            if (rowResult.Errors.Count > 0)
            {
                rowResult.Action = "Error";
                rowResult.IsValid = false;

                sheetResult.ErrorCount++;
                result.ErrorCount++;

                sheetResult.Rows.Add(rowResult);

                continue;
            }

            // -------------------------------------------------
            // Build Item
            // -------------------------------------------------

            var itemDto =
                new DefaultTestSetItemDto
                {
                    Id = Guid.NewGuid(),
                    IsPanel = hasPanel,
                    TestId = testId,
                    TestPanelId = testPanelId,
                    SortOrder = displayOrder
                };

            // -------------------------------------------------
            // Find existing set by Code
            // -------------------------------------------------

            var existing =
                existingSets.FirstOrDefault(x =>
                    x.Code.Equals(
                        code,
                        StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                // -------------------------------------------------
                // New DefaultTestSet
                // -------------------------------------------------

                var dto =
                    new DefaultTestSetDto
                    {
                        Id = Guid.NewGuid(),
                        Code = code,
                        Name = name,
                        Description =
                            string.IsNullOrWhiteSpace(description)
                                ? null
                                : description,
                        CustomerId = customerId,
                        SampleCategoryId = sampleCategoryId,
                        MatrixId = matrixId,
                        StandardSampleId = standardSampleId,
                        Priority = 100,
                        Items = new List<DefaultTestSetItemDto>
                        {
                        itemDto
                        }
                    };

                var entity =
                    new Barman.Domain.Entities.DefaultTestSet
                    {
                        Id = dto.Id,
                        Code = dto.Code,
                        Name = dto.Name,
                        Description = dto.Description,
                        CustomerId = dto.CustomerId,
                        SampleCategoryId = dto.SampleCategoryId,
                        MatrixId = dto.MatrixId,
                        StandardSampleId = dto.StandardSampleId,
                        Priority = dto.Priority,
                        IsActive = true,
                        IsDeleted = false
                    };

                entity.Items.Add(
                    new Barman.Domain.Entities.DefaultTestSetItem
                    {
                        Id = itemDto.Id,
                        DefaultTestSetId = entity.Id,
                        IsPanel = itemDto.IsPanel,
                        TestId = itemDto.TestId,
                        TestPanelId = itemDto.TestPanelId,
                        SortOrder = itemDto.SortOrder,
                        IsActive = true,
                        IsDeleted = false
                    });

                await _unitOfWork.DefaultTestSets
                    .AddAsync(
                        entity,
                        cancellationToken);

                existingSets.Add(entity);

                rowResult.Action = "Insert";
                rowResult.IsValid = true;

                sheetResult.InsertCount++;
                result.InsertCount++;
            }
            else
            {
                // -------------------------------------------------
                // Existing DefaultTestSet
                // -------------------------------------------------

                existing.Name = name;

                existing.Description =
                    string.IsNullOrWhiteSpace(description)
                        ? null
                        : description;

                existing.CustomerId = customerId;
                existing.SampleCategoryId = sampleCategoryId;
                existing.MatrixId = matrixId;
                existing.StandardSampleId = standardSampleId;

                var existingItem =
                    existing.Items.FirstOrDefault(x =>
                        x.IsPanel == itemDto.IsPanel &&
                        x.TestId == itemDto.TestId &&
                        x.TestPanelId == itemDto.TestPanelId);

                if (existingItem == null)
                {
                    existing.Items.Add(
                        new Barman.Domain.Entities.DefaultTestSetItem
                        {
                            Id = Guid.NewGuid(),
                            DefaultTestSetId = existing.Id,
                            IsPanel = itemDto.IsPanel,
                            TestId = itemDto.TestId,
                            TestPanelId = itemDto.TestPanelId,
                            SortOrder = itemDto.SortOrder,
                            IsActive = true,
                            IsDeleted = false
                        });
                }
                else
                {
                    existingItem.SortOrder =
                        itemDto.SortOrder;

                    existingItem.IsActive = true;
                    existingItem.IsDeleted = false;
                }

                _unitOfWork.DefaultTestSets
                    .Update(existing);

                rowResult.Action = "Update";
                rowResult.IsValid = true;

                sheetResult.UpdateCount++;
                result.UpdateCount++;
            }

            sheetResult.Rows.Add(rowResult);
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        result.Sheets.Add(sheetResult);
    }

    private async Task ImportTestLimitRulesAsync(
    IXLWorksheet worksheet,
    MasterDataImportResultDto result,
    CancellationToken cancellationToken)
    {
        var sheetResult =
            result.Sheets.First(x => x.SheetName == "TestLimitRules");

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        var tests =
            await _unitOfWork.Tests.GetAllAsync();

        var limitReferences =
            await _unitOfWork.LimitReferences.GetAllAsync(
                cancellationToken);

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var testCode =
                worksheet.Cell(rowNumber, 1).GetString().Trim();

            var limitReferenceCode =
                worksheet.Cell(rowNumber, 2).GetString().Trim();

            var limitTypeText =
                worksheet.Cell(rowNumber, 3).GetString().Trim();

            var lowerValueText =
                worksheet.Cell(rowNumber, 4).GetString().Trim();

            var upperValueText =
                worksheet.Cell(rowNumber, 5).GetString().Trim();

            var exactValueText =
                worksheet.Cell(rowNumber, 6).GetString().Trim();

            var lowerInclusiveText =
                worksheet.Cell(rowNumber, 7).GetString().Trim();

            var upperInclusiveText =
                worksheet.Cell(rowNumber, 8).GetString().Trim();

            var allowedValues =
                worksheet.Cell(rowNumber, 9).GetString().Trim();

            var unit =
                worksheet.Cell(rowNumber, 10).GetString().Trim();

            var priority =
                worksheet.Cell(rowNumber, 11).GetValue<int>();

            var validFrom =
                TryParseExcelDate(
                    worksheet.Cell(rowNumber, 12).GetString());

            var validTo =
                TryParseExcelDate(
                    worksheet.Cell(rowNumber, 13).GetString());

            var isActiveText =
                worksheet.Cell(rowNumber, 14).GetString().Trim();

            var description =
                worksheet.Cell(rowNumber, 15).GetString().Trim();

            if (string.IsNullOrWhiteSpace(testCode) &&
                string.IsNullOrWhiteSpace(limitReferenceCode))
                continue;

            sheetResult.TotalRows++;
            result.TotalRows++;

            var rowResult = new MasterDataImportRowResultDto
            {
                SheetName = "TestLimitRules",
                RowNumber = rowNumber,
                Code = $"{testCode}/{limitReferenceCode}",
                IsValid = true
            };

            if (string.IsNullOrWhiteSpace(testCode))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("TestCode خالی است.");
            }

            if (string.IsNullOrWhiteSpace(limitReferenceCode))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add("LimitReferenceCode خالی است.");
            }

            var test =
                tests.FirstOrDefault(x =>
                    x.Code.Equals(
                        testCode,
                        StringComparison.OrdinalIgnoreCase));

            if (test == null)
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add(
                    $"Test با Code '{testCode}' پیدا نشد.");
            }

            var limitReference =
                limitReferences.FirstOrDefault(x =>
                    x.Code.Equals(
                        limitReferenceCode,
                        StringComparison.OrdinalIgnoreCase));

            if (limitReference == null)
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add(
                    $"LimitReference با Code '{limitReferenceCode}' پیدا نشد.");
            }

            if (!Enum.TryParse<LimitType>(
                    limitTypeText,
                    true,
                    out var limitType))
            {
                rowResult.IsValid = false;
                rowResult.Errors.Add(
                    $"LimitType مقدار نامعتبر '{limitTypeText}' دارد.");
            }

            decimal? ParseDecimal(
                string value,
                string fieldName)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return null;

                if (decimal.TryParse(
                        value,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out var parsed))
                {
                    return parsed;
                }

                rowResult.IsValid = false;
                rowResult.Errors.Add(
                    $"{fieldName} مقدار عددی نامعتبر دارد.");

                return null;
            }

            var lowerValue =
                ParseDecimal(lowerValueText, "LowerValue");

            var upperValue =
                ParseDecimal(upperValueText, "UpperValue");

            var exactValue =
                ParseDecimal(exactValueText, "ExactValue");

            bool? ParseBool(
                string value,
                string fieldName)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return null;

                if (bool.TryParse(value, out var parsed))
                    return parsed;

                rowResult.IsValid = false;
                rowResult.Errors.Add(
                    $"{fieldName} مقدار True/False نامعتبر دارد.");

                return null;
            }

            var lowerInclusive =
                ParseBool(
                    lowerInclusiveText,
                    "LowerInclusive");

            var upperInclusive =
                ParseBool(
                    upperInclusiveText,
                    "UpperInclusive");

            var isActive = true;

            if (!string.IsNullOrWhiteSpace(isActiveText))
            {
                if (bool.TryParse(isActiveText, out var parsed))
                    isActive = parsed;
                else
                {
                    rowResult.IsValid = false;
                    rowResult.Errors.Add(
                        "IsActive مقدار True/False نامعتبر دارد.");
                }
            }

            if (!rowResult.IsValid)
            {
                rowResult.Action = "Error";
                sheetResult.ErrorCount++;
                result.ErrorCount++;
                sheetResult.Rows.Add(rowResult);
                continue;
            }

            var existingRules =
                await _unitOfWork.TestLimitRules
                    .GetByTestIdAsync(test!.Id);

            var existing =
                existingRules.FirstOrDefault(x =>
                    x.LimitReferenceId ==
                        limitReference!.Id &&
                    x.LimitType == limitType);

            if (existing == null)
            {
                var entity = new TestLimitRule
                {
                    TestId = test.Id,
                    LimitReferenceId = limitReference.Id,
                    LimitType = limitType,
                    LowerValue = lowerValue,
                    UpperValue = upperValue,
                    ExactValue = exactValue,
                    LowerInclusive =
                        lowerInclusive ?? true,
                    UpperInclusive =
                        upperInclusive ?? true,
                    AllowedValues =
                        string.IsNullOrWhiteSpace(allowedValues)
                            ? null
                            : allowedValues,
                    Unit =
                        string.IsNullOrWhiteSpace(unit)
                            ? null
                            : unit,
                    Priority = priority,
                    ValidFrom = validFrom ?? DateTime.MinValue,
                    ValidTo = validTo ?? DateTime.MinValue,
                    IsActive = isActive,
                    Description =
                        string.IsNullOrWhiteSpace(description)
                            ? null
                            : description
                };

                await _unitOfWork.TestLimitRules.AddAsync(entity);

                rowResult.Action = "Insert";
                sheetResult.InsertCount++;
                result.InsertCount++;
            }
            else
            {
                existing.LowerValue = lowerValue;
                existing.UpperValue = upperValue;
                existing.ExactValue = exactValue;
                existing.LowerInclusive =
                    lowerInclusive ?? true;
                existing.UpperInclusive =
                    upperInclusive ?? true;
                existing.AllowedValues =
                    string.IsNullOrWhiteSpace(allowedValues)
                        ? null
                        : allowedValues;
                existing.Unit =
                    string.IsNullOrWhiteSpace(unit)
                        ? null
                        : unit;
                existing.Priority = priority;
                existing.ValidFrom = validFrom ?? DateTime.MinValue;
                existing.ValidTo = validTo ?? DateTime.MinValue;
                existing.IsActive = isActive;
                existing.Description =
                    string.IsNullOrWhiteSpace(description)
                        ? null
                        : description;

                _unitOfWork.TestLimitRules.Update(existing);

                rowResult.Action = "Update";
                sheetResult.UpdateCount++;
                result.UpdateCount++;
            }

            sheetResult.Rows.Add(rowResult);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}