namespace ObrotoSklad.Application.Auth;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, IList<string> roles);
}
