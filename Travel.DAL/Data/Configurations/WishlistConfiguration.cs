using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travel.DAL.Entities;

namespace Travel.DAL.Data.Configurations;

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.ToTable("Wishlists");

        builder.HasKey(w => w.Id);

        // A user can only wishlist a given package once.
        builder.HasIndex(w => new { w.UserId, w.TravelPackageId }).IsUnique();

        builder.HasOne(w => w.User)
            .WithMany(u => u.WishlistItems)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(w => w.TravelPackage)
            .WithMany(p => p.WishlistedBy)
            .HasForeignKey(w => w.TravelPackageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
