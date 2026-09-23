using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Identity;


namespace ObrotoSklad.Application.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }
    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user is null) return null;

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!isPasswordValid) return null;

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenGenerator.GenerateToken(user, roles);

        return new AuthResponseDto(token, user.Email!, user.FirstName!, user.LastName!, roles.FirstOrDefault() ?? "");   
    }
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var extUser = await _userManager.FindByEmailAsync(dto.Email);

        if (extUser is not null) throw new Exception("Użytkownik z takim mailem już istnieje.");

        var user = new User
        {
            Email = dto.Email,
            UserName = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        await _userManager.AddToRoleAsync(user, "Handlowiec");

        var token = _jwtTokenGenerator.GenerateToken(user, new List<string> {"Handlowiec"});

        return new AuthResponseDto(token, user.Email!, user.FirstName!, user.LastName!, "Handlowiec");

    }
}
