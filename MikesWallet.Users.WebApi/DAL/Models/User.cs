namespace MikesWallet.Users.WebApi.DAL.Models;

public class User
{
    public required Guid Id { get; init; }
    
    public required string Email { get; init; }
    
    public required string PasswordHash { get; init; }
}