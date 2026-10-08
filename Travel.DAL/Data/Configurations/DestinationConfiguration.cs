using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travel.DAL.Entities;

namespace Travel.DAL.Data.Configurations;

public class DestinationConfiguration : IEntityTypeConfiguration<Destination>
{
    public void Configure(EntityTypeBuilder<Destination> builder)
    {
        builder.ToTable("Destinations");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name).IsRequired().HasMaxLength(150);
        builder.Property(d => d.ArabicName).IsRequired().HasMaxLength(150);
        builder.Property(d => d.Description).IsRequired().HasMaxLength(4000);
        builder.Property(d => d.ArabicDescription).IsRequired().HasMaxLength(4000);
        builder.Property(d => d.Country).IsRequired().HasMaxLength(100);
        builder.Property(d => d.City).IsRequired().HasMaxLength(100);
        builder.Property(d => d.ImageUrl).HasMaxLength(500);

        builder.HasIndex(d => d.Name);
        builder.HasIndex(d => new { d.Country, d.City });

        builder.HasMany(d => d.TravelPackages)
            .WithOne(p => p.Destination)
            .HasForeignKey(p => p.DestinationId)
            // A destination with existing packages cannot be silently
            // cascade-deleted along with its bookings/reviews history.
            .OnDelete(DeleteBehavior.Restrict);
    }
}
