namespace Barman.Application.DTOs.Test;

public class ResultSetStatusDto
{
    public Guid AssignmentId { get; set; }

    public int ResultCount { get; set; }

    public int RequiredResultCount { get; set; }

    public int OutOfLimitCount { get; set; }

    public bool HasAnyLimit { get; set; }

    public bool IsResultSet =>
        RequiredResultCount > 0;

    public bool HasNoResult =>
        ResultCount == 0;

    public bool IsIncomplete =>
        ResultCount > 0 &&
        ResultCount < RequiredResultCount;

    public bool IsOutOfLimit =>
        OutOfLimitCount > 0;

    public bool HasNoApplicableLimit =>
        !IsOutOfLimit &&
        ResultCount >= RequiredResultCount &&
        !HasAnyLimit;

    public bool IsNormal =>
        ResultCount >= RequiredResultCount &&
        !IsOutOfLimit &&
        HasAnyLimit;
}