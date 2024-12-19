namespace VIS_projekt.TableModule
{
    public class Payment
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
    }
}
