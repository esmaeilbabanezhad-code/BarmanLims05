namespace Barman.Domain.Enums;

public enum ResultSetStatus
{
    NoResult = 0,
    Incomplete = 1,
    OutOfLimit = 2,
    NoApplicableLimit = 3,
    Normal = 4
}