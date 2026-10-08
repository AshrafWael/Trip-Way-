namespace Travel.BLL.Exceptions;

/// <summary>
/// Thrown for invalid login credentials. Mapped to a 401 (not a 400) by
/// the centralized exception-handling middleware added in Phase 4 --
/// kept distinct from BusinessRuleException for that reason.
/// </summary>
public class AuthenticationFailedException : Exception
{
    public AuthenticationFailedException(string message) : base(message) { }
}
