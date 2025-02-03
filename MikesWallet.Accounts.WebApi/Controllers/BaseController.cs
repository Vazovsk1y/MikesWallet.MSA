using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace MikesWallet.Accounts.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    protected Guid GetRequiredCurrentUserId()
    {
        var id = HttpContext.User.Claims.First(e => e.Type == JwtRegisteredClaimNames.Sub).Value;
        return Guid.Parse(id);
    }
}