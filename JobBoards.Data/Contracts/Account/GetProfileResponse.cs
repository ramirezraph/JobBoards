namespace JobBoards.Data.Contracts.Account;

public class GetProfileResponse
{
    public string Id { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
}