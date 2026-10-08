using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travel.DAL.Entities;

namespace Travel.DAL.Data.Configurations;

public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        builder.ToTable("BlogPosts");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title).IsRequired().HasMaxLength(250);
        builder.Property(b => b.ArabicTitle).IsRequired().HasMaxLength(250);
        builder.Property(b => b.Content).IsRequired();
        builder.Property(b => b.ArabicContent).IsRequired();
        builder.Property(b => b.ImageUrl).HasMaxLength(500);

        builder.HasIndex(b => new { b.IsPublished, b.PublishedAt });

        builder.HasOne(b => b.Author)
            .WithMany(u => u.BlogPosts)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
