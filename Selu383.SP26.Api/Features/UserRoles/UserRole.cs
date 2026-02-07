using Selu383.SP26.Api.Features.Users;
using Selu383.SP26.Api.Features.Roles;
namespace Selu383.SP26.Api.Features;

public class UserRole
{
    public int UserId { get; set; }

    public required User User {  get; set; }

    public int RoleId { get; set; }

    public required Role Role { get; set; }
}
