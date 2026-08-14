using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class TestPanelConfiguration : IEntityTypeConfiguration<TestPanel>
{
    public void Configure(EntityTypeBuilder<TestPanel> builder)
    {
        builder.ToTable("TestPanels");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.HasIndex(x => x.Code)
            .IsUnique();
    }
}