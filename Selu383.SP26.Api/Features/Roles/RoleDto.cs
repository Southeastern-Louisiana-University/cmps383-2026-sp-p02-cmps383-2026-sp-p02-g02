using Selu383.SP26.Api.Features.UserRoles;

namespace Selu383.SP26.Api.Features.Roles;

public class RoleDto
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public List<UserRole> UserRoles { get; set; } = new();
}
