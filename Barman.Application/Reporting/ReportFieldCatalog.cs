namespace Barman.Application.Reporting;

public sealed record ReportFieldDefinition(
    string Code,
    string Caption,
    string Group,
    string DataType,
    string? Description);

public static class ReportFieldCatalog
{
    public static IReadOnlyList<ReportFieldDefinition> All { get; } =
        new List<ReportFieldDefinition>
        {
            // =========================
            // Report
            // =========================
            new("ReportNumber", "شماره گزارش", "گزارش", "Text", null),
            new("ReportVersion", "نسخه گزارش", "گزارش", "Text", null),
            new("IssueDate", "تاریخ صدور", "گزارش", "Date", null),
            new("TemplateCode", "کد قالب", "گزارش", "Text", null),
            new("TemplateVersion", "نسخه قالب", "گزارش", "Text", null),

            // =========================
            // Laboratory
            // =========================
            new("LabName", "نام آزمایشگاه", "آزمایشگاه", "Text", null),
            new("LabLogo", "لوگوی آزمایشگاه", "آزمایشگاه", "Image", null),
            new("LabAddress", "آدرس آزمایشگاه", "آزمایشگاه", "Text", null),
            new("LabPhone", "تلفن آزمایشگاه", "آزمایشگاه", "Text", null),
            new("LabEmail", "ایمیل آزمایشگاه", "آزمایشگاه", "Text", null),
            new("AccreditationNo", "شماره اعتبار/استاندارد", "آزمایشگاه", "Text", null),

            // =========================
            // Reception
            // =========================
            new("ReceptionNumber", "شماره پذیرش", "پذیرش", "Text", null),
            new("ReceptionDate", "تاریخ پذیرش", "پذیرش", "DateTime", null),
            new("ReceptionStatus", "وضعیت پذیرش", "پذیرش", "Text", null),
            new("IsUrgent", "فوری", "پذیرش", "Boolean", null),
            new("ReceptionDescription", "توضیحات پذیرش", "پذیرش", "Text", null),
            new("TotalPrice", "مبلغ کل", "پذیرش", "Decimal", null),
            new("IsPaid", "وضعیت پرداخت", "پذیرش", "Boolean", null),

            // =========================
            // Customer
            // =========================
            new("CustomerCode", "کد مشتری", "مشتری", "Text", null),
            new("CustomerName", "نام مشتری", "مشتری", "Text", null),
            new("CustomerNationalId", "شناسه ملی مشتری", "مشتری", "Text", null),
            new("CustomerEconomicCode", "کد اقتصادی مشتری", "مشتری", "Text", null),
            new("CustomerPhone", "تلفن مشتری", "مشتری", "Text", null),
            new("CustomerMobile", "موبایل مشتری", "مشتری", "Text", null),
            new("CustomerEmail", "ایمیل مشتری", "مشتری", "Text", null),
            new("CustomerAddress", "آدرس مشتری", "مشتری", "Text", null),

            // =========================
            // Sample
            // =========================
            new("SampleCode", "کد نمونه", "نمونه", "Text", null),
            new("SampleName", "نام نمونه", "نمونه", "Text", null),
            new("CustomerSampleName", "نام نمونه مشتری", "نمونه", "Text", null),
            new("SampleCategory", "گروه نمونه", "نمونه", "Text", null),
            new("Matrix", "ماتریس", "نمونه", "Text", null),
            new("StandardSample", "نمونه استاندارد", "نمونه", "Text", null),
            new("BatchLotNumber", "شماره بچ/لات", "نمونه", "Text", null),
            new("QuotaNumber", "شماره کوتا", "نمونه", "Text", null),
            new("ShipmentNumber", "شماره محموله", "نمونه", "Text", null),
            new("ProductionDate", "تاریخ تولید", "نمونه", "Date", null),
            new("ExpiryDate", "تاریخ انقضا", "نمونه", "Date", null),
            new("Quantity", "مقدار نمونه", "نمونه", "Decimal", null),
            new("SampleUnit", "واحد نمونه", "نمونه", "Text", null),
            new("ContainerType", "نوع بسته‌بندی/ظرف", "نمونه", "Text", null),
            new("SampleDescription", "توضیحات نمونه", "نمونه", "Text", null),

            // =========================
            // Test
            // =========================
            new("TestCode", "کد آزمون", "آزمون", "Text", null),
            new("TestName", "نام آزمون", "آزمون", "Text", null),
            new("TestEnglishName", "نام انگلیسی آزمون", "آزمون", "Text", null),
            new("TestUnit", "واحد آزمون", "آزمون", "Text", null),
            new("TestMethod", "روش آزمون", "آزمون", "Text", null),
            new("TestInstrument", "دستگاه آزمون", "آزمون", "Text", null),

            // =========================
            // Result
            // =========================
            new("ResultDefinitionCode", "کد تعریف نتیجه", "نتیجه", "Text", null),
            new("ResultDefinitionName", "عنوان نتیجه", "نتیجه", "Text", null),
            new("Result", "نتیجه", "نتیجه", "Text", null),
            new("Unit", "واحد", "نتیجه", "Text", null),
            new("LOD", "LOD", "نتیجه", "Decimal", null),
            new("LOQ", "LOQ", "نتیجه", "Decimal", null),
            new("Min", "حداقل مجاز", "نتیجه", "Decimal", null),
            new("Max", "حداکثر مجاز", "نتیجه", "Decimal", null),
            new("ComplianceStatus", "وضعیت انطباق", "نتیجه", "Text", null),
            new("ResultComment", "توضیحات نتیجه", "نتیجه", "Text", null),

            // =========================
            // Limit
            // =========================
            new("PermissibleLimit", "حد مجاز", "حدود مجاز", "Text", null),
            new("LimitReference", "مرجع حد مجاز", "حدود مجاز", "Text", null),
            new("LowerLimit", "حد پایین", "حدود مجاز", "Decimal", null),
            new("UpperLimit", "حد بالا", "حدود مجاز", "Decimal", null),

            // =========================
            // Technical information
            // =========================
            new("Method", "روش آزمون", "اطلاعات فنی", "Text", null),
            new("Instrument", "دستگاه", "اطلاعات فنی", "Text", null),
            new("SOP", "SOP", "اطلاعات فنی", "Text", null),
            new("ReferenceStandard", "استاندارد مرجع", "اطلاعات فنی", "Text", null),
            new("AnalyticalProcedure", "روش اجرایی/روش آنالیز", "اطلاعات فنی", "Text", null),
            new("TechnicalNotes", "یادداشت فنی", "اطلاعات فنی", "Text", null),

            // =========================
            // Signature
            // =========================
            new("SectionHeadName", "نام مسئول بخش", "امضاها", "Text", null),
            new("SectionHeadSignature", "امضای مسئول بخش", "امضاها", "Signature", null),
            new("TechnicalManagerName", "نام مسئول فنی", "امضاها", "Text", null),
            new("TechnicalManagerSignature", "امضای مسئول فنی", "امضاها", "Signature", null),
            new("DirectorName", "نام مدیر آزمایشگاه", "امضاها", "Text", null),
            new("DirectorSignature", "امضای مدیر آزمایشگاه", "امضاها", "Signature", null),

            // =========================
            // Footer
            // =========================
            new("PageNumber", "شماره صفحه", "پاورقی", "Number", null),
            new("TotalPages", "تعداد صفحات", "پاورقی", "Number", null),
            new("GeneratedAt", "زمان تولید گزارش", "پاورقی", "DateTime", null),

            // =========================
            // Custom fields
            // =========================
            // Custom fields are resolved dynamically.
        };

    public static ReportFieldDefinition? Find(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        return All.FirstOrDefault(
            x => string.Equals(
                x.Code,
                code,
                StringComparison.OrdinalIgnoreCase));
    }
}