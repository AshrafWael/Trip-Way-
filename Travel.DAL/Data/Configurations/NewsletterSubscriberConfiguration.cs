using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travel.DAL.Entities;

namespace Travel.DAL.Data.Configurations;

public class NewsletterSubscriberConfiguration : IEntityTypeConfiguration<NewsletterSubscriber>
{
    public void Configure(EntityTypeBuilder<NewsletterSubscriber> builder)
    {
        builder.ToTable("NewsletterSubscribers");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Email).IsRequired().HasMaxLength(256);

        // Re-subscribing with the same address should be a no-op, not a
        // duplicate row -- enforced here as the last line of defense
        // behind the service-layer AnyAsync check.
        builder.HasIndex(n => n.Email).IsUnique();
    }
}
