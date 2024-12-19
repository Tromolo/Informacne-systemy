namespace VIS_projekt.Controllers.Models
{
    public class RegisterTrainerRequest
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Specialisation { get; set; }
        public string? Skills { get; set; }
        public string Password { get; set; }
    }
}
