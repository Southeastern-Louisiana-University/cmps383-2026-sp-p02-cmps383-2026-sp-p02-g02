using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Selu383.SP26.Api.Data;
using Selu383.SP26.Api.Features.Users;
using Selu383.SP26.Api.Features.Locations;

namespace Selu383.SP26.Api.Controllers;

[Route("api/locations")]
[ApiController]
public class LocationsController(
    DataContext dataContext,
    UserManager<User> userManager
    ) : ControllerBase
{
    [HttpGet]
    public IQueryable<LocationDto> GetAll()
    {
        return dataContext.Set<Location>()
            .Select(x => new LocationDto
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                TableCount = x.TableCount,
                ManagerId = x.ManagerId,
            });
    }

    [HttpGet("{id}")]
    public ActionResult<LocationDto> GetById(int id)
    {
        var result = dataContext.Set<Location>()
            .FirstOrDefault(x => x.Id == id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(new LocationDto
        {
            Id = result.Id,
            Name = result.Name,
            Address = result.Address,
            TableCount = result.TableCount,
            ManagerId = result.ManagerId,
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public ActionResult<LocationDto> Create(LocationDto dto)
    {
        if (IsLocationInvalid(dto))
        {
            return BadRequest();
        }

        if (dto.Id > 0)
        {
            var managerExists = dataContext.Users.Any(u => u.Id == dto.ManagerId);
            if (!managerExists)
            {
                return BadRequest("Invalid ManagerId. User does not exist.");
            }
        }

        var location = new Location
        {
            Name = dto.Name,
            Address = dto.Address,
            TableCount = dto.TableCount,
            ManagerId = dto.ManagerId
        };

        dataContext.Set<Location>().Add(location);
        dataContext.SaveChanges();

        dto.Id = location.Id;

        return CreatedAtAction(
            nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut]
    [Route("{id}")]
    [Authorize]
    public async Task<ActionResult<LocationDto>> Update(int id, LocationDto dto)
    {
        if (string.IsNullOrEmpty(dto.Name))
        {
            return BadRequest("Name is required.");
        }

        if (dto.Name.Length > 120)
        {
            return BadRequest("Name is too long.");
        }

        if (string.IsNullOrEmpty(dto.Address))
        {
            return BadRequest("Address is required.");
        }

        if (dto.TableCount <= 0)
        {
            return BadRequest("TableCount must be greater than 0.");
        }

        var location = dataContext.Set<Location>()
            .FirstOrDefault(x => x.Id == id);

        if (location == null)
        {
            return NotFound();
        }

        // check if user is admin or the manager of this location
        var currentUser = await userManager.GetUserAsync(User);
        var isAdmin = currentUser != null && await userManager.IsInRoleAsync(currentUser, "Admin");
        var isManager = currentUser != null && location.ManagerId == currentUser.Id;

        if (!isAdmin && !isManager)
        {
            return Forbid();
        }

        // update existing entity with new values from dto
        location.Name = dto.Name;
        location.Address = dto.Address;
        location.TableCount = dto.TableCount;
        location.ManagerId = dto.ManagerId;

        dataContext.SaveChanges();

        // entity -> dto
        var locationDto = new LocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Address = dto.Address,
            TableCount = dto.TableCount,
            ManagerId = dto.ManagerId
        };

        return Ok(locationDto);
    }

    [HttpDelete]
    [Route("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        var location = dataContext.Set<Location>()
            .FirstOrDefault(x => x.Id == id);

        if (location == null)
        {
            return NotFound();
        }

        // check if user is admin or the manager of this location
        var currentUser = await userManager.GetUserAsync(User);
        var isAdmin = currentUser != null && await userManager.IsInRoleAsync(currentUser, "Admin");
        var isManager = currentUser != null && location.ManagerId == currentUser.Id;

        if (!isAdmin && !isManager)
        {
            return Forbid();
        }

        dataContext.Set<Location>().Remove(location);
        dataContext.SaveChanges();

        return Ok();
    }

    // check to see if location is valid
    private static bool IsLocationInvalid(LocationDto dto)
    {
        return string.IsNullOrWhiteSpace(dto.Name) ||
            dto.Name.Length > 120 ||
            string.IsNullOrWhiteSpace(dto.Address) ||
            dto.TableCount <= 0;
    }


}
