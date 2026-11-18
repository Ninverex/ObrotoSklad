using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using ObrotoSklad.Infrastructure.Auth;


namespace ObrotoSklad.Application.Tests;

public class JwtTokenGeneratorTests
{
    private static JwtTokenGenerator CreateGenerator()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] = "test-secret-key-must-be-at-least-32-characters-long",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience"
            })
            .Build();

        return new JwtTokenGenerator(config);
    }

    private static User NewUser() => new() { Id = "user-123", Email = "jan@test.pl", UserName = "jan@test.pl" };

    [Fact]
    public void Token_ContainsUserIdEmailAndRole()
    {
        var token = CreateGenerator().GenerateToken(NewUser(), new List<string> { "Handlowiec" });

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal("user-123", jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        Assert.Equal("jan@test.pl", jwt.Claims.First(c => c.Type == ClaimTypes.Email).Value);
        Assert.Equal("Handlowiec", jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value);
    }

    [Fact]
    public void Token_HasIssuerAudienceAndFutureExpiry()
    {
        var token = CreateGenerator().GenerateToken(NewUser(), new List<string> { "Admin" });

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal("TestIssuer", jwt.Issuer);
        Assert.Contains("TestAudience", jwt.Audiences);
        Assert.True(jwt.ValidTo > DateTime.UtcNow);
    }

    [Fact]
    public void Token_WithMultipleRoles_ContainsAllOfThem()
    {
        var token = CreateGenerator().GenerateToken(NewUser(), new List<string> { "Admin", "Magazynier" });

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        var roles = jwt.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
        Assert.Contains("Admin", roles);
        Assert.Contains("Magazynier", roles);
    }
}