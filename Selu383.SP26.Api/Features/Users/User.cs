namespace Selu383.SP26.Api.Features.Users;

public class User
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public List<UserRole> UserRoles { get; set; } = new();
}
