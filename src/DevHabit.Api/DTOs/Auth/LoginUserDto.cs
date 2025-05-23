namespace DevHabit.Api.DTOs.Auth;

public sealed record LoginUserDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
