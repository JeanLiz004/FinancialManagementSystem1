namespace FinancialManagementSystem1.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } // Ejemplo: "Alimentos", "Transporte", "Entretenimiento"

        // Relación con la clase Expense
        public ICollection<Expense> Expenses { get; set; }

    }
}
