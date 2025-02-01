namespace MikesWallet.Users.Infrastructure;

public record AccessTokenSettings
{
    public required string Audience { get; init; }

    public required string Issuer { get; init; }

    public required string SecretKey { get; init; }
    
    public TimeSpan ClockSkew { get; init; }

    public TimeSpan TokenLifetime { get; init; }
}