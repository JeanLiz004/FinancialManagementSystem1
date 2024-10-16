namespace FinancialManagementSystem1.Models
{
    public class Income
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        // Relación con el usuario
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
