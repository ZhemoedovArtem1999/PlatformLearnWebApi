namespace PlatformLearnWebApi.Models.Auth
{
    public class RegisterRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTime DateBirth { get; set; }
        public string Gender { get; set; }
    }
}
