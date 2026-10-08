namespace Travel.BLL.Exceptions;

/// <summary>
/// Thrown when an operation violates a business rule (e.g. booking more
/// seats than are available). Mapped to a 400 by the centralized
/// exception-handling middleware added in Phase 4.
/// </summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}
