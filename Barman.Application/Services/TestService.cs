using Barman.Application.DTOs.Test;
using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class TestService : ITestService
{
    private readonly IUnitOfWork _unitOfWork;

    public TestService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TestLookupDto>> GetReceptionLookupAsync(
        CancellationToken cancellationToken = default)
    {
        var tests =
            await _unitOfWork.Tests.GetAllAsync();

        return tests
            .Where(x => x.IsActiveForReception)
            .OrderBy(x => x.Code)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<List<TestLookupDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var tests =
            await _unitOfWork.Tests.GetAllAsync();

        return tests
            .OrderBy(x => x.Code)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<List<TestLookupDto>> GetDeletedAsync(
    CancellationToken cancellationToken = default)
    {
        var tests =
            await _unitOfWork.Tests.GetDeletedAsync();

        return tests
            .OrderBy(x => x.Code)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<TestLookupDto?> RestoreAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        var test =
            await _unitOfWork.Tests.GetByIdAsync(id);

        if (test == null)
            return null;

        test.IsDeleted = false;
        test.IsActive = true;
        test.IsActiveForReception = true;

        _unitOfWork.Tests.Update(test);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return MapToDto(test);
    }

    public async Task<TestLookupDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        var test =
            await _unitOfWork.Tests.GetByIdAsync(id);

        return test == null
            ? null
            : MapToDto(test);
    }

    public async Task<TestLookupDto> CreateAsync(
        CreateTestDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Code))
            throw new ArgumentException("کد آزمون الزامی است.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("نام آزمون الزامی است.");

        var code =
            dto.Code.Trim();

        var existing =
            await _unitOfWork.Tests.GetByCodeAsync(code);

        if (existing != null)
            throw new InvalidOperationException(
                $"آزمونی با کد «{code}» قبلاً ثبت شده است.");

        var test = new Test
        {
            Code = code,
            Name = dto.Name.Trim(),

            EnglishName =
                Clean(dto.EnglishName),

            Unit =
                Clean(dto.Unit),

            Method =
                Clean(dto.Method),

            InstrumentName =
                Clean(dto.InstrumentName),

            LOD = dto.LOD,

            LOQ = dto.LOQ,

            DefaultResult =
                Clean(dto.DefaultResult),

            IsQuantitative =
                dto.IsQuantitative,

            IsActiveForReception =
                dto.IsActiveForReception,

            Description =
                Clean(dto.Description),

            DepartmentId = dto.DepartmentId,

            SampleCategoryId = dto.SampleCategoryId,

            MatrixId = dto.MatrixId,
        };

        await _unitOfWork.Tests.AddAsync(test);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return MapToDto(test);
    }

    public async Task<TestLookupDto?> UpdateAsync(
        Guid id,
        UpdateTestDto dto,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        if (string.IsNullOrWhiteSpace(dto.Code))
            throw new ArgumentException("کد آزمون الزامی است.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("نام آزمون الزامی است.");

        var test =
            await _unitOfWork.Tests.GetByIdAsync(id);

        if (test == null)
            return null;

        var code =
            dto.Code.Trim();

        var existing =
            await _unitOfWork.Tests.GetByCodeAsync(code);

        if (existing != null &&
            existing.Id != id)
        {
            throw new InvalidOperationException(
                $"آزمونی با کد «{code}» قبلاً ثبت شده است.");
        }

        test.Code = code;

        test.Name =
            dto.Name.Trim();

        test.EnglishName =
            Clean(dto.EnglishName);

        test.Unit =
            Clean(dto.Unit);

        test.Method =
            Clean(dto.Method);

        test.InstrumentName =
            Clean(dto.InstrumentName);

        test.LOD =
            dto.LOD;

        test.LOQ =
            dto.LOQ;

        test.DefaultResult =
            Clean(dto.DefaultResult);

        test.IsQuantitative =
            dto.IsQuantitative;

        test.IsActiveForReception =
            dto.IsActiveForReception;

        test.Description =
            Clean(dto.Description);

        test.DepartmentId = dto.DepartmentId;

        test.SampleCategoryId = dto.SampleCategoryId;

        test.MatrixId = dto.MatrixId;

        _unitOfWork.Tests.Update(test);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return MapToDto(test);
    }

    public async Task<List<Department>> GetDepartmentsAsync(
    CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Departments.GetActiveAsync();
    }

    public async Task<bool> DeleteAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return false;

        var test =
            await _unitOfWork.Tests.GetByIdAsync(id);

        if (test == null)
            return false;

        // حذف منطقی
        test.IsDeleted = true;

        // بعد از حذف، دیگر در پذیرش فعال نباشد
        test.IsActiveForReception = false;

        _unitOfWork.Tests.Update(test);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<TestLookupDto> ActivateAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        var test = await _unitOfWork.Tests.GetByIdAsync(id);

        if (test == null)
            throw new KeyNotFoundException("Test not found.");

        test.IsActive = true;
        test.IsActiveForReception = true;

        _unitOfWork.Tests.Update(test);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(test);
    }

    public async Task<TestLookupDto> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var test = await _unitOfWork.Tests.GetByIdAsync(id);

        if (test == null)
            throw new KeyNotFoundException("Test not found.");

        test.IsActive = false;
        test.IsActiveForReception = false;

        _unitOfWork.Tests.Update(test);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(test);
    }

    private static TestLookupDto MapToDto(Test test)
    {
        return new TestLookupDto
        {
            Id = test.Id,

            Code = test.Code,

            Name = test.Name,

            EnglishName = test.EnglishName,

            Unit = test.Unit,

            IsQuantitative =
                test.IsQuantitative,

            IsActiveForReception =
                test.IsActiveForReception,

            Method =
                test.Method,

            InstrumentName =
                test.InstrumentName,

            LOD =
                test.LOD,

            LOQ =
                test.LOQ,

            DepartmentId = test.DepartmentId,

            SampleCategoryId = test.SampleCategoryId,

            MatrixId = test.MatrixId,

            DefaultResult =
                test.DefaultResult,

            Description =
                test.Description
        };
    }

    private static string? Clean(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    public async Task<List<SampleCategory>> GetSampleCategoriesAsync(
    CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.SampleCategories
            .GetAllAsync(cancellationToken);
    }

    public async Task<List<Matrix>> GetMatricesAsync(
    Guid? sampleCategoryId = null,
    CancellationToken cancellationToken = default)
    {
        if (sampleCategoryId.HasValue &&
            sampleCategoryId.Value != Guid.Empty)
        {
            return await _unitOfWork.Matrices
                .GetBySampleCategoryIdAsync(
                    sampleCategoryId.Value);
        }

        return await _unitOfWork.Matrices
            .GetAllAsync(cancellationToken);
    }
    
}