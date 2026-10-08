using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travel.DAL.Entities;

namespace Travel.DAL.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.TotalPrice).HasColumnType("decimal(10,2)");
        builder.Property(b => b.SpecialRequests).HasMaxLength(1000);
        builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(b => b.UserId);
        builder.HasIndex(b => b.Status);

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Booking_NumberOfTravelers_Positive",
            "[NumberOfTravelers] > 0"));

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Booking_TotalPrice_NonNegative",
            "[TotalPrice] >= 0"));

        builder.HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            // Deleting a user account must not silently wipe booking
            // history used for revenue/reporting; disable the account
            // instead at the application level.
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.TravelPackage)
            .WithMany(p => p.Bookings)
            .HasForeignKey(b => b.TravelPackageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
