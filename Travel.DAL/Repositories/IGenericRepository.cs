using System.Linq.Expressions;

namespace Travel.DAL.Repositories;

/// <summary>
/// Generic, reusable data-access contract shared by every entity.
/// Specialized repositories (e.g. IPackageRepository) are added only
/// where eager-loading or filtering needs go beyond what this generic
/// surface can express cleanly.
/// </summary>
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Exposes IQueryable for callers (typically the service layer) that
    /// need to compose paging/sorting/filtering/Include chains without
    /// forcing every possible combination through dedicated methods.
    /// </summary>
    IQueryable<T> Query(bool asNoTracking = true);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Remove(T entity);

    Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
}
