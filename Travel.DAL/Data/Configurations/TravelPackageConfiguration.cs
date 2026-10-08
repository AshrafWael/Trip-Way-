using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travel.DAL.Entities;

namespace Travel.DAL.Data.Configurations;

public class TravelPackageConfiguration : IEntityTypeConfiguration<TravelPackage>
{
    public void Configure(EntityTypeBuilder<TravelPackage> builder)
    {
        builder.ToTable("TravelPackages");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
        builder.Property(p => p.ArabicTitle).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).IsRequired().HasMaxLength(4000);
        builder.Property(p => p.ArabicDescription).IsRequired().HasMaxLength(4000);
        builder.Property(p => p.MainImageUrl).HasMaxLength(500);

        builder.Property(p => p.Price).HasColumnType("decimal(10,2)");
        builder.Property(p => p.DiscountPrice).HasColumnType("decimal(10,2)");

        builder.HasIndex(p => p.Title);
        builder.HasIndex(p => new { p.IsActive, p.IsFeatured });
        builder.HasIndex(p => p.Price);

        // Enforced again in the service layer / FluentValidation, but a
        // database-level guard is the last line of defense.
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_TravelPackage_DiscountPrice",
            "[DiscountPrice] IS NULL OR [DiscountPrice] <= [Price]"));

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_TravelPackage_Price_NonNegative",
            "[Price] >= 0"));

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_TravelPackage_AvailableSeats_NonNegative",
            "[AvailableSeats] >= 0"));

        builder.HasOne(p => p.Destination)
            .WithMany(d => d.TravelPackages)
            .HasForeignKey(p => p.DestinationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Images)
            .WithOne(i => i.TravelPackage)
            .HasForeignKey(i => i.TravelPackageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Reviews)
            .WithOne(r => r.TravelPackage)
            .HasForeignKey(r => r.TravelPackageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Bookings)
            .WithOne(b => b.TravelPackage)
            .HasForeignKey(b => b.TravelPackageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.WishlistedBy)
            .WithOne(w => w.TravelPackage)
            .HasForeignKey(w => w.TravelPackageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
