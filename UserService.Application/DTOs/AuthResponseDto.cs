namespace UserService.Application.DTOs
{
    public class AuthResponseDto
    {
        public UserDto User { get; set; } = new UserDto();
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
