namespace Barman.Application.DTOs.TestTariff;

public class TariffBulkAdjustmentDto
{
    public Guid OrganizationTypeId { get; set; }

    /// <summary>
    /// null یعنی تعرفه عمومی سازمان.
    /// مقداردهی یعنی تعرفه اختصاصی یک مشتری.
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// تاریخ شروع تعرفه جدید.
    /// </summary>
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// درصد افزایش یا کاهش.
    /// مثال: 41 یعنی 41 درصد افزایش
    /// مثال: -15 یعنی 15 درصد کاهش
    /// </summary>
    public decimal Percentage { get; set; }

    /// <summary>
    /// اگر true باشد تمام Test و Panelهای دارای تعرفه مشمول می‌شوند.
    /// </summary>
    public bool ApplyToAll { get; set; }

    /// <summary>
    /// آزمون‌های انتخاب‌شده برای تعدیل.
    /// </summary>
    public List<Guid> TestIds { get; set; } = new();

    /// <summary>
    /// پنل‌های انتخاب‌شده برای تعدیل.
    /// </summary>
    public List<Guid> TestPanelIds { get; set; } = new();

    public List<Guid> ExpectedTariffIds { get; set; } = new();

    public Dictionary<Guid, decimal> ExpectedOldPrices { get; set; } = new();
}