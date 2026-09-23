using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class CustomFieldDefinition : BaseEntity
{
    public string Title { get; set; } = "";

    public string DataType { get; set; } = "Text";

    public bool IsRequired { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsReusable { get; set; } = true;

    public int DisplayOrder { get; set; }

    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public Guid? SampleCategoryId { get; set; }
    public SampleCategory? SampleCategory { get; set; }

    public Guid? MatrixId { get; set; }
    public Matrix? Matrix { get; set; }

    public ICollection<CustomFieldValue> Values { get; set; }
        = new List<CustomFieldValue>();
}