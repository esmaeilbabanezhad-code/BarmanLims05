using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class TechnicalManagerScopeConfiguration
    : IEntityTypeConfiguration<TechnicalManagerScope>
{
    public void Configure(
        EntityTypeBuilder<TechnicalManagerScope> builder)
    {
        builder.ToTable("TechnicalManagerScopes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.Priority)
            .IsRequired()
            .HasDefaultValue(100);

        builder.HasIndex(x => x.Code)
            .IsUnique();
    }
}