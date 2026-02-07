namespace Selu383.SP26.Api.Features.Users
{
    public class UserCreateDto
    {
        public string UserName { get; set; } = string.Empty;
        public string[] Roles { get; set; } = new string[0];
        public string Password { get; set; } = string.Empty;

    }
}