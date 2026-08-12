namespace ass01.BusinessLogic.DTOs.Auth;

public class LoginResponse
{
    public string Token { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
}
