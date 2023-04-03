using JobBoards.Data.Identity;

namespace JobBoards.Data.Authentication;

public interface IJwtTokenGenerator
{
    Task<string> GenerateToken(ApplicationUser user);
}