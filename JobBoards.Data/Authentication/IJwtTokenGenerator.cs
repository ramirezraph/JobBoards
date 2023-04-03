using JobBoards.Data.Identity;

namespace JobBoards.Data.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(ApplicationUser user);
}