using Travel.DAL.Entities;

namespace Travel.BLL.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(ApplicationUser user, IList<string> roles);
}
