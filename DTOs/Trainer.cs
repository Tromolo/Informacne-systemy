namespace VIS_projekt.DTOs
{
    public class Trainer
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Specialisation { get; set; }
        public string? Skills { get; set; }
        public string? PasswordHash { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
