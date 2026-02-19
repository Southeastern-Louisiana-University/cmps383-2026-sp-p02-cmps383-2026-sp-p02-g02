using Selu383.SP26.Api.Features.Users;
using Selu383.SP26.Api.Features.Roles;
using Microsoft.AspNetCore.Identity;
namespace Selu383.SP26.Api.Features.UserRoles
{
    public class UserRole : IdentityUserRole<int>
    {
        public User User { get; set; } = default!;
        public Role Role { get; set; } = default!;
    }
}