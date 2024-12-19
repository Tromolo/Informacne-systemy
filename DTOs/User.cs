namespace VIS_projekt.DTOs
{
    public class User
    {
        public int Id { get; set; }
        public int? TrainerId { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public char Role { get; set; }
        public string? PasswordHash { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
