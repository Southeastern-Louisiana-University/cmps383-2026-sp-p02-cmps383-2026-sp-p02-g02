using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Selu383.SP26.Api.Features.Users;
using Selu383.SP26.Api.Features.Logins;
using Selu383.SP26.Api.Features.Roles;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Selu383.SP26.Api.Controllers;

[Route("api/authentication")]
[ApiController]

public class AuthenticationController : ControllerBase
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public AuthenticationController(SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) {
            return Unauthorized();
        }

        return Ok(new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Roles = (await _userManager.GetRolesAsync(user)).ToArray()
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.UserName);
        if (user == null)
        {
            return BadRequest("Invalid username or password.");
        }

        var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);
        if (!result.Succeeded)
        {
            return BadRequest("Invalid username or password.");
        }

        return Ok(new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Roles = (await _userManager.GetRolesAsync(user)).ToArray()
        });
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] UserCreateDto userDto)
    {
        var userExist = await _userManager.FindByNameAsync(userDto.UserName);
        if (userExist != null)
        {
            return BadRequest("Username is taken!");
        }

        var user = new User
        {
            UserName = userDto.UserName
        };

        var result = await _userManager.CreateAsync(user, userDto.Password);
        if (!result.Succeeded)
        {
            return BadRequest("Something went wrong.");
        }

        foreach (var role in userDto.Roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                return BadRequest($"Role '{role}' does not exist.");
            }

            await _userManager.AddToRoleAsync(user, role);
        }

        return Ok(new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Roles = userDto.Roles
        });
    }


    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

}