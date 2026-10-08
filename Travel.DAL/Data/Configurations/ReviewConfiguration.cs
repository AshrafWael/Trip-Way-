using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travel.DAL.Entities;

namespace Travel.DAL.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Comment).HasMaxLength(2000);

        builder.HasIndex(r => new { r.TravelPackageId, r.IsApproved });

        // One review per traveler per package.
        builder.HasIndex(r => new { r.UserId, r.TravelPackageId }).IsUnique();

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Review_Rating_Range",
            "[Rating] BETWEEN 1 AND 5"));

        builder.HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.TravelPackage)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.TravelPackageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
