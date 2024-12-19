namespace VIS_projekt.Controllers.Models
{
    public class CreateTrainingPlanRequest
    {
        public int UserId { get; set; }
        public string? Description { get; set; }
        public bool Active { get; set; } = true;
    }
}
