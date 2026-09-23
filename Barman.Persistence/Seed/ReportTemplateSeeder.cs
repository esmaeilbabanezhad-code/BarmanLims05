using Barman.Domain.Entities.Reporting;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Seed;

public static class ReportTemplateSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context)
    {
        const string defaultCode = "RPT-DEFAULT";

        var exists = await context.ReportTemplates
            .AnyAsync(x => x.Code == defaultCode);

        if (exists)
            return;

        var template = new ReportTemplate
        {
            Code = defaultCode,
            Name = "قالب استاندارد گزارش آزمایشگاه",
            ReportType = "Standard",
            Authority = "Laboratory",
            Version = "1.0",
            IsActive = true,
            IsSystemTemplate = true,
            IsSystemDefault = true,
            CanDelete = false,
            Description = "قالب پیش‌فرض گزارش نهایی آزمایشگاه"
        };

        AddSection(
            template,
            "HEADER",
            "سربرگ گزارش",
            "Standard",
            "Header",
            "Grid",
            1,
            new[]
            {
                ("Laboratory", "LabLogo", "لوگوی آزمایشگاه", "Image", 1),
                ("Laboratory", "LabName", "نام آزمایشگاه", "Value", 2),
                ("Laboratory", "AccreditationNo", "شماره اعتبار/استاندارد", "Value", 3)
            });

        AddSection(
            template,
            "REPORT_INFO",
            "اطلاعات گزارش",
            "Standard",
            "Body",
            "Grid",
            2,
            new[]
            {
                ("Report", "ReportNumber", "شماره گزارش", "Value", 1),
                ("Report", "IssueDate", "تاریخ صدور", "Value", 2),
                ("Report", "ReportVersion", "نسخه گزارش", "Value", 3)
            });

        AddSection(
            template,
            "CUSTOMER",
            "اطلاعات مشتری",
            "Standard",
            "Body",
            "Grid",
            3,
            new[]
            {
                ("Customer", "CustomerCode", "کد مشتری", "Value", 1),
                ("Customer", "CustomerName", "نام مشتری", "Value", 2),
                ("Customer", "CustomerNationalId", "شناسه ملی", "Value", 3),
                ("Customer", "CustomerAddress", "آدرس مشتری", "Value", 4)
            });

        AddSection(
            template,
            "SAMPLE",
            "اطلاعات نمونه",
            "Standard",
            "Body",
            "Grid",
            4,
            new[]
            {
                ("Sample", "SampleCode", "کد نمونه", "Value", 1),
                ("Sample", "SampleName", "نام نمونه", "Value", 2),
                ("Sample", "SampleCategory", "گروه نمونه", "Value", 3),
                ("Sample", "Matrix", "ماتریس", "Value", 4),
                ("Sample", "BatchLotNumber", "شماره بچ/لات", "Value", 5),
                ("Sample", "ProductionDate", "تاریخ تولید", "Value", 6),
                ("Sample", "ExpiryDate", "تاریخ انقضا", "Value", 7)
            });

        AddSection(
            template,
            "RESULTS",
            "نتایج آزمون",
            "Results",
            "Body",
            "Table",
            5,
            new[]
            {
                ("Test", "TestName", "آزمون", "TableColumn", 1),
                ("Result", "ResultDefinitionName", "عنوان نتیجه", "TableColumn", 2),
                ("Result", "Result", "نتیجه", "TableColumn", 3),
                ("Result", "Unit", "واحد", "TableColumn", 4),
                ("Result", "LOD", "LOD", "TableColumn", 5),
                ("Result", "LOQ", "LOQ", "TableColumn", 6),
                ("Limit", "PermissibleLimit", "حد مجاز", "TableColumn", 7),
                ("Result", "ComplianceStatus", "وضعیت انطباق", "TableColumn", 8)
            });

        AddSection(
            template,
            "TECHNICAL",
            "اطلاعات فنی",
            "Standard",
            "Body",
            "Grid",
            6,
            new[]
            {
                ("Test", "TestMethod", "روش آزمون", "Value", 1),
                ("Test", "TestInstrument", "دستگاه", "Value", 2),
                ("Technical", "SOP", "SOP", "Value", 3),
                ("Technical", "ReferenceStandard", "استاندارد مرجع", "Value", 4)
            });

        AddSection(
            template,
            "NOTES",
            "توضیحات",
            "Text",
            "Body",
            "Stack",
            7,
            new[]
            {
                ("Reception", "ReceptionDescription", "توضیحات پذیرش", "Text", 1),
                ("Sample", "SampleDescription", "توضیحات نمونه", "Text", 2),
                ("Result", "ResultComment", "توضیحات نتیجه", "Text", 3)
            });

        AddSection(
            template,
            "SIGNATURES",
            "امضاها",
            "Signature",
            "Body",
            "Grid",
            8,
            new[]
            {
                ("Signature", "SectionHeadName", "مسئول بخش", "Value", 1),
                ("Signature", "TechnicalManagerName", "مسئول فنی", "Value", 2),
                ("Signature", "DirectorName", "مدیر آزمایشگاه", "Value", 3),
                ("Signature", "SectionHeadSignature", "امضای مسئول بخش", "Signature", 4),
                ("Signature", "TechnicalManagerSignature", "امضای مسئول فنی", "Signature", 5),
                ("Signature", "DirectorSignature", "امضای مدیر آزمایشگاه", "Signature", 6)
            });

        AddSection(
            template,
            "FOOTER",
            "پاورقی گزارش",
            "Standard",
            "Footer",
            "Grid",
            9,
            new[]
            {
                ("Laboratory", "LabAddress", "آدرس آزمایشگاه", "Value", 1),
                ("Laboratory", "LabPhone", "تلفن آزمایشگاه", "Value", 2),
                ("Laboratory", "LabEmail", "ایمیل آزمایشگاه", "Value", 3),
                ("Footer", "PageNumber", "شماره صفحه", "PageNumber", 4),
                ("Footer", "TotalPages", "تعداد صفحات", "Value", 5)
            });

        await context.ReportTemplates.AddAsync(template);

        await context.SaveChangesAsync();
    }

    private static void AddSection(
        ReportTemplate template,
        string code,
        string name,
        string sectionType,
        string position,
        string layout,
        int displayOrder,
        IEnumerable<(string Source, string FieldCode, string Caption, string FieldType, int Order)> fields)
    {
        var section = new ReportTemplateSection
        {
            Code = code,
            Name = name,
            SectionType = sectionType,
            Position = position,
            Layout = layout,
            DisplayOrder = displayOrder,
            IsVisible = true
        };

        foreach (var field in fields)
        {
            section.Fields.Add(
                new ReportTemplateField
                {
                    Source = field.Source,
                    FieldCode = field.FieldCode,
                    FieldType = field.FieldType,
                    Caption = field.Caption,
                    DisplayOrder = field.Order,
                    IsVisible = true,
                    Alignment = "Right"
                });
        }

        template.Sections.Add(section);
    }
}
