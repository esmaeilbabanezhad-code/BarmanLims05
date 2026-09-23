using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class TestTariffService : ITestTariffService
{
    private readonly IUnitOfWork _unitOfWork;

    public TestTariffService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TestTariff>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.TestTariffs.GetAllAsync();
    }

    public async Task<TestTariff?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        return await _unitOfWork.TestTariffs.GetByIdAsync(id);
    }

    public async Task<List<TestTariff>> GetByTestIdAsync(
        Guid testId,
        CancellationToken cancellationToken = default)
    {
        if (testId == Guid.Empty)
            return new List<TestTariff>();

        return await _unitOfWork.TestTariffs.GetByTestIdAsync(testId);
    }

    public async Task<List<TestTariff>> GetByTestPanelIdAsync(
        Guid testPanelId,
        CancellationToken cancellationToken = default)
    {
        if (testPanelId == Guid.Empty)
            return new List<TestTariff>();

        return await _unitOfWork.TestTariffs.GetByTestPanelIdAsync(testPanelId);
    }

    public async Task<List<TestTariff>> GetByOrganizationTypeIdAsync(
        Guid organizationTypeId,
        CancellationToken cancellationToken = default)
    {
        if (organizationTypeId == Guid.Empty)
            return new List<TestTariff>();

        return await _unitOfWork.TestTariffs
            .GetByOrganizationTypeIdAsync(organizationTypeId);
    }

    public async Task<TestTariff?> GetApplicableAsync(
    Guid organizationTypeId,
    Guid? customerId,
    Guid? standardSampleId,
    Guid? matrixId,
    Guid? testId,
    Guid? testPanelId,
    DateTime effectiveDate,
    CancellationToken cancellationToken = default)
    {
        if (organizationTypeId == Guid.Empty)
            return null;

        if (!testId.HasValue && !testPanelId.HasValue)
            return null;

        if (testId.HasValue && testPanelId.HasValue)
            throw new ArgumentException(
                "تعرفه باید برای آزمون یا پنل مشخص شود، نه هر دو.");

        return await _unitOfWork.TestTariffs.GetApplicableAsync(
            organizationTypeId,
            customerId,
            standardSampleId,
            matrixId,
            testId,
            testPanelId,
            effectiveDate);
    }
    public async Task<List<TestTariff>> GetForBulkAdjustmentAsync(
    Guid organizationTypeId,
    Guid? customerId,
    DateTime effectiveDate,
    List<Guid>? testIds,
    List<Guid>? testPanelIds,
    bool applyToAll)
    {
        return await _unitOfWork.TestTariffs.GetForBulkAdjustmentAsync(
            organizationTypeId,
            customerId,
            effectiveDate,
            testIds,
            testPanelIds,
            applyToAll);
    }
    public async Task<List<Barman.Application.DTOs.TestTariff.TariffBulkAdjustmentPreviewDto>>
    PreviewBulkAdjustmentAsync(
        Barman.Application.DTOs.TestTariff.TariffBulkAdjustmentDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (dto.OrganizationTypeId == Guid.Empty)
            throw new ArgumentException(
                "نوع سازمان الزامی است.");

        if (dto.EffectiveDate == default)
            throw new ArgumentException(
                "تاریخ شروع تعرفه جدید الزامی است.");

        if (dto.Percentage == 0)
            throw new ArgumentException(
                "درصد تعدیل نمی‌تواند صفر باشد.");

        if (!dto.ApplyToAll &&
            (dto.TestIds == null || dto.TestIds.Count == 0) &&
            (dto.TestPanelIds == null || dto.TestPanelIds.Count == 0))
        {
            throw new ArgumentException(
                "حداقل یک آزمون یا پنل باید انتخاب شود.");
        }

        var effectiveDate =
            DateTime.SpecifyKind(
                dto.EffectiveDate.Date,
                DateTimeKind.Utc);

        var tariffs =
            await _unitOfWork.TestTariffs.GetForBulkAdjustmentAsync(
                dto.OrganizationTypeId,
                dto.CustomerId,
                effectiveDate,
                dto.TestIds,
                dto.TestPanelIds,
                dto.ApplyToAll);

        var result =
            new List<Barman.Application.DTOs.TestTariff.TariffBulkAdjustmentPreviewDto>();

        foreach (var current in tariffs)
        {
            if (effectiveDate <= current.ValidFrom)
            {
                throw new InvalidOperationException(
                    $"تاریخ اجرای تعرفه جدید برای «{current.Name}» باید بعد از تاریخ شروع تعرفه فعلی باشد.");
            }

            var newPrice =
                current.Price * (1m + dto.Percentage / 100m);

            if (newPrice < 0)
            {
                throw new InvalidOperationException(
                    $"قیمت جدید برای «{current.Name}» نمی‌تواند منفی باشد.");
            }

            result.Add(new Barman.Application.DTOs.TestTariff.TariffBulkAdjustmentPreviewDto
            {
                TariffId = current.Id,
                TestId = current.TestId,
                TestPanelId = current.TestPanelId,

                Code = current.Code,
                Name = current.Name,

                TargetType =
                    current.TestId.HasValue
                        ? "آزمون"
                        : "پنل",

                OldPrice = current.Price,
                Percentage = dto.Percentage,

                NewPrice =
                    Math.Round(
                        newPrice,
                        2,
                        MidpointRounding.AwayFromZero),

                Currency =
                    string.IsNullOrWhiteSpace(current.Currency)
                        ? "ریال"
                        : current.Currency,

                OldValidFrom = current.ValidFrom,
                OldValidTo = current.ValidTo,

                NewValidFrom = effectiveDate
            });
        }

        return result;
    }
    public async Task<int> ApplyBulkAdjustmentAsync(
    Barman.Application.DTOs.TestTariff.TariffBulkAdjustmentDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (dto.OrganizationTypeId == Guid.Empty)
            throw new ArgumentException(
                "نوع سازمان الزامی است.");

        if (dto.EffectiveDate == default)
            throw new ArgumentException(
                "تاریخ شروع تعرفه جدید الزامی است.");

        if (dto.Percentage == 0)
            throw new ArgumentException(
                "درصد تغییر نمی‌تواند صفر باشد.");

        if (!dto.ApplyToAll &&
            (dto.TestIds == null || dto.TestIds.Count == 0) &&
            (dto.TestPanelIds == null || dto.TestPanelIds.Count == 0))
        {
            throw new ArgumentException(
                "حداقل یک آزمون یا پنل باید انتخاب شود.");
        }

        if (dto.ExpectedTariffIds == null ||
            dto.ExpectedTariffIds.Count == 0)
        {
            throw new InvalidOperationException(
                "پیش‌نمایش معتبر برای اعمال تغییرات وجود ندارد.");
        }

        var effectiveDate =
            DateTime.SpecifyKind(
                dto.EffectiveDate.Date,
                DateTimeKind.Utc);

        var tariffs =
            await _unitOfWork.TestTariffs.GetForBulkAdjustmentAsync(
                dto.OrganizationTypeId,
                dto.CustomerId,
                effectiveDate,
                dto.TestIds,
                dto.TestPanelIds,
                dto.ApplyToAll);

        if (tariffs.Count == 0)
        {
            throw new InvalidOperationException(
                "تعرفه فعالی برای اعمال تغییرات پیدا نشد.");
        }

        var expectedIds =
            dto.ExpectedTariffIds.ToHashSet();

        var actualIds =
            tariffs.Select(x => x.Id).ToHashSet();

        if (!expectedIds.SetEquals(actualIds))
        {
            throw new InvalidOperationException(
                "اطلاعات تعرفه‌ها از زمان پیش‌نمایش تغییر کرده است. لطفاً پیش‌نمایش را دوباره ایجاد کنید.");
        }

        foreach (var current in tariffs)
        {
            if (!dto.ExpectedOldPrices.TryGetValue(
                    current.Id,
                    out var expectedOldPrice))
            {
                throw new InvalidOperationException(
                    "اطلاعات پیش‌نمایش کامل نیست. لطفاً پیش‌نمایش را دوباره ایجاد کنید.");
            }

            if (current.Price != expectedOldPrice)
            {
                throw new InvalidOperationException(
                    $"قیمت تعرفه «{current.Name}» از زمان پیش‌نمایش تغییر کرده است. لطفاً پیش‌نمایش را دوباره ایجاد کنید.");
            }

            if (effectiveDate <= current.ValidFrom)
            {
                throw new InvalidOperationException(
                    $"تاریخ اجرای تعرفه جدید برای «{current.Name}» باید بعد از تاریخ شروع تعرفه فعلی باشد.");
            }
        }

        var newTariffs = new List<TestTariff>();

        foreach (var current in tariffs)
        {
            var newPrice =
                current.Price *
                (1m + dto.Percentage / 100m);

            if (newPrice < 0)
            {
                throw new InvalidOperationException(
                    $"قیمت جدید برای تعرفه «{current.Name}» نمی‌تواند منفی باشد.");
            }

            current.ValidTo =
                effectiveDate.AddDays(-1);

            _unitOfWork.TestTariffs.Update(current);

            var newTariff = new TestTariff
            {
                Id = Guid.NewGuid(),

                Code = current.Code,
                Name = current.Name,

                TestId = current.TestId,
                TestPanelId = current.TestPanelId,

                OrganizationTypeId =
                    current.OrganizationTypeId,

                CustomerId =
                    current.CustomerId,

                StandardSampleId =
                    current.StandardSampleId,

                MatrixId =
                    current.MatrixId,

                Price =
                    Math.Round(
                        newPrice,
                        2,
                        MidpointRounding.AwayFromZero),

                Currency =
                    string.IsNullOrWhiteSpace(current.Currency)
                        ? "ریال"
                        : current.Currency,

                ValidFrom =
                    effectiveDate,

                ValidTo = null,

                Priority =
                    current.Priority,

                Description =
                    current.Description,

                IsDeleted = false,
                IsActive = true
            };

            newTariffs.Add(newTariff);
        }

        foreach (var tariff in newTariffs)
        {
            await _unitOfWork.TestTariffs.AddAsync(tariff);
        }

        await _unitOfWork.SaveChangesAsync();

        return newTariffs.Count;
    }
    public async Task<TestTariff> CreateAsync(
        TestTariff tariff,
        CancellationToken cancellationToken = default)
    {
        if (tariff == null)
            throw new ArgumentNullException(nameof(tariff));

        ValidateTariff(tariff);
        tariff.ValidFrom =
            DateTime.SpecifyKind(tariff.ValidFrom.Date, DateTimeKind.Utc);

        if (tariff.ValidTo.HasValue)
        {
            tariff.ValidTo =
                DateTime.SpecifyKind(
                    tariff.ValidTo.Value.Date,
                    DateTimeKind.Utc);
        }

        tariff.Id = Guid.NewGuid();
        tariff.Code = tariff.Code.Trim().ToUpperInvariant();
        tariff.Name = tariff.Name.Trim();
        tariff.Currency = string.IsNullOrWhiteSpace(tariff.Currency)
                ? "ریال"
                : tariff.Currency.Trim();

        tariff.IsDeleted = false;
        tariff.IsActive = true;

        await _unitOfWork.TestTariffs.AddAsync(tariff);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return tariff;
    }

    public async Task<TestTariff> UpdateAsync(
        Guid id,
        TestTariff tariff,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("شناسه تعرفه نامعتبر است.");

        if (tariff == null)
            throw new ArgumentNullException(nameof(tariff));

        ValidateTariff(tariff);
        tariff.ValidFrom =
            DateTime.SpecifyKind(tariff.ValidFrom.Date, DateTimeKind.Utc);

        if (tariff.ValidTo.HasValue)
        {
            tariff.ValidTo =
                DateTime.SpecifyKind(
                    tariff.ValidTo.Value.Date,
                    DateTimeKind.Utc);
        }

        var existing =
            await _unitOfWork.TestTariffs.GetByIdAsync(id);

        if (existing == null)
            throw new KeyNotFoundException(
                "تعرفه یافت نشد.");

        existing.Code =
            tariff.Code.Trim().ToUpperInvariant();

        existing.Name =
            tariff.Name.Trim();

        existing.TestId =
            tariff.TestId;

        existing.TestPanelId =
            tariff.TestPanelId;

        existing.OrganizationTypeId =
            tariff.OrganizationTypeId;

        existing.CustomerId =
            tariff.CustomerId;

        existing.StandardSampleId =
             tariff.StandardSampleId;

        existing.MatrixId =
            tariff.MatrixId;

        existing.Price =
            tariff.Price;

        existing.Currency =
    string.IsNullOrWhiteSpace(tariff.Currency)
        ? "ریال"
        : tariff.Currency.Trim();

        existing.ValidFrom =
            tariff.ValidFrom;

        existing.ValidTo =
            tariff.ValidTo;

        existing.Priority =
            tariff.Priority;

        existing.Description =
            tariff.Description;

        existing.IsActive =
            tariff.IsActive;

        _unitOfWork.TestTariffs.Update(existing);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return existing;
    }

    public async Task<TestTariff> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var tariff =
            await _unitOfWork.TestTariffs.GetByIdAsync(id);

        if (tariff == null)
            throw new KeyNotFoundException(
                "تعرفه یافت نشد.");

        tariff.IsDeleted = true;
        tariff.IsActive = false;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return tariff;
    }

    private static void ValidateTariff(TestTariff tariff)
    {
        if (tariff.OrganizationTypeId == Guid.Empty)
            throw new ArgumentException(
                "نوع سازمان الزامی است.");

        if (!tariff.TestId.HasValue &&
            !tariff.TestPanelId.HasValue)
            throw new ArgumentException(
                "تعرفه باید برای یک آزمون یا پنل تعیین شود.");

        if (tariff.TestId.HasValue &&
            tariff.TestPanelId.HasValue)
            throw new ArgumentException(
                "تعرفه نمی‌تواند همزمان برای آزمون و پنل باشد.");

        if (tariff.Price < 0)
            throw new ArgumentException(
                "مبلغ تعرفه نمی‌تواند منفی باشد.");

        if (tariff.ValidTo.HasValue &&
            tariff.ValidTo.Value < tariff.ValidFrom)
            throw new ArgumentException(
                "تاریخ پایان اعتبار نمی‌تواند قبل از تاریخ شروع باشد.");
    }
}