namespace Travel.BLL.Exceptions;

/// <summary>
/// Thrown when a requested entity doesn't exist. Mapped to a 404 by the
/// centralized exception-handling middleware added in Phase 4.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string entityName, object key)
        : base($"{entityName} with id '{key}' was not found.") { }
}
