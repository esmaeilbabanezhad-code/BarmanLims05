using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class TestPanelItemConfiguration
    : IEntityTypeConfiguration<TestPanelItem>
{
    public void Configure(
        EntityTypeBuilder<TestPanelItem> builder)
    {
        builder.ToTable("TestPanelItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SortOrder)
            .IsRequired();

        builder.HasOne(x => x.TestPanel)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.TestPanelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Test)
            .WithMany(x => x.PanelItems)
            .HasForeignKey(x => x.TestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DefaultAnalyst)
            .WithMany()
            .HasForeignKey(x => x.DefaultAnalystId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => new
        {
            x.TestPanelId,
            x.TestId
        })
        .IsUnique();

        builder.HasIndex(x => new
        {
            x.TestPanelId,
            x.SortOrder
        });
    }
}