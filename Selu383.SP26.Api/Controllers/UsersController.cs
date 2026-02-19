using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Selu383.SP26.Api.Features.Roles;
using Selu383.SP26.Api.Features.Users;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Selu383.SP25.P02.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UserController(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Roles = _userManager.GetRolesAsync(user).Result.ToArray()
            }).ToList();

            return Ok(userDtos);
        }

        // get user by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return Ok(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Roles = (await _userManager.GetRolesAsync(user)).ToArray()
            });
        }

        // create new user
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
        {
            // validation stuff
            if (string.IsNullOrWhiteSpace(dto.UserName))
            {
                return BadRequest("Username cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Password is required.");
            }

            if (dto.Roles == null || dto.Roles.Length == 0)
            {
                return BadRequest("At least one role is required.");
            }

            // does the username exist?
            var userExists = await _userManager.FindByNameAsync(dto.UserName);
            if (userExists != null)
            {
                return BadRequest("Username already exists.");
            }

            var user = new User
            {
                UserName = dto.UserName
            };

            // create user with pass
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            // is the role valid?
            foreach (var role in dto.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    return BadRequest($"Role '{role}' does not exist.");
                }
            }

            // assign role to user
            await _userManager.AddToRolesAsync(user, dto.Roles);

            return Ok(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Roles = dto.Roles
            });
        }

        // update user's roles
        [HttpPut("{id}/roles")]
        public async Task<IActionResult> UpdateUserRoles(int id, [FromBody] string[] roles)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            // remove existing roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // add new roles
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    return BadRequest($"Role '{role}' does not exist.");
                }
            }
            await _userManager.AddToRolesAsync(user, roles);

            return Ok(new { message = $"User {user.UserName} roles updated successfully!" });
        }

        // delete user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            await _userManager.DeleteAsync(user);
            return Ok(new { message = $"User {user.UserName} deleted successfully!" });
        }
    }
}