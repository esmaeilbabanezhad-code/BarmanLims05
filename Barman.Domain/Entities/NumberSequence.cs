using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class NumberSequence : BaseEntity
{
    public string EntityName { get; set; } = "";

    public string Prefix { get; set; } = "";

    public string Separator { get; set; } = "-";

    public bool UseYear { get; set; } = true;

    public bool PersianYear { get; set; } = false;

    public int YearDigits { get; set; } = 2;

    public int SequenceDigits { get; set; } = 5;

    public bool ResetEveryYear { get; set; } = true;

    public long LastNumber { get; set; }

    public int LastYear { get; set; }
}