using Selu383.SP26.Api.Features.Users;
using Selu383.SP26.Api.Features.Roles;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Selu383.SP26.Api.Features.UserRoles;

public class UserRoleDto
{
    public int UserId { get; set; }

    public required User User { get; set; }

    public int RoleId { get; set; }

    public required Role Role { get; set; }
}
