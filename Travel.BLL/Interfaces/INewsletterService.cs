namespace Travel.BLL.Interfaces;

public interface INewsletterService
{
    /// <returns>false if the address was already subscribed (not an error -- treated as a no-op success by the caller).</returns>
    Task<bool> SubscribeAsync(string email, CancellationToken cancellationToken = default);
}
