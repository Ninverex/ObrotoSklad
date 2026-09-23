namespace ObrotoSklad.Application.Auth;

public record class AuthResponseDto(string Token, string Email, string FirstName, string LastName, string Role);
