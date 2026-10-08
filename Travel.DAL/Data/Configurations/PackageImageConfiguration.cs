using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travel.DAL.Entities;

namespace Travel.DAL.Data.Configurations;

public class PackageImageConfiguration : IEntityTypeConfiguration<PackageImage>
{
    public void Configure(EntityTypeBuilder<PackageImage> builder)
    {
        builder.ToTable("PackageImages");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ImageUrl).IsRequired().HasMaxLength(500);

        builder.HasIndex(i => new { i.TravelPackageId, i.DisplayOrder });
    }
}
