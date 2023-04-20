namespace JobBoards.Data.Contracts.Account;

public class AuthenticationResponse
{
    public string Id { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Token { get; set; } = default!;
}