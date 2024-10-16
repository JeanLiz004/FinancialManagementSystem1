namespace FinancialManagementSystem1.Models
{
    public class Budget
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountSpent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Relación con el usuario
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
