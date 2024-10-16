namespace FinancialManagementSystem1.Models
{
    public class Report
    {
        public int Id { get; set; }
        public DateTime ReportDate { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }

        // Relación con el usuario
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
