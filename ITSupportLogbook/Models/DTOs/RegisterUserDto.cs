namespace ITSupportLogbook.Models.DTOs
{
    public class RegisterUserDto
    {
        public required string FirstName { get; set; } = string.Empty;
        public required string LastName { get; set; } = string.Empty;
        public required string Password { get; set; } = string.Empty;
    }
}
