using Barman.Application.Interfaces;
using Barman.Application.Interfaces.Excel;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Globalization;

namespace Barman.Infrastructure.Services.Excel;

public class ExcelExportService : IExcelExportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDefaultTestSetService _defaultTestSetService;

    public ExcelExportService(
        IUnitOfWork unitOfWork,
        IDefaultTestSetService defaultTestSetService)
    {
        _unitOfWork = unitOfWork;
        _defaultTestSetService = defaultTestSetService;
    }

    // =========================================================
    // Normal XLSX Export
    // =========================================================

    public async Task<byte[]> ExportTestTariffsAsync(
        CancellationToken cancellationToken = default)
    {
        var tariffs =
            await _unitOfWork.TestTariffs.GetAllAsync();

        using var workbook = new XLWorkbook();

        var worksheet =
            workbook.Worksheets.Add("تعرفه‌ها");

        var headers = new[]
        {
            "OrganizationCode",
            "OrganizationName",
            "CustomerCode",
            "CustomerName",
            "SampleCategoryCode",
            "SampleCategoryName",
            "StandardSampleCode",
            "StandardSampleName",
            "MatrixCode",
            "MatrixName",
            "TargetType",
            "TestCode",
            "TestName",
            "PanelCode",
            "PanelName",
            "TariffCode",
            "TariffName",
            "Price",
            "Currency",
            "ValidFrom",
            "ValidTo",
            "Priority",
            "IsActive",
            "Description"
        };

        for (var i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
        }

        var headerRange =
            worksheet.Range(
                1,
                1,
                1,
                headers.Length);

        headerRange.Style.Font.Bold = true;

        headerRange.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        var row = 2;

        foreach (var tariff in tariffs)
        {
            cancellationToken.ThrowIfCancellationRequested();

            worksheet.Cell(row, 1).Value =
                tariff.OrganizationType?.Code ?? "";

            worksheet.Cell(row, 2).Value =
                tariff.OrganizationType?.Name ?? "";

            worksheet.Cell(row, 3).Value =
                tariff.Customer?.Code ?? "";

            worksheet.Cell(row, 4).Value =
                tariff.Customer?.DisplayName ?? "";

            worksheet.Cell(row, 5).Value =
                tariff.SampleCategory?.Code ?? "";

            worksheet.Cell(row, 6).Value =
                tariff.SampleCategory?.Name ?? "";

            worksheet.Cell(row, 7).Value =
                tariff.StandardSample?.Code ?? "";

            worksheet.Cell(row, 8).Value =
                tariff.StandardSample?.Name ?? "";

            worksheet.Cell(row, 9).Value =
                tariff.Matrix?.Code ?? "";

            worksheet.Cell(row, 10).Value =
                tariff.Matrix?.Name ?? "";

            worksheet.Cell(row, 11).Value =
                tariff.TestId.HasValue
                    ? "Test"
                    : "Panel";

            worksheet.Cell(row, 12).Value =
                tariff.Test?.Code ?? "";

            worksheet.Cell(row, 13).Value =
                tariff.Test?.Name ?? "";

            worksheet.Cell(row, 14).Value =
                tariff.TestPanel?.Code ?? "";

            worksheet.Cell(row, 15).Value =
                tariff.TestPanel?.Name ?? "";

            worksheet.Cell(row, 16).Value =
                tariff.Code;

            worksheet.Cell(row, 17).Value =
                tariff.Name;

            worksheet.Cell(row, 18).Value =
                tariff.Price;

            worksheet.Cell(row, 19).Value =
                tariff.Currency;

            worksheet.Cell(row, 20).Value =
                FormatPersianDate(tariff.ValidFrom);

            worksheet.Cell(row, 21).Value =
                tariff.ValidTo.HasValue
                    ? FormatPersianDate(tariff.ValidTo.Value)
                    : "";

            worksheet.Cell(row, 22).Value =
                tariff.Priority;

            worksheet.Cell(row, 23).Value =
                tariff.IsActive;

            worksheet.Cell(row, 24).Value =
                tariff.Description ?? "";

            row++;
        }

        worksheet.SheetView.FreezeRows(1);

        worksheet.AutoFilter.Clear();

        worksheet.Range(
                1,
                1,
                Math.Max(row - 1, 1),
                headers.Length)
            .SetAutoFilter();

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        return stream.ToArray();
    }


    // =========================================================
    // XLSM Template Export
    // =========================================================

    public async Task<byte[]> ExportTestTariffTemplateAsync(
        CancellationToken cancellationToken = default)
    {
        // -----------------------------------------------------
        // Load Master Data
        // -----------------------------------------------------

        var organizations =
            await _unitOfWork.OrganizationTypes.GetAllAsync();

        var customers =
            await _unitOfWork.Customers.GetAllAsync();

        var sampleCategories =
            await _unitOfWork.SampleCategories.GetAllAsync();

        var standardSamples =
            await _unitOfWork.StandardSamples.GetAllAsync(
                cancellationToken);

        var matrices =
            await _unitOfWork.Matrices.GetAllAsync();

        var tests =
            await _unitOfWork.Tests.GetAllAsync();

        var panels =
            await _unitOfWork.TestPanels.GetAllAsync();

        var tariffs =
            await _unitOfWork.TestTariffs.GetAllAsync();

        cancellationToken.ThrowIfCancellationRequested();


        // -----------------------------------------------------
        // Master XLSM Template
        // -----------------------------------------------------

        var templatePath =
            Path.Combine(
                AppContext.BaseDirectory,
                "Templates",
                "Excel",
                "price.xlsm");

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException(
                "فایل Master Excel Template پیدا نشد.",
                templatePath);
        }


        // -----------------------------------------------------
        // Copy original XLSM into memory
        // -----------------------------------------------------

        using var memoryStream =
            new MemoryStream();

        await using (var fileStream =
            new FileStream(
                templatePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
        {
            await fileStream.CopyToAsync(
                memoryStream,
                cancellationToken);
        }

        memoryStream.Position = 0;


        // -----------------------------------------------------
        // Open existing XLSM
        //
        // IMPORTANT:
        // We do NOT create a new workbook.
        // Therefore VBA + ActiveX + VML + existing structure
        // remain inside the file.
        // -----------------------------------------------------

        using (var document =
            SpreadsheetDocument.Open(
                memoryStream,
                true))
        {
            var workbookPart =
                document.WorkbookPart
                ?? throw new InvalidOperationException(
                    "WorkbookPart پیدا نشد.");

            var sheets =
                workbookPart.Workbook
                    .GetFirstChild<Sheets>()
                ?? throw new InvalidOperationException(
                    "Sheets پیدا نشد.");


            // =================================================
            // Sheet1
            // =================================================

            var sheet1 =
                sheets.Elements<Sheet>()
                    .FirstOrDefault(x =>
                        x.Name?.Value == "Sheet1");

            if (sheet1 == null)
            {
                throw new InvalidOperationException(
                    "Sheet1 در price.xlsm پیدا نشد.");
            }

            var sheet1Part =
                (WorksheetPart)
                    workbookPart.GetPartById(
                        sheet1.Id!.Value!);


            // Remove old data only.
            // Header / VBA / ActiveX / sheet structure stay.
            ClearRows(
                sheet1Part,
                2,
                10000,
                1,
                24);


            // =================================================
            // Write Tariffs to Sheet1
            // =================================================

            for (var i = 0; i < tariffs.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var tariff =
                    tariffs[i];

                var row =
                    i + 2;


                SetCell(
                    sheet1Part,
                    row,
                    1,
                    tariff.OrganizationType?.Code ?? "");

                SetCell(
                    sheet1Part,
                    row,
                    2,
                    tariff.OrganizationType?.Name ?? "");

                SetCell(
                    sheet1Part,
                    row,
                    3,
                    tariff.Customer?.Code ?? "");

                SetCell(
                    sheet1Part,
                    row,
                    4,
                    tariff.Customer?.DisplayName ?? "");

                SetCell(
                    sheet1Part,
                    row,
                    5,
                    tariff.SampleCategory?.Code ?? "");

                SetCell(
                    sheet1Part,
                    row,
                    6,
                    tariff.SampleCategory?.Name ?? "");

                SetCell(
                    sheet1Part,
                    row,
                    7,
                    tariff.StandardSample?.Code ?? "");

                SetCell(
                    sheet1Part,
                    row,
                    8,
                    tariff.StandardSample?.Name ?? "");

                SetCell(
                    sheet1Part,
                    row,
                    9,
                    tariff.Matrix?.Code ?? "");

                SetCell(
                    sheet1Part,
                    row,
                    10,
                    tariff.Matrix?.Name ?? "");

                SetCell(
                    sheet1Part,
                    row,
                    11,
                    tariff.TestId.HasValue
                        ? "Test"
                        : "Panel");

                SetCell(
                    sheet1Part,
                    row,
                    12,
                    tariff.Test?.Code ?? "");

                SetCell(
                    sheet1Part,
                    row,
                    13,
                    tariff.Test?.Name ?? "");

                SetCell(
                    sheet1Part,
                    row,
                    14,
                    tariff.TestPanel?.Code ?? "");

                SetCell(
                    sheet1Part,
                    row,
                    15,
                    tariff.TestPanel?.Name ?? "");

                SetCell(
                    sheet1Part,
                    row,
                    16,
                    tariff.Code ?? "");

                SetCell(
                    sheet1Part,
                    row,
                    17,
                    tariff.Name ?? "");

                SetCell(
                    sheet1Part,
                    row,
                    18,
                    tariff.Price);

                SetCell(
                    sheet1Part,
                    row,
                    19,
                    tariff.Currency ?? "");

                SetCell(
                    sheet1Part,
                    row,
                    20,
                    FormatPersianDate(
                        tariff.ValidFrom));

                SetCell(
                    sheet1Part,
                    row,
                    21,
                    tariff.ValidTo.HasValue
                        ? FormatPersianDate(
                            tariff.ValidTo.Value)
                        : "");

                SetCell(
                    sheet1Part,
                    row,
                    22,
                    tariff.Priority);

                SetCell(
                    sheet1Part,
                    row,
                    23,
                    tariff.IsActive);

                SetCell(
                    sheet1Part,
                    row,
                    24,
                    tariff.Description ?? "");
            }


            // =================================================
            // Lookup
            // =================================================

            var lookupSheet =
                sheets.Elements<Sheet>()
                    .FirstOrDefault(x =>
                        x.Name?.Value == "Lookup");

            if (lookupSheet == null)
            {
                throw new InvalidOperationException(
                    "Lookup در price.xlsm پیدا نشد.");
            }

            var lookupPart =
                (WorksheetPart)
                    workbookPart.GetPartById(
                        lookupSheet.Id!.Value!);


            // Remove old Lookup data only.
            ClearRows(
                lookupPart,
                2,
                10000,
                1,
                14);


            // -------------------------------------------------
            // Organization A:B
            // -------------------------------------------------

            for (var i = 0; i < organizations.Count; i++)
            {
                var row =
                    i + 2;

                SetCell(
                    lookupPart,
                    row,
                    1,
                    organizations[i].Code ?? "");

                SetCell(
                    lookupPart,
                    row,
                    2,
                    organizations[i].Name ?? "");
            }


            // -------------------------------------------------
            // Customer C:D
            // -------------------------------------------------

            for (var i = 0; i < customers.Count; i++)
            {
                var row =
                    i + 2;

                SetCell(
                    lookupPart,
                    row,
                    3,
                    customers[i].Code ?? "");

                SetCell(
                    lookupPart,
                    row,
                    4,
                    customers[i].DisplayName ?? "");
            }


            // -------------------------------------------------
            // Sample Category E:F
            // -------------------------------------------------

            for (var i = 0; i < sampleCategories.Count; i++)
            {
                var row =
                    i + 2;

                SetCell(
                    lookupPart,
                    row,
                    5,
                    sampleCategories[i].Code ?? "");

                SetCell(
                    lookupPart,
                    row,
                    6,
                    sampleCategories[i].Name ?? "");
            }


            // -------------------------------------------------
            // Standard Sample G:H
            // -------------------------------------------------

            for (var i = 0; i < standardSamples.Count; i++)
            {
                var row =
                    i + 2;

                SetCell(
                    lookupPart,
                    row,
                    7,
                    standardSamples[i].Code ?? "");

                SetCell(
                    lookupPart,
                    row,
                    8,
                    standardSamples[i].Name ?? "");
            }


            // -------------------------------------------------
            // Matrix I:J
            // -------------------------------------------------

            for (var i = 0; i < matrices.Count; i++)
            {
                var row =
                    i + 2;

                SetCell(
                    lookupPart,
                    row,
                    9,
                    matrices[i].Code ?? "");

                SetCell(
                    lookupPart,
                    row,
                    10,
                    matrices[i].Name ?? "");
            }


            // -------------------------------------------------
            // Test K:L
            // -------------------------------------------------

            for (var i = 0; i < tests.Count; i++)
            {
                var row =
                    i + 2;

                SetCell(
                    lookupPart,
                    row,
                    11,
                    tests[i].Code ?? "");

                SetCell(
                    lookupPart,
                    row,
                    12,
                    tests[i].Name ?? "");
            }


            // -------------------------------------------------
            // Panel M:N
            // -------------------------------------------------

            for (var i = 0; i < panels.Count; i++)
            {
                var row =
                    i + 2;

                SetCell(
                    lookupPart,
                    row,
                    13,
                    panels[i].Code ?? "");

                SetCell(
                    lookupPart,
                    row,
                    14,
                    panels[i].Name ?? "");
            }


            // =================================================
            // Update worksheet dimensions
            // =================================================

            UpdateDimension(
                sheet1Part,
                Math.Max(tariffs.Count + 1, 1),
                24);

            UpdateDimension(
                lookupPart,
                Math.Max(
                    new[]
                    {
                        organizations.Count,
                        customers.Count,
                        sampleCategories.Count,
                        standardSamples.Count,
                        matrices.Count,
                        tests.Count,
                        panels.Count
                    }.Max() + 1,
                    1),
                14);


            // =================================================
            // Save
            // =================================================

            sheet1Part.Worksheet.Save();

            lookupPart.Worksheet.Save();

            workbookPart.Workbook.Save();
        }


        memoryStream.Position = 0;

        return memoryStream.ToArray();
    }

    public async Task<byte[]> ExportBarmanMasterDataAsync(
    CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tests =
            await _unitOfWork.Tests.GetAllAsync();

        var panels =
            await _unitOfWork.TestPanels.GetAllAsync(
                cancellationToken);

        var panelItems = new List<Barman.Domain.Entities.TestPanelItem>();

        foreach (var panel in panels)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var items =
                await _unitOfWork.TestPanelItems.GetByPanelIdAsync(
                    panel.Id,
                    cancellationToken);

            panelItems.AddRange(items);
        }
        var customers =
    await _unitOfWork.Customers.GetAllAsync();

        var sampleCategories =
            await _unitOfWork.SampleCategories.GetAllAsync(
                cancellationToken);

        var matrices =
            await _unitOfWork.Matrices.GetAllAsync(
                cancellationToken);

        var standardSamples =
            await _unitOfWork.StandardSamples.GetAllAsync(
                cancellationToken);

        var employees =
    await _unitOfWork.Employees.GetAllAsync();

        var departments =
            await _unitOfWork.Departments.GetAllAsync();

        var roles =
            await _unitOfWork.Roles.GetAllAsync();

        var departmentResponsibilities = new List<Barman.Domain.Entities.DepartmentResponsibility>();

        foreach (var employee in employees)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var responsibilities =
                await _unitOfWork.DepartmentResponsibilities
                    .GetByEmployeeIdAsync(employee.Id);

            departmentResponsibilities.AddRange(responsibilities);
        }

        var instruments =
    await _unitOfWork.Instruments.GetAllAsync();


        var limitReferences =
       await _unitOfWork.LimitReferences.GetAllAsync(
           cancellationToken);

        var testLimitRules =
            new List<Barman.Domain.Entities.TestLimitRule>();

        foreach (var test in tests)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rules =
                await _unitOfWork.TestLimitRules.GetByTestIdAsync(
                    test.Id);

            testLimitRules.AddRange(rules);
        }



        var testMethods =
    await _unitOfWork.TestMethods.GetAllAsync();

        var resultDefinitions =
            new List<Barman.Domain.Entities.TestResultDefinition>();

        foreach (var test in tests)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var definitions =
                await _unitOfWork.TestResultDefinitions.GetByTestIdAsync(
                    test.Id,
                    cancellationToken);

            resultDefinitions.AddRange(definitions);
        }





        var templatePath =
            Path.Combine(
                AppContext.BaseDirectory,
                "Templates",
                "Excel",
                "BarmanMasterData.xlsm");

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException(
                "فایل BarmanMasterData.xlsm پیدا نشد.",
                templatePath);
        }

        using var memoryStream = new MemoryStream();

        await using (var fileStream =
            new FileStream(
                templatePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
        {
            await fileStream.CopyToAsync(
                memoryStream,
                cancellationToken);
        }

        memoryStream.Position = 0;

        using (var document =
            SpreadsheetDocument.Open(
                memoryStream,
                true))
        {
            var workbookPart =
                document.WorkbookPart
                ?? throw new InvalidOperationException(
                    "WorkbookPart پیدا نشد.");

            var sheets =
                workbookPart.Workbook
                    .GetFirstChild<Sheets>()
                ?? throw new InvalidOperationException(
                    "Sheets پیدا نشد.");


            // =====================================================
            // Lookup
            // =====================================================

            var lookupSheet =
                sheets.Elements<Sheet>()
                    .FirstOrDefault(x =>
                        x.Name?.Value == "Lookup")
                ?? throw new InvalidOperationException(
                    "Sheet Lookup پیدا نشد.");

            var lookupPart =
                (WorksheetPart)workbookPart.GetPartById(
                    lookupSheet.Id!.Value);

            ClearRows(
                lookupPart,
                2,
                10000,
                1,
                33);

            var lookupHeaders = new[]
            {
                "OrganizationCode",
                "OrganizationName",
                "CustomerCode",
                "CustomerName",
                "SampleCategoryCode",
                "SampleCategoryName",
                "StandardSampleCode",
                "StandardSampleName",
                "MatrixCode",
                "MatrixName",
                "TestCode",
                "TestName",
                "PanelCode",
                "PanelName",
                "EmployeeCode",
                "EmployeeName",
                "DepartmentCode",
                "DepartmentName",
                "InstrumentCode",
                "InstrumentName",
                "LimitReferenceCode",
                "LimitReferenceName",
                "TestMethodCode",
                "TestMethodName",
                "AnalystPhraseCode",
                "AnalystPhrase",
                "ResultDefinitionTestCode",
                "ResultDefinitionCode",
                "ResultDefinitionName",
                "RoleCode",
                "RoleName",
                "ResponsibilityTypeCode",
                "ResponsibilityTypeName"
            };

            for (var i = 0; i < lookupHeaders.Length; i++)
            {
                SetCell(
                    lookupPart,
                    1,
                    i + 1,
                    lookupHeaders[i]);
            }

            // =====================================================
            // Lookup source data
            // Each lookup list starts from row 2 independently.
            // =====================================================

            var sortedCustomers =
                customers
                    .OrderBy(x => x.Code)
                    .ToList();

            var sortedSampleCategories =
                sampleCategories
                    .OrderBy(x => x.Code)
                    .ToList();

            var sortedStandardSamples =
                standardSamples
                    .OrderBy(x => x.Code)
                    .ToList();

            var sortedMatrices =
                matrices
                    .OrderBy(x => x.Code)
                    .ToList();

            var sortedTests =
                tests
                    .OrderBy(x => x.Code)
                    .ToList();

            var sortedPanels =
                panels
                    .OrderBy(x => x.Code)
                    .ToList();

            var sortedEmployees =
                employees
                    .OrderBy(x => x.PersonnelCode)
                    .ToList();

            var sortedDepartments =
                departments
                    .OrderBy(x => x.Code)
                    .ToList();

            var sortedInstruments =
                instruments
                    .OrderBy(x => x.Code)
                    .ToList();

            var sortedLimitReferences =
                limitReferences
                    .OrderBy(x => x.Code)
                    .ToList();

            var sortedTestMethods =
                testMethods
                    .OrderBy(x => x.Code)
                    .ToList();

            var sortedResultDefinitions =
                resultDefinitions
                    .OrderBy(x => x.Test?.Code)
                    .ThenBy(x => x.DisplayOrder)
                    .ToList();

            var sortedRoles =
                roles
                    .OrderBy(x => x.Code)
                    .ToList();

            var responsibilityTypes =
                Enum.GetValues<
                    Barman.Domain.Entities.DepartmentResponsibilityType>();

            // =====================================================
            // Customers - C:D
            // =====================================================

            for (var i = 0; i < sortedCustomers.Count; i++)
            {
                var row = i + 2;
                var customer = sortedCustomers[i];

                SetCell(
                    lookupPart,
                    row,
                    3,
                    customer.Code);

                SetCell(
                    lookupPart,
                    row,
                    4,
                    customer.DisplayName);
            }

            // =====================================================
            // Sample Categories - E:F
            // =====================================================

            for (var i = 0; i < sortedSampleCategories.Count; i++)
            {
                var row = i + 2;
                var category = sortedSampleCategories[i];

                SetCell(
                    lookupPart,
                    row,
                    5,
                    category.Code);

                SetCell(
                    lookupPart,
                    row,
                    6,
                    category.Name);
            }

            // =====================================================
            // Standard Samples - G:H
            // =====================================================

            for (var i = 0; i < sortedStandardSamples.Count; i++)
            {
                var row = i + 2;
                var sample = sortedStandardSamples[i];

                SetCell(
                    lookupPart,
                    row,
                    7,
                    sample.Code);

                SetCell(
                    lookupPart,
                    row,
                    8,
                    sample.Name);
            }

            // =====================================================
            // Matrices - I:J
            // =====================================================

            for (var i = 0; i < sortedMatrices.Count; i++)
            {
                var row = i + 2;
                var matrix = sortedMatrices[i];

                SetCell(
                    lookupPart,
                    row,
                    9,
                    matrix.Code);

                SetCell(
                    lookupPart,
                    row,
                    10,
                    matrix.Name);
            }

            // =====================================================
            // Tests - K:L
            // =====================================================

            for (var i = 0; i < sortedTests.Count; i++)
            {
                var row = i + 2;
                var test = sortedTests[i];

                SetCell(
                    lookupPart,
                    row,
                    11,
                    test.Code);

                SetCell(
                    lookupPart,
                    row,
                    12,
                    test.Name);
            }

            // =====================================================
            // Test Panels - M:N
            // =====================================================

            for (var i = 0; i < sortedPanels.Count; i++)
            {
                var row = i + 2;
                var panel = sortedPanels[i];

                SetCell(
                    lookupPart,
                    row,
                    13,
                    panel.Code);

                SetCell(
                    lookupPart,
                    row,
                    14,
                    panel.Name);
            }

            // =====================================================
            // Employees - O:P
            // =====================================================

            for (var i = 0; i < sortedEmployees.Count; i++)
            {
                var row = i + 2;
                var employee = sortedEmployees[i];

                SetCell(
                    lookupPart,
                    row,
                    15,
                    employee.PersonnelCode);

                SetCell(
                    lookupPart,
                    row,
                    16,
                    employee.FullName);
            }

            // =====================================================
            // Departments - Q:R
            // =====================================================

            for (var i = 0; i < sortedDepartments.Count; i++)
            {
                var row = i + 2;
                var department = sortedDepartments[i];

                SetCell(
                    lookupPart,
                    row,
                    17,
                    department.Code);

                SetCell(
                    lookupPart,
                    row,
                    18,
                    department.Name);
            }

            // =====================================================
            // Instruments - S:T
            // =====================================================

            for (var i = 0; i < sortedInstruments.Count; i++)
            {
                var row = i + 2;
                var instrument = sortedInstruments[i];

                SetCell(
                    lookupPart,
                    row,
                    19,
                    instrument.Code);

                SetCell(
                    lookupPart,
                    row,
                    20,
                    instrument.Name);
            }

            // =====================================================
            // Limit References - U:V
            // =====================================================

            for (var i = 0; i < sortedLimitReferences.Count; i++)
            {
                var row = i + 2;
                var reference = sortedLimitReferences[i];

                SetCell(
                    lookupPart,
                    row,
                    21,
                    reference.Code);

                SetCell(
                    lookupPart,
                    row,
                    22,
                    reference.Name);
            }

            // =====================================================
            // Test Methods - W:X
            // =====================================================

            for (var i = 0; i < sortedTestMethods.Count; i++)
            {
                var row = i + 2;
                var method = sortedTestMethods[i];

                SetCell(
                    lookupPart,
                    row,
                    23,
                    method.Code);

                SetCell(
                    lookupPart,
                    row,
                    24,
                    method.Name);
            }

            // =====================================================
            // Result Definitions - AA:AC
            // =====================================================

            for (var i = 0; i < sortedResultDefinitions.Count; i++)
            {
                var row = i + 2;
                var definition = sortedResultDefinitions[i];

                SetCell(
                    lookupPart,
                    row,
                    27,
                    definition.Test?.Code);

                SetCell(
                    lookupPart,
                    row,
                    28,
                    definition.Code);

                SetCell(
                    lookupPart,
                    row,
                    29,
                    definition.Name);
            }

            // =====================================================
            // Roles - AD:AE
            // =====================================================

            for (var i = 0; i < sortedRoles.Count; i++)
            {
                var row = i + 2;
                var role = sortedRoles[i];

                SetCell(
                    lookupPart,
                    row,
                    30,
                    role.Code);

                SetCell(
                    lookupPart,
                    row,
                    31,
                    role.Name);
            }

            // 12. Department Responsibilities
            {
                var sheet = sheets
                    .OfType<Sheet>()
                    .FirstOrDefault(
                        x => x.Name?.Value == "DepartmentResponsibilities");

                if (sheet != null)
                {
                    var wsPart = (WorksheetPart)document.WorkbookPart!
                        .GetPartById(sheet.Id!.Value!);

                    ClearRows(wsPart, 2, 10000, 1, 8);

                    var headers = new[]
                    {
            "Code",
            "DepartmentCode",
            "EmployeeCode",
            "ResponsibilityType",
            "IsPrimary",
            "Description"
        };

                    for (var col = 0; col < headers.Length; col++)
                        SetCell(wsPart, 1, col + 1, headers[col]);

                    var row = 2;

                    foreach (var item in departmentResponsibilities
                                 .OrderBy(x => x.Department?.Code)
                                 .ThenBy(x => x.Employee?.PersonnelCode))
                    {
                        SetCell(wsPart, row, 1, item.Id.ToString());
                        SetCell(wsPart, row, 2, item.Department?.Code);
                        SetCell(wsPart, row, 3, item.Employee?.PersonnelCode);

                        var responsibilityType =
                            item.ResponsibilityType switch
                            {
                                Barman.Domain.Entities.DepartmentResponsibilityType.TechnicalManager
                                    => "TECHNICAL_MANAGER",

                                Barman.Domain.Entities.DepartmentResponsibilityType.SectionHead
                                    => "SECTION_HEAD",

                                _ => null
                            };

                        SetCell(wsPart, row, 4, responsibilityType);
                        SetCell(wsPart, row, 5, item.IsPrimary);
                        SetCell(wsPart, row, 6, null);

                        row++;
                    }

                    UpdateDimension(
                        wsPart,
                        Math.Max(row - 1, 1),
                        headers.Length);

                    // Synchronize the Excel table definition with the new 6-column structure.
                    var tablePart = wsPart.TableDefinitionParts.FirstOrDefault();

                    if (tablePart?.Table != null)
                    {
                        var table = tablePart.Table;

                        var lastRow = Math.Max(row - 1, 1);

                        table.Reference = $"A1:F{lastRow}";

                        if (table.AutoFilter != null)
                        {
                            table.AutoFilter.Reference = $"A1:F{lastRow}";
                        }

                        var tableColumns = table.TableColumns;

                        if (tableColumns != null)
                        {
                            var existingColumns =
                                tableColumns
                                    .Elements<DocumentFormat.OpenXml.Spreadsheet.TableColumn>()
                                    .ToList();

                            foreach (var column in existingColumns.Skip(6))
                            {
                                tableColumns.RemoveChild(column);
                            }

                            var columnNames = new[]
                            {
            "Code",
            "DepartmentCode",
            "EmployeeCode",
            "ResponsibilityType",
            "IsPrimary",
            "Description"
        };

                            var remainingColumns =
                                tableColumns
                                    .Elements<DocumentFormat.OpenXml.Spreadsheet.TableColumn>()
                                    .ToList();

                            for (var i = 0; i < remainingColumns.Count && i < 6; i++)
                            {
                                remainingColumns[i].Id = (uint)(i + 1);
                                remainingColumns[i].Name = columnNames[i];
                            }

                            tableColumns.Count = 6U;
                        }

                        tablePart.Table.Save();
                    }


                }
            }

            // 13. Responsibility Types
            {
                var sheet = sheets
                    .OfType<Sheet>()
                    .FirstOrDefault(
                        x => x.Name?.Value == "ResponsibilityTypes");

                if (sheet != null)
                {
                    var wsPart = (WorksheetPart)document.WorkbookPart!
                        .GetPartById(sheet.Id!.Value!);

                    ClearRows(wsPart, 2, 10000, 1, 5);

                    var headers = new[]
                    {
            "Code",
            "Name",
            "EnglishName",
            "IsActive",
            "Description"
        };

                    for (var col = 0; col < headers.Length; col++)
                        SetCell(wsPart, 1, col + 1, headers[col]);

                    SetCell(wsPart, 2, 1, "TECHNICAL_MANAGER");
                    SetCell(wsPart, 2, 2, "مسئول فنی");
                    SetCell(wsPart, 2, 3, "Technical Manager");
                    SetCell(wsPart, 2, 4, true);

                    SetCell(wsPart, 3, 1, "SECTION_HEAD");
                    SetCell(wsPart, 3, 2, "مسئول بخش");
                    SetCell(wsPart, 3, 3, "Section Head");
                    SetCell(wsPart, 3, 4, true);

                    UpdateDimension(wsPart, 3, headers.Length);
                }
            }

            // =====================================================
            // Responsibility Types - AF:AG
            // =====================================================

            for (var i = 0; i < responsibilityTypes.Length; i++)
            {
                var row = i + 2;
                var responsibilityType = responsibilityTypes[i];

                SetCell(
                    lookupPart,
                    row,
                    32,
                    responsibilityType.ToString());

                SetCell(
                    lookupPart,
                    row,
                    33,
                    responsibilityType.ToString());
            }

            // =====================================================
            // Organization / Analyst Phrases
            // No current exported source is available.
            // =====================================================

            // OrganizationCode / OrganizationName -> empty
            // AnalystPhraseCode / AnalystPhrase -> empty

            var maxLookupRows =
                new[]
                {
                    sortedCustomers.Count,
                    sortedSampleCategories.Count,
                    sortedStandardSamples.Count,
                    sortedMatrices.Count,
                    sortedTests.Count,
                    sortedPanels.Count,
                    sortedEmployees.Count,
                    sortedDepartments.Count,
                    sortedInstruments.Count,
                    sortedLimitReferences.Count,
                    sortedTestMethods.Count,
                    sortedResultDefinitions.Count,
                    sortedRoles.Count,
                    responsibilityTypes.Length
                }
                .DefaultIfEmpty(0)
                .Max();

            UpdateDimension(
                lookupPart,
                maxLookupRows + 1,
                33);

            // =====================================================
            // Update tblLookup range
            // =====================================================

            var lookupTablePart =
                lookupPart.TableDefinitionParts
                    .FirstOrDefault();

            if (lookupTablePart != null)
            {
                var lookupTable =
                    lookupTablePart.Table
                    ?? throw new InvalidOperationException(
                        "Table tblLookup پیدا نشد.");

                var lookupLastRow =
                    Math.Max(
                        2,
                        maxLookupRows + 1);

                lookupTable.Reference =
                    $"A1:AG{lookupLastRow}";

                if (lookupTable.AutoFilter != null)
                {
                    lookupTable.AutoFilter.Reference =
                        $"A1:AG{lookupLastRow}";
                }

                lookupTable.Save();
            }

            // =====================================================
            // Tests
            // =====================================================

            var testsSheet =
                sheets.Elements<Sheet>()
                    .FirstOrDefault(x =>
                        x.Name?.Value == "Tests")
                ?? throw new InvalidOperationException(
                    "Sheet Tests پیدا نشد.");

            var testsPart =
                (WorksheetPart)workbookPart.GetPartById(
                    testsSheet.Id!.Value);

            ClearRows(
                testsPart,
                2,
                10000,
                1,
                11);

            var testHeaders = new[]
            {
            "Code",
            "Name",
            "EnglishName",
            "TestType",
            "Unit",
            "DefaultAnalystCode",
            "MethodCode",
            "SOPCode",
            "ReferenceStandard",
            "IsActive",
            "Description"
        };

            for (var i = 0; i < testHeaders.Length; i++)
            {
                SetCell(
                    testsPart,
                    1,
                    i + 1,
                    testHeaders[i]);
            }

            var testRow = 2;

            foreach (var test in tests.OrderBy(x => x.Code))
            {
                cancellationToken.ThrowIfCancellationRequested();

                SetCell(
                    testsPart,
                    testRow,
                    1,
                    test.Code);

                SetCell(
                    testsPart,
                    testRow,
                    2,
                    test.Name);

                SetCell(
                    testsPart,
                    testRow,
                    3,
                    test.EnglishName);

                SetCell(
                    testsPart,
                    testRow,
                    4,
                    test.IsQuantitative
                        ? "Quantitative"
                        : "Qualitative");

                SetCell(
                    testsPart,
                    testRow,
                    5,
                    test.Unit);

                SetCell(
                    testsPart,
                    testRow,
                    6,
                    test.DefaultAnalyst?.PersonnelCode);

                SetCell(
                    testsPart,
                    testRow,
                    7,
                    test.TestMethod?.Code);

                SetCell(
                    testsPart,
                    testRow,
                    8,
                    null);

                SetCell(
                    testsPart,
                    testRow,
                    9,
                    null);

                SetCell(
                    testsPart,
                    testRow,
                    10,
                    test.IsActive);

                SetCell(
                    testsPart,
                    testRow,
                    11,
                    test.Description);

                testRow++;
            }

            UpdateDimension(
                testsPart,
                testRow - 1,
                11);

            // =====================================================
            // TestPanels
            // =====================================================

            var panelsSheet =
                sheets.Elements<Sheet>()
                    .FirstOrDefault(x =>
                        x.Name?.Value == "TestPanels")
                ?? throw new InvalidOperationException(
                    "Sheet TestPanels پیدا نشد.");

            var panelsPart =
                (WorksheetPart)workbookPart.GetPartById(
                    panelsSheet.Id!.Value);

            ClearRows(
                panelsPart,
                2,
                10000,
                1,
                5);

            var panelHeaders = new[]
            {
            "Code",
            "Name",
            "EnglishName",
            "Description",
            "IsActive"
        };

            for (var i = 0; i < panelHeaders.Length; i++)
            {
                SetCell(
                    panelsPart,
                    1,
                    i + 1,
                    panelHeaders[i]);
            }

            var panelRow = 2;

            foreach (var panel in panels.OrderBy(x => x.Code))
            {
                cancellationToken.ThrowIfCancellationRequested();

                SetCell(
                    panelsPart,
                    panelRow,
                    1,
                    panel.Code);

                SetCell(
                    panelsPart,
                    panelRow,
                    2,
                    panel.Name);

                SetCell(
                    panelsPart,
                    panelRow,
                    3,
                    null);

                SetCell(
                    panelsPart,
                    panelRow,
                    4,
                    panel.Description);

                SetCell(
                    panelsPart,
                    panelRow,
                    5,
                    panel.IsActive);

                panelRow++;
            }

            UpdateDimension(
                panelsPart,
                panelRow - 1,
                5);

            // =====================================================
            // PanelItems
            // =====================================================

            var panelItemsSheet =
                sheets.Elements<Sheet>()
                    .FirstOrDefault(x =>
                        x.Name?.Value == "PanelItems")
                ?? throw new InvalidOperationException(
                    "Sheet PanelItems پیدا نشد.");

            var panelItemsPart =
                (WorksheetPart)workbookPart.GetPartById(
                    panelItemsSheet.Id!.Value);

            ClearRows(
                panelItemsPart,
                2,
                10000,
                1,
                6);

            var panelItemHeaders = new[]
            {
            "PanelCode",
            "TestCode",
            "DisplayOrder",
            "DefaultAnalystCode",
            "IsActive",
            "Description"
        };

            for (var i = 0; i < panelItemHeaders.Length; i++)
            {
                SetCell(
                    panelItemsPart,
                    1,
                    i + 1,
                    panelItemHeaders[i]);
            }

            var panelItemRow = 2;

            foreach (var item in panelItems
                .OrderBy(x => x.TestPanel?.Code)
                .ThenBy(x => x.SortOrder))
            {
                cancellationToken.ThrowIfCancellationRequested();

                SetCell(
                    panelItemsPart,
                    panelItemRow,
                    1,
                    item.TestPanel?.Code);

                SetCell(
                    panelItemsPart,
                    panelItemRow,
                    2,
                    item.Test?.Code);

                SetCell(
                    panelItemsPart,
                    panelItemRow,
                    3,
                    item.SortOrder);

                SetCell(
                    panelItemsPart,
                    panelItemRow,
                    4,
                    item.DefaultAnalyst?.PersonnelCode);

                SetCell(
                    panelItemsPart,
                    panelItemRow,
                    5,
                    item.IsActive);

                SetCell(
                    panelItemsPart,
                    panelItemRow,
                    6,
                    null);

                panelItemRow++;
            }

            UpdateDimension(
                panelItemsPart,
                panelItemRow - 1,
                6);

            // =====================================================
            // Customers
            // =====================================================

            var customersSheet =
                sheets.Elements<Sheet>()
                    .FirstOrDefault(x =>
                        x.Name?.Value == "Customers")
                ?? throw new InvalidOperationException(
                    "Sheet Customers پیدا نشد.");

            var customersPart =
                (WorksheetPart)workbookPart.GetPartById(
                    customersSheet.Id!.Value);

            ClearRows(
                customersPart,
                2,
                10000,
                1,
                12);

            var customerHeaders = new[]
            {
    "Code",
    "Name",
    "EnglishName",
    "NationalId",
    "EconomicCode",
    "Phone",
    "Mobile",
    "Email",
    "Address",
    "ContactPerson",
    "IsActive",
    "Description"
};

            for (var i = 0; i < customerHeaders.Length; i++)
            {
                SetCell(
                    customersPart,
                    1,
                    i + 1,
                    customerHeaders[i]);
            }

            var customerRow = 2;

            foreach (var customer in customers.OrderBy(x => x.Code))
            {
                cancellationToken.ThrowIfCancellationRequested();

                SetCell(customersPart, customerRow, 1, customer.Code);
                SetCell(customersPart, customerRow, 2, customer.DisplayName);
                SetCell(customersPart, customerRow, 3, null);
                SetCell(customersPart, customerRow, 4, customer.NationalId);
                SetCell(customersPart, customerRow, 5, customer.EconomicCode);
                SetCell(customersPart, customerRow, 6, customer.Phone);
                SetCell(customersPart, customerRow, 7, customer.Mobile);
                SetCell(customersPart, customerRow, 8, customer.Email);
                SetCell(customersPart, customerRow, 9, customer.Address);
                SetCell(customersPart, customerRow, 10, null);
                SetCell(customersPart, customerRow, 11, customer.IsActive);
                SetCell(customersPart, customerRow, 12, customer.Description);

                customerRow++;
            }

            UpdateDimension(
                customersPart,
                customerRow - 1,
                12);


            // =====================================================
            // SampleCategories
            // =====================================================

            var sampleCategoriesSheet =
                sheets.Elements<Sheet>()
                    .FirstOrDefault(x =>
                        x.Name?.Value == "SampleCategories")
                ?? throw new InvalidOperationException(
                    "Sheet SampleCategories پیدا نشد.");

            var sampleCategoriesPart =
                (WorksheetPart)workbookPart.GetPartById(
                    sampleCategoriesSheet.Id!.Value);

            ClearRows(
                sampleCategoriesPart,
                2,
                10000,
                1,
                5);

            var sampleCategoryHeaders = new[]
            {
    "Code",
    "Name",
    "EnglishName",
    "IsActive",
    "Description"
};

            for (var i = 0; i < sampleCategoryHeaders.Length; i++)
            {
                SetCell(
                    sampleCategoriesPart,
                    1,
                    i + 1,
                    sampleCategoryHeaders[i]);
            }

            var sampleCategoryRow = 2;

            foreach (var category in sampleCategories.OrderBy(x => x.Code))
            {
                cancellationToken.ThrowIfCancellationRequested();

                SetCell(
                    sampleCategoriesPart,
                    sampleCategoryRow,
                    1,
                    category.Code);

                SetCell(
                    sampleCategoriesPart,
                    sampleCategoryRow,
                    2,
                    category.Name);

                SetCell(
                    sampleCategoriesPart,
                    sampleCategoryRow,
                    3,
                    null);

                SetCell(
                    sampleCategoriesPart,
                    sampleCategoryRow,
                    4,
                    category.IsActive);

                SetCell(
                    sampleCategoriesPart,
                    sampleCategoryRow,
                    5,
                    category.Description);

                sampleCategoryRow++;
            }

            UpdateDimension(
                sampleCategoriesPart,
                sampleCategoryRow - 1,
                5);


            // =====================================================
            // Matrices
            // =====================================================

            var sampleCategoryNames =
                sampleCategories.ToDictionary(
                    x => x.Id,
                    x => x.Name);

            var matricesSheet =
                sheets.Elements<Sheet>()
                    .FirstOrDefault(x =>
                        x.Name?.Value == "Matrices")
                ?? throw new InvalidOperationException(
                    "Sheet Matrices پیدا نشد.");

            var matricesPart =
                (WorksheetPart)workbookPart.GetPartById(
                    matricesSheet.Id!.Value);

            ClearRows(
                matricesPart,
                2,
                10000,
                1,
                6);

            var matrixHeaders = new[]
            {
    "Code",
    "Name",
    "EnglishName",
    "SampleCategoryCode",
    "IsActive",
    "Description"
};

            for (var i = 0; i < matrixHeaders.Length; i++)
            {
                SetCell(
                    matricesPart,
                    1,
                    i + 1,
                    matrixHeaders[i]);
            }

            var matrixRow = 2;

            foreach (var matrix in matrices.OrderBy(x => x.Code))
            {
                cancellationToken.ThrowIfCancellationRequested();

                sampleCategoryNames.TryGetValue(
                    matrix.SampleCategoryId,
                    out var sampleCategoryName);

                var sampleCategoryCode =
                    sampleCategories
                        .FirstOrDefault(x =>
                            x.Id == matrix.SampleCategoryId)
                        ?.Code;

                SetCell(
                    matricesPart,
                    matrixRow,
                    1,
                    matrix.Code);

                SetCell(
                    matricesPart,
                    matrixRow,
                    2,
                    matrix.Name);

                SetCell(
                    matricesPart,
                    matrixRow,
                    3,
                    null);

                SetCell(
                    matricesPart,
                    matrixRow,
                    4,
                    sampleCategoryCode);

                SetCell(
                    matricesPart,
                    matrixRow,
                    5,
                    matrix.IsActive);

                SetCell(
                    matricesPart,
                    matrixRow,
                    6,
                    matrix.Description);

                matrixRow++;
            }

            UpdateDimension(
                matricesPart,
                matrixRow - 1,
                6);


            // =====================================================
            // StandardSamples
            // =====================================================

            var matrixNames =
                matrices.ToDictionary(
                    x => x.Id,
                    x => x.Name);

            var standardSamplesSheet =
                sheets.Elements<Sheet>()
                    .FirstOrDefault(x =>
                        x.Name?.Value == "StandardSamples")
                ?? throw new InvalidOperationException(
                    "Sheet StandardSamples پیدا نشد.");

            var standardSamplesPart =
                (WorksheetPart)workbookPart.GetPartById(
                    standardSamplesSheet.Id!.Value);

            ClearRows(
                standardSamplesPart,
                2,
                10000,
                1,
                7);

            var standardSampleHeaders = new[]
            {
    "Code",
    "Name",
    "EnglishName",
    "SampleCategoryCode",
    "MatrixCode",
    "IsActive",
    "Description"
};

            for (var i = 0; i < standardSampleHeaders.Length; i++)
            {
                SetCell(
                    standardSamplesPart,
                    1,
                    i + 1,
                    standardSampleHeaders[i]);
            }

            var standardSampleRow = 2;

            foreach (var standardSample in
                     standardSamples.OrderBy(x => x.Code))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var sampleCategoryCode =
                    sampleCategories
                        .FirstOrDefault(x =>
                            x.Id == standardSample.SampleCategoryId)
                        ?.Code;

                var matrixCode =
                    matrices
                        .FirstOrDefault(x =>
                            x.Id == standardSample.MatrixId)
                        ?.Code;

                SetCell(
                    standardSamplesPart,
                    standardSampleRow,
                    1,
                    standardSample.Code);

                SetCell(
                    standardSamplesPart,
                    standardSampleRow,
                    2,
                    standardSample.Name);

                SetCell(
                    standardSamplesPart,
                    standardSampleRow,
                    3,
                    null);

                SetCell(
                    standardSamplesPart,
                    standardSampleRow,
                    4,
                    sampleCategoryCode);

                SetCell(
                    standardSamplesPart,
                    standardSampleRow,
                    5,
                    matrixCode);

                SetCell(
                    standardSamplesPart,
                    standardSampleRow,
                    6,
                    standardSample.IsActive);

                SetCell(
                    standardSamplesPart,
                    standardSampleRow,
                    7,
                    standardSample.Description);

                standardSampleRow++;
            }

            UpdateDimension(
                standardSamplesPart,
                standardSampleRow - 1,
                7);

            // =====================================================
            // Employees
            // =====================================================

            var employeesSheet =
                sheets.Elements<Sheet>()
                    .FirstOrDefault(x =>
                        x.Name?.Value == "Employees")
                ?? throw new InvalidOperationException(
                    "Sheet Employees پیدا نشد.");

            var employeesPart =
                (WorksheetPart)workbookPart.GetPartById(
                    employeesSheet.Id!.Value);

            ClearRows(
                employeesPart,
                2,
                10000,
                1,
                12);

            var employeeHeaders = new[]
            {
    "Code",
    "FirstName",
    "LastName",
    "NationalId",
    "PersonnelCode",
    "DepartmentCode",
    "RoleCode",
    "Phone",
    "Mobile",
    "Email",
    "IsActive",
    "Description"
};

            for (var i = 0; i < employeeHeaders.Length; i++)
            {
                SetCell(
                    employeesPart,
                    1,
                    i + 1,
                    employeeHeaders[i]);
            }

            var employeeRow = 2;

            foreach (var employee in employees.OrderBy(x => x.PersonnelCode))
            {
                cancellationToken.ThrowIfCancellationRequested();

                SetCell(
                    employeesPart,
                    employeeRow,
                    1,
                    employee.PersonnelCode);

                SetCell(
                    employeesPart,
                    employeeRow,
                    2,
                    employee.FullName);

                SetCell(
                    employeesPart,
                    employeeRow,
                    3,
                    null);

                SetCell(
                    employeesPart,
                    employeeRow,
                    4,
                    employee.NationalCode);

                SetCell(
                    employeesPart,
                    employeeRow,
                    5,
                    employee.PersonnelCode);

                SetCell(
                    employeesPart,
                    employeeRow,
                    6,
                    null);

                SetCell(
                    employeesPart,
                    employeeRow,
                    7,
                    null);

                SetCell(
                    employeesPart,
                    employeeRow,
                    8,
                    null);

                SetCell(
                    employeesPart,
                    employeeRow,
                    9,
                    employee.Mobile);

                SetCell(
                    employeesPart,
                    employeeRow,
                    10,
                    employee.Email);

                SetCell(
                    employeesPart,
                    employeeRow,
                    11,
                    true);

                SetCell(
                    employeesPart,
                    employeeRow,
                    12,
                    null);

                employeeRow++;
            }

            UpdateDimension(
                employeesPart,
                employeeRow - 1,
                12);


            // =====================================================
            // Departments
            // =====================================================

            var departmentsSheet =
                sheets.Elements<Sheet>()
                    .FirstOrDefault(x =>
                        x.Name?.Value == "Departments")
                ?? throw new InvalidOperationException(
                    "Sheet Departments پیدا نشد.");

            var departmentsPart =
                (WorksheetPart)workbookPart.GetPartById(
                    departmentsSheet.Id!.Value);

            ClearRows(
                departmentsPart,
                2,
                10000,
                1,
                6);

            var departmentHeaders = new[]
            {
    "Code",
    "Name",
    "EnglishName",
    "ParentDepartmentCode",
    "IsActive",
    "Description"
};

            for (var i = 0; i < departmentHeaders.Length; i++)
            {
                SetCell(
                    departmentsPart,
                    1,
                    i + 1,
                    departmentHeaders[i]);
            }

            var departmentRow = 2;

            foreach (var department in departments.OrderBy(x => x.Code))
            {
                cancellationToken.ThrowIfCancellationRequested();

                SetCell(
                    departmentsPart,
                    departmentRow,
                    1,
                    department.Code);

                SetCell(
                    departmentsPart,
                    departmentRow,
                    2,
                    department.Name);

                SetCell(
                    departmentsPart,
                    departmentRow,
                    3,
                    null);

                SetCell(
                    departmentsPart,
                    departmentRow,
                    4,
                    null);

                SetCell(
                    departmentsPart,
                    departmentRow,
                    5,
                    true);

                SetCell(
                    departmentsPart,
                    departmentRow,
                    6,
                    department.Description);

                departmentRow++;
            }

            UpdateDimension(
                departmentsPart,
                departmentRow - 1,
                6);


            // =====================================================
            // Roles
            // =====================================================

            var rolesSheet =
                sheets.Elements<Sheet>()
                    .FirstOrDefault(x =>
                        x.Name?.Value == "Roles")
                ?? throw new InvalidOperationException(
                    "Sheet Roles پیدا نشد.");

            var rolesPart =
                (WorksheetPart)workbookPart.GetPartById(
                    rolesSheet.Id!.Value);

            ClearRows(
                rolesPart,
                2,
                10000,
                1,
                5);

            var roleHeaders = new[]
            {
    "Code",
    "Name",
    "EnglishName",
    "IsActive",
    "Description"
};

            for (var i = 0; i < roleHeaders.Length; i++)
            {
                SetCell(
                    rolesPart,
                    1,
                    i + 1,
                    roleHeaders[i]);
            }

            var roleRow = 2;

            foreach (var role in roles.OrderBy(x => x.Code))
            {
                cancellationToken.ThrowIfCancellationRequested();

                SetCell(
                    rolesPart,
                    roleRow,
                    1,
                    role.Code);

                SetCell(
                    rolesPart,
                    roleRow,
                    2,
                    role.Name);

                SetCell(
                    rolesPart,
                    roleRow,
                    3,
                    null);

                SetCell(
                    rolesPart,
                    roleRow,
                    4,
                    true);

                SetCell(
                    rolesPart,
                    roleRow,
                    5,
                    role.Description);

                roleRow++;
            }

            UpdateDimension(
                rolesPart,
                roleRow - 1,
                5);


            // =====================================================
            // Instruments
            // =====================================================

            var instrumentsSheet =
                sheets.Elements<Sheet>()
                    .FirstOrDefault(x =>
                        x.Name?.Value == "Instruments")
                ?? throw new InvalidOperationException(
                    "Sheet Instruments پیدا نشد.");

            var instrumentsPart =
                (WorksheetPart)workbookPart.GetPartById(
                    instrumentsSheet.Id!.Value);

            ClearRows(
    instrumentsPart,
    2,
    10000,
    1,
    18);

            var instrumentHeaders = new[]
            {
                "Code",
                "Name",
                "InstrumentType",
                "Manufacturer",
                "Model",
                "SerialNumber",
                "AssetNumber",
                "DepartmentCode",
                "Location",
                "PurchaseDate",
                "InstallationDate",
                "CalibrationRequired",
                "CalibrationIntervalMonths",
                "LastCalibrationDate",
                "NextCalibrationDate",
                "Status",
                "IsActive",
                "Description"
            };

            for (var i = 0; i < instrumentHeaders.Length; i++)
            {
                SetCell(
                    instrumentsPart,
                    1,
                    i + 1,
                    instrumentHeaders[i]);
            }

            var instrumentRow = 2;

            foreach (var instrument in instruments.OrderBy(x => x.Code))
            {
                cancellationToken.ThrowIfCancellationRequested();

                SetCell(
                    instrumentsPart,
                    instrumentRow,
                    1,
                    instrument.Code);

                SetCell(
                    instrumentsPart,
                    instrumentRow,
                    2,
                    instrument.Name);

                SetCell(
                    instrumentsPart,
                    instrumentRow,
                    3,
                    null);

                SetCell(
                    instrumentsPart,
                    instrumentRow,
                    4,
                    instrument.Manufacturer);

                SetCell(
                    instrumentsPart,
                    instrumentRow,
                    5,
                    instrument.Model);

                SetCell(
                    instrumentsPart,
                    instrumentRow,
                    6,
                    instrument.SerialNumber);

                SetCell(
                    instrumentsPart,
                    instrumentRow,
                    7,
                    null);

                SetCell(
                    instrumentsPart,
                    instrumentRow,
                    8,
                    null);

                SetCell(
                    instrumentsPart,
                    instrumentRow,
                    9,
                    null);

                SetCell(
                    instrumentsPart,
                    instrumentRow,
                    10,
                    null);

                SetCell(
                    instrumentsPart,
                    instrumentRow,
                    11,
                    null);

                SetCell(
                    instrumentsPart,
                    instrumentRow,
                    12,
                    null);

                SetCell(
                    instrumentsPart,
                    instrumentRow,
                    13,
                    null);

                SetCell(
                    instrumentsPart,
                    instrumentRow,
                    14,
                    null);

                SetCell(
                    instrumentsPart,
                    instrumentRow,
                    15,
                    null);

                SetCell(
                    instrumentsPart,
                    instrumentRow,
                    16,
                    null);

                SetCell(
                    instrumentsPart,
                    instrumentRow,
                    17,
                    instrument.IsActiveForTesting);

                SetCell(
                    instrumentsPart,
                    instrumentRow,
                    18,
                    instrument.Description);

                instrumentRow++;
            }

            UpdateDimension(
                instrumentsPart,
                instrumentRow - 1,
                18);


            // =====================================================
            // TestMethods
            // =====================================================

            var testMethodsSheet =
                sheets.Elements<Sheet>()
                    .FirstOrDefault(x =>
                        x.Name?.Value == "TestMethods")
                ?? throw new InvalidOperationException(
                    "Sheet TestMethods پیدا نشد.");

            var testMethodsPart =
                (WorksheetPart)workbookPart.GetPartById(
                    testMethodsSheet.Id!.Value);

            ClearRows(
                testMethodsPart,
                2,
                10000,
                1,
                5);

            var testMethodHeaders = new[]
            {
    "Code",
    "Name",
    "EnglishName",
    "IsActive",
    "Description"
};

            for (var i = 0; i < testMethodHeaders.Length; i++)
            {
                SetCell(
                    testMethodsPart,
                    1,
                    i + 1,
                    testMethodHeaders[i]);
            }

            var testMethodRow = 2;

            foreach (var method in testMethods.OrderBy(x => x.Code))
            {
                cancellationToken.ThrowIfCancellationRequested();

                SetCell(
                    testMethodsPart,
                    testMethodRow,
                    1,
                    method.Code);

                SetCell(
                    testMethodsPart,
                    testMethodRow,
                    2,
                    method.Name);

                SetCell(
                    testMethodsPart,
                    testMethodRow,
                    3,
                    null);

                SetCell(
                    testMethodsPart,
                    testMethodRow,
                    4,
                    true);

                SetCell(
                    testMethodsPart,
                    testMethodRow,
                    5,
                    method.Description);

                testMethodRow++;
            }

            UpdateDimension(
                testMethodsPart,
                testMethodRow - 1,
                5);

            // =====================================================
            // ResultDefinitions
            // =====================================================

            var resultDefinitionsSheet =
                sheets.Elements<Sheet>()
                    .FirstOrDefault(x =>
                        x.Name?.Value == "ResultDefinitions")
                ?? throw new InvalidOperationException(
                    "Sheet ResultDefinitions پیدا نشد.");

            var resultDefinitionsPart =
                (WorksheetPart)workbookPart.GetPartById(
                    resultDefinitionsSheet.Id!.Value);

            ClearRows(
                resultDefinitionsPart,
                2,
                10000,
                1,
                8);

            var resultDefinitionHeaders = new[]
            {
    "TestCode",
"Code",
"Name",
"DisplayOrder",
"Unit",
"IsRequired",
"DecimalPlaces",
"Description"
};

            for (var i = 0; i < resultDefinitionHeaders.Length; i++)
            {
                SetCell(
                    resultDefinitionsPart,
                    1,
                    i + 1,
                    resultDefinitionHeaders[i]);
            }

            var resultDefinitionRow = 2;

            foreach (var definition in
                     resultDefinitions
                         .OrderBy(x => x.Test?.Code)
                         .ThenBy(x => x.DisplayOrder))
            {
                cancellationToken.ThrowIfCancellationRequested();

                SetCell(
    resultDefinitionsPart,
    resultDefinitionRow,
    1,
    definition.Test?.Code);

                SetCell(
                    resultDefinitionsPart,
                    resultDefinitionRow,
                    2,
                    definition.Code);

                SetCell(
                    resultDefinitionsPart,
                    resultDefinitionRow,
                    3,
                    definition.Name);

                SetCell(
                    resultDefinitionsPart,
                    resultDefinitionRow,
                    4,
                    definition.DisplayOrder);

                SetCell(
                    resultDefinitionsPart,
                    resultDefinitionRow,
                    5,
                    definition.Unit);

                SetCell(
                    resultDefinitionsPart,
                    resultDefinitionRow,
                    6,
                    definition.IsRequired);

                SetCell(
                    resultDefinitionsPart,
                    resultDefinitionRow,
                    7,
                    definition.DecimalPlaces);

                SetCell(
                    resultDefinitionsPart,
                    resultDefinitionRow,
                    8,
                    definition.Description);

                resultDefinitionRow++;
            }

            UpdateDimension(
                resultDefinitionsPart,
                resultDefinitionRow - 1,
                8);

            // =====================================================
            // DefaultTestSets
            // =====================================================

            var defaultTestSetsSheet =
                sheets.Elements<Sheet>()
                    .FirstOrDefault(x =>
                        x.Name?.Value == "DefaultTestSets")
                ?? throw new InvalidOperationException(
                    "Sheet DefaultTestSets پیدا نشد.");

            var defaultTestSetsPart =
                (WorksheetPart)workbookPart.GetPartById(
                    defaultTestSetsSheet.Id!.Value);

            ClearRows(
                defaultTestSetsPart,
                2,
                10000,
                1,
                11);

            var defaultTestSetHeaders = new[]
            {
                "Code",
                "Name",
                "CustomerCode",
                "SampleCategoryCode",
                "MatrixCode",
                "StandardSampleCode",
                "TestCode",
                "PanelCode",
                "DisplayOrder",
                "IsActive",
                "Description"
            };

            for (var i = 0; i < defaultTestSetHeaders.Length; i++)
            {
                SetCell(
                    defaultTestSetsPart,
                    1,
                    i + 1,
                    defaultTestSetHeaders[i]);
            }

            var defaultTestSetRow = 2;

            var defaultTestSets =
                await _defaultTestSetService.GetAllAsync(
                    cancellationToken);

            var customerCodes =
                customers.ToDictionary(
                    x => x.Id,
                    x => x.Code);

            var sampleCategoryCodes =
                sampleCategories.ToDictionary(
                    x => x.Id,
                    x => x.Code);

            var matrixCodes =
                matrices.ToDictionary(
                    x => x.Id,
                    x => x.Code);

            var standardSampleCodes =
                standardSamples.ToDictionary(
                    x => x.Id,
                    x => x.Code);

            foreach (var defaultTestSet in
                     defaultTestSets
                         .OrderBy(x => x.Code))
            {
                cancellationToken.ThrowIfCancellationRequested();

                foreach (var item in
                         defaultTestSet.Items
                             .OrderBy(x => x.SortOrder))
                {
                    SetCell(
                        defaultTestSetsPart,
                        defaultTestSetRow,
                        1,
                        defaultTestSet.Code);

                    SetCell(
                        defaultTestSetsPart,
                        defaultTestSetRow,
                        2,
                        defaultTestSet.Name);

                    SetCell(
                        defaultTestSetsPart,
                        defaultTestSetRow,
                        3,
                        defaultTestSet.CustomerId.HasValue &&
                        customerCodes.TryGetValue(
                            defaultTestSet.CustomerId.Value,
                            out var customerCode)
                            ? customerCode
                            : null);

                    SetCell(
                        defaultTestSetsPart,
                        defaultTestSetRow,
                        4,
                        defaultTestSet.SampleCategoryId.HasValue &&
                        sampleCategoryCodes.TryGetValue(
                            defaultTestSet.SampleCategoryId.Value,
                            out var sampleCategoryCode)
                            ? sampleCategoryCode
                            : null);

                    SetCell(
                        defaultTestSetsPart,
                        defaultTestSetRow,
                        5,
                        defaultTestSet.MatrixId.HasValue &&
                        matrixCodes.TryGetValue(
                            defaultTestSet.MatrixId.Value,
                            out var matrixCode)
                            ? matrixCode
                            : null);

                    SetCell(
                        defaultTestSetsPart,
                        defaultTestSetRow,
                        6,
                        defaultTestSet.StandardSampleId.HasValue &&
                        standardSampleCodes.TryGetValue(
                            defaultTestSet.StandardSampleId.Value,
                            out var standardSampleCode)
                            ? standardSampleCode
                            : null);

                    SetCell(
                        defaultTestSetsPart,
                        defaultTestSetRow,
                        7,
                        item.IsPanel
                            ? null
                            : item.TestCode);

                    SetCell(
                        defaultTestSetsPart,
                        defaultTestSetRow,
                        8,
                        item.IsPanel
                            ? item.TestPanelCode
                            : null);

                    SetCell(
                        defaultTestSetsPart,
                        defaultTestSetRow,
                        9,
                        item.SortOrder);

                    SetCell(
                        defaultTestSetsPart,
                        defaultTestSetRow,
                        10,
                        true);

                    SetCell(
                        defaultTestSetsPart,
                        defaultTestSetRow,
                        11,
                        defaultTestSet.Description);

                    defaultTestSetRow++;
                }
            }

            UpdateDimension(
                defaultTestSetsPart,
                defaultTestSetRow - 1,
                11);



            // =====================================================
            // LimitReferences
            // =====================================================

            var limitReferencesSheet =
                sheets.Elements<Sheet>()
                    .FirstOrDefault(x =>
                        x.Name?.Value == "LimitReferences")
                ?? throw new InvalidOperationException(
                    "Sheet LimitReferences پیدا نشد.");

            var limitReferencesPart =
                (WorksheetPart)workbookPart.GetPartById(
                    limitReferencesSheet.Id!.Value);

            ClearRows(
                limitReferencesPart,
                2,
                10000,
                1,
                10);

            var limitReferenceHeaders = new[]
            {
                "Code",
                "Name",
                "Version",
                "IssueDate",
                "ValidFrom",
                "ValidTo",
                "IsActive",
                "DocumentNo",
                "Priority",
                "Description"
            };

            for (var i = 0; i < limitReferenceHeaders.Length; i++)
            {
                SetCell(
                    limitReferencesPart,
                    1,
                    i + 1,
                    limitReferenceHeaders[i]);
            }

            var limitReferenceRow = 2;

            foreach (var reference in
                     limitReferences.OrderBy(x => x.Code))
            {
                cancellationToken.ThrowIfCancellationRequested();

                SetCell(
                    limitReferencesPart,
                    limitReferenceRow,
                    1,
                    reference.Code);

                SetCell(
                    limitReferencesPart,
                    limitReferenceRow,
                    2,
                    reference.Name);

                SetCell(
                    limitReferencesPart,
                    limitReferenceRow,
                    3,
                    reference.Version);

                SetCell(
                    limitReferencesPart,
                    limitReferenceRow,
                    4,
                    reference.IssueDate);

                SetCell(
                    limitReferencesPart,
                    limitReferenceRow,
                    5,
                    reference.ValidFrom);

                SetCell(
                    limitReferencesPart,
                    limitReferenceRow,
                    6,
                    reference.ValidTo);

                SetCell(
                    limitReferencesPart,
                    limitReferenceRow,
                    7,
                    reference.IsActive);

                SetCell(
                    limitReferencesPart,
                    limitReferenceRow,
                    8,
                    reference.DocumentNo);

                SetCell(
                    limitReferencesPart,
                    limitReferenceRow,
                    9,
                    reference.Priority);

                SetCell(
                    limitReferencesPart,
                    limitReferenceRow,
                    10,
                    reference.Description);

                limitReferenceRow++;
            }

            UpdateDimension(
                limitReferencesPart,
                limitReferenceRow - 1,
                10);


            // =====================================================
            // TestLimitRules
            // =====================================================

            var testLimitRulesSheet =
                sheets.Elements<Sheet>()
                    .FirstOrDefault(x =>
                        x.Name?.Value == "TestLimitRules")
                ?? throw new InvalidOperationException(
                    "Sheet TestLimitRules پیدا نشد.");

            var testLimitRulesPart =
                (WorksheetPart)workbookPart.GetPartById(
                    testLimitRulesSheet.Id!.Value);

            ClearRows(
                testLimitRulesPart,
                2,
                10000,
                1,
                15);

            var testLimitRuleHeaders = new[]
            {
                "TestCode",
                "LimitReferenceCode",
                "LimitType",
                "LowerValue",
                "UpperValue",
                "ExactValue",
                "LowerInclusive",
                "UpperInclusive",
                "AllowedValues",
                "Unit",
                "Priority",
                "ValidFrom",
                "ValidTo",
                "IsActive",
                "Description"
            };

            for (var i = 0; i < testLimitRuleHeaders.Length; i++)
            {
                SetCell(
                    testLimitRulesPart,
                    1,
                    i + 1,
                    testLimitRuleHeaders[i]);
            }

            var testLimitRuleRow = 2;

            foreach (var rule in
                     testLimitRules
                         .OrderBy(x => x.Test?.Code)
                         .ThenBy(x => x.LimitReference?.Code)
                         .ThenBy(x => x.Priority))
            {
                cancellationToken.ThrowIfCancellationRequested();

                SetCell(
                    testLimitRulesPart,
                    testLimitRuleRow,
                    1,
                    rule.Test?.Code);

                SetCell(
                    testLimitRulesPart,
                    testLimitRuleRow,
                    2,
                    rule.LimitReference?.Code);

                SetCell(
                    testLimitRulesPart,
                    testLimitRuleRow,
                    3,
                    rule.LimitType.ToString());

                SetCell(
                    testLimitRulesPart,
                    testLimitRuleRow,
                    4,
                    rule.LowerValue);

                SetCell(
                    testLimitRulesPart,
                    testLimitRuleRow,
                    5,
                    rule.UpperValue);

                SetCell(
                    testLimitRulesPart,
                    testLimitRuleRow,
                    6,
                    rule.ExactValue);

                SetCell(
                    testLimitRulesPart,
                    testLimitRuleRow,
                    7,
                    rule.LowerInclusive);

                SetCell(
                    testLimitRulesPart,
                    testLimitRuleRow,
                    8,
                    rule.UpperInclusive);

                SetCell(
                    testLimitRulesPart,
                    testLimitRuleRow,
                    9,
                    rule.AllowedValues);

                SetCell(
                    testLimitRulesPart,
                    testLimitRuleRow,
                    10,
                    rule.Unit);

                SetCell(
                    testLimitRulesPart,
                    testLimitRuleRow,
                    11,
                    rule.Priority);

                SetCell(
                    testLimitRulesPart,
                    testLimitRuleRow,
                    12,
                    rule.ValidFrom);

                SetCell(
                    testLimitRulesPart,
                    testLimitRuleRow,
                    13,
                    rule.ValidTo);

                SetCell(
                    testLimitRulesPart,
                    testLimitRuleRow,
                    14,
                    rule.IsActive);

                SetCell(
                    testLimitRulesPart,
                    testLimitRuleRow,
                    15,
                    rule.Description);

                testLimitRuleRow++;
            }

            UpdateDimension(
                testLimitRulesPart,
                testLimitRuleRow - 1,
                15);



        }

        return memoryStream.ToArray();
    }


    // =========================================================
    // Helpers
    // =========================================================

    private static void ClearRows(
        WorksheetPart worksheetPart,
        uint startRow,
        uint endRow,
        int startColumn,
        int endColumn)
    {
        var sheetData =
            worksheetPart.Worksheet
                .GetFirstChild<SheetData>();

        if (sheetData == null)
            return;

        var rows =
            sheetData.Elements<Row>()
                .Where(r =>
                    r.RowIndex != null &&
                    r.RowIndex.Value >= startRow &&
                    r.RowIndex.Value <= endRow)
                .ToList();

        foreach (var row in rows)
        {
            var cells =
                row.Elements<Cell>()
                    .Where(cell =>
                    {
                        var reference =
                            cell.CellReference?.Value;

                        if (string.IsNullOrWhiteSpace(reference))
                            return false;

                        var column =
                            GetColumnNumber(reference);

                        return column >= startColumn &&
                               column <= endColumn;
                    })
                    .ToList();

            foreach (var cell in cells)
            {
                cell.Remove();
            }

            // Remove empty rows.
            // This does not affect ActiveX/VBA because those
            // are stored in separate worksheet parts.
            if (!row.Elements<Cell>().Any())
            {
                row.Remove();
            }
        }
    }

    private static void SetCell(
    WorksheetPart worksheetPart,
    int rowNumber,
    int columnNumber,
    object? value)
    {
        var sheetData =
            worksheetPart.Worksheet
                .GetFirstChild<SheetData>();

        if (sheetData == null)
        {
            sheetData = new SheetData();

            worksheetPart.Worksheet
                .Append(sheetData);
        }

        var row =
            sheetData.Elements<Row>()
                .FirstOrDefault(r =>
                    r.RowIndex?.Value == (uint)rowNumber);

        if (row == null)
        {
            row =
                new Row
                {
                    RowIndex = (uint)rowNumber
                };

            var nextRow =
                sheetData.Elements<Row>()
                    .FirstOrDefault(r =>
                        r.RowIndex?.Value > (uint)rowNumber);

            if (nextRow != null)
                sheetData.InsertBefore(row, nextRow);
            else
                sheetData.Append(row);
        }

        var cellReference =
            $"{GetColumnName(columnNumber)}{rowNumber}";

        var cell =
            row.Elements<Cell>()
                .FirstOrDefault(c =>
                    c.CellReference?.Value ==
                    cellReference);

        if (cell == null)
        {
            cell =
                new Cell
                {
                    CellReference = cellReference
                };

            var nextCell =
                row.Elements<Cell>()
                    .FirstOrDefault(c =>
                        c.CellReference != null &&
                        GetColumnNumber(
                            c.CellReference.Value!) >
                        columnNumber);

            if (nextCell != null)
                row.InsertBefore(cell, nextCell);
            else
                row.Append(cell);
        }

        // Clear previous content
        cell.CellValue = null;
        cell.InlineString = null;
        cell.DataType = null;

        if (value == null)
        {
            cell.DataType =
                CellValues.InlineString;

            cell.InlineString =
                new InlineString(
                    new Text(""));

            return;
        }

        // Boolean
        if (value is bool boolValue)
        {
            cell.DataType =
                CellValues.Boolean;

            cell.CellValue =
                new CellValue(
                    boolValue ? "1" : "0");

            return;
        }

        // Numeric values
        if (IsNumeric(value))
        {
            cell.DataType =
                CellValues.Number;

            cell.CellValue =
                new CellValue(
                    Convert.ToString(
                        value,
                        CultureInfo.InvariantCulture)
                    ?? "");

            return;
        }

        // Everything else -> text
        cell.DataType =
            CellValues.InlineString;

        cell.InlineString =
            new InlineString(
                new Text(
                    Convert.ToString(
                        value,
                        CultureInfo.InvariantCulture)
                    ?? ""));
    }
    private static bool IsNumeric(object value)
    {
        return value is
            byte or
            sbyte or
            short or
            ushort or
            int or
            uint or
            long or
            ulong or
            float or
            double or
            decimal;
    }

    private static int GetColumnNumber(
        string cellReference)
    {
        var result = 0;

        foreach (var character in cellReference)
        {
            if (!char.IsLetter(character))
                break;

            result =
                result * 26 +
                (char.ToUpperInvariant(character) - 'A' + 1);
        }

        return result;
    }


    private static string GetColumnName(
        int columnNumber)
    {
        var result =
            string.Empty;

        while (columnNumber > 0)
        {
            columnNumber--;

            result =
                (char)('A' + (columnNumber % 26))
                + result;

            columnNumber /= 26;
        }

        return result;
    }


    private static void UpdateDimension(
        WorksheetPart worksheetPart,
        int lastRow,
        int lastColumn)
    {
        var worksheet =
            worksheetPart.Worksheet;

        var dimension =
            worksheet.GetFirstChild<SheetDimension>();

        var lastColumnName =
            GetColumnName(lastColumn);

        var reference =
            $"A1:{lastColumnName}{Math.Max(lastRow, 1)}";

        if (dimension == null)
        {
            dimension =
                new SheetDimension
                {
                    Reference = reference
                };

            worksheet.InsertAt(
                dimension,
                0);
        }
        else
        {
            dimension.Reference =
                reference;
        }
    }


    private static string FormatPersianDate(
        DateTime date)
    {
        var calendar =
            new System.Globalization.PersianCalendar();

        var year =
            calendar.GetYear(date);

        var month =
            calendar.GetMonth(date);

        var day =
            calendar.GetDayOfMonth(date);

        return
            $"{year:0000}/{month:00}/{day:00}";
    }
}