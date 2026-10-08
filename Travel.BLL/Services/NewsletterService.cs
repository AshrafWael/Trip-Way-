using Travel.BLL.Interfaces;
using Travel.DAL.Entities;
using Travel.DAL.UnitOfWork;

namespace Travel.BLL.Services;

public class NewsletterService : INewsletterService
{
    private readonly IUnitOfWork _unitOfWork;

    public NewsletterService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> SubscribeAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();

        var alreadySubscribed = await _unitOfWork.NewsletterSubscribers.AnyAsync(
            n => n.Email == normalized, cancellationToken);

        if (alreadySubscribed)
            return false;

        await _unitOfWork.NewsletterSubscribers.AddAsync(
            new NewsletterSubscriber { Email = normalized }, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
