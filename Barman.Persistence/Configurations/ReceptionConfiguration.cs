using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class ReceptionConfiguration : IEntityTypeConfiguration<Reception>
{
    public void Configure(EntityTypeBuilder<Reception> builder)
    {
        builder.ToTable("Receptions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReceptionNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.TotalPrice)
            .HasPrecision(18, 2);

        builder.HasIndex(x => x.ReceptionNumber)
            .IsUnique();

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TechManager)
            .WithMany()
            .HasForeignKey(x => x.TechManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Director)
            .WithMany()
            .HasForeignKey(x => x.DirectorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Samples)
            .WithOne(x => x.Reception)
            .HasForeignKey(x => x.ReceptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}