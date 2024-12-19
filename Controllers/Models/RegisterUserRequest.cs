namespace VIS_projekt.Controllers.Models
{
    public class RegisterUserRequest
    {
        public int? TrainerId { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public char Role { get; set; } = 'U'; 
        public string Password { get; set; }
    }
}
