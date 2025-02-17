namespace PicHub.API.Models
{
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class ConfirmRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmationCode { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RefreshRequest
    {
        public string RefreshToken { get; set; }
    }

    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }


}
