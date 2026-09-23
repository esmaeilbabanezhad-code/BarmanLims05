using Barman.Domain.Entities;

namespace Barman.Application.DTOs.TestTariff;

public class TariffImportRowResultDto
{
    public TariffImportRowDto Row { get; set; } = new();

    public bool IsValid { get; set; }

    public List<string> Errors { get; set; } = new();

    public List<string> Warnings { get; set; } = new();

    public Guid? OrganizationTypeId { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? StandardSampleId { get; set; }

    public Guid? SampleCategoryId { get; set; }

    public Guid? MatrixId { get; set; }

    public Guid? TestId { get; set; }

    public Guid? TestPanelId { get; set; }

    public DateTime? ParsedValidFrom { get; set; }

    public DateTime? ParsedValidTo { get; set; }

    public Barman.Domain.Entities.TestTariff? ExistingTariff { get; set; }
}