namespace Selu383.SP26.Api.Features.Logins
{
    public class LoginDto
    {
        public required string UserName { get; set; } = string.Empty;

        public required string Password { get; set; } = string.Empty;
    }
}