namespace Barman.Domain.Enums;

public enum ReceptionStatus
{
    Draft = 0,
    WaitingForSection = 1,
    WaitingForAnalyst = 2,
    InProgress = 3,
    WaitingForReview = 4,
    WaitingForApproval = 5,
    Completed = 6,
    Rejected = 7
}