using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MikesWallet.Users.WebApi.DAL;
using MikesWallet.Users.WebApi.DAL.Models;
using MikesWallet.Users.WebApi.Requests;

namespace MikesWallet.Users.WebApi.Infrastructure;

public static class Extensions
{
    public static void MapAuthEndpoints(this RouteGroupBuilder mainGroup)
    {
        var authGroup = mainGroup.MapGroup("/auth");
        
        authGroup.MapPost("/sign-up", async (SignUpRequest request, ApplicationDbContext dbContext, CancellationToken cancellationToken) =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = new User
            {
                Id = Guid.CreateVersion7(),
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            };
    
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync(cancellationToken);
    
            return Results.Ok(user.Id);
        });

        authGroup.MapPost("/sign-in", async (
            SignInRequest request, 
            IOptions<AccessTokenSettings> tokenSettingsAccessor,
            ApplicationDbContext dbContext,
            TimeProvider timeProvider,
            CancellationToken cancellationToken) =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user is null)
            {
                return Results.BadRequest("Неверный логин или пароль.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Results.BadRequest("Неверный логин или пароль.");
            }
            
            var tokenSettings = tokenSettingsAccessor.Value;
            
            var currentDateTime = timeProvider.GetUtcNow().UtcDateTime;
            var expires = currentDateTime.Add(tokenSettings.TokenLifetime);
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.SecretKey)),
                SecurityAlgorithms.HmacSha256
            );

            var claims = new Dictionary<string, object>
            {
                { JwtRegisteredClaimNames.Sub, user.Id },
                { JwtRegisteredClaimNames.Email, user.Email },
            };

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = tokenSettings.Issuer,
                Audience = tokenSettings.Audience,
                Claims = claims,
                Expires = expires,
                SigningCredentials = signingCredentials,
                NotBefore = currentDateTime,
                IssuedAt = currentDateTime,
            };

            var accessToken = new JsonWebTokenHandler().CreateToken(descriptor);
            return Results.Ok(accessToken);
        });
    }
}