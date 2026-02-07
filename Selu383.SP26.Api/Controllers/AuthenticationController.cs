using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Selu383.SP26.Api.Data;
using Selu383.SP26.Api.Features.Users;
using Selu383.SP26.Api.Features.Logins;
using System.Threading.Tasks;

namespace Selu383.SP26.Api.Controllers;

[Route("api/authentication")]
[ApiController]

public class AuthenticationController(
    UserManager<User> userManager,
    SignInManager<User> signInManager
    ) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDto loginDto)
    {
        var user = await userManager.FindByNameAsync(loginDto.UserName);

        if (user == null)
        {
            return Unauthorized();
        }
        var passwordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!passwordValid)
        {
            return Unauthorized();
        }

        await signInManager.SignInAsync(user, isPersistent: false);

        return Ok();

    }

}