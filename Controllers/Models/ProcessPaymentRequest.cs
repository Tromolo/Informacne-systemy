namespace VIS_projekt.Controllers.Models
{
    public class ProcessPaymentRequest
    {
        public int UserId { get; set; }
        public string? Type { get; set; }
        public decimal Amount { get; set; }
    }
}
