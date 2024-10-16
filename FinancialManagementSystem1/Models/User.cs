using System.Data;

namespace FinancialManagementSystem1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } // La contraseña debe almacenarse encriptada
        public string Email { get; set; }

        // Relación con la clase Rol
        public int RoleId { get; set; }
        public Role Role { get; set; }

        // Relaciones con las clases de datos financieros
        public ICollection<Income> Incomes { get; set; }
        public ICollection<Expense> Expenses { get; set; }
        public ICollection<Budget> Budgets { get; set; }
        public ICollection<Report> Reports { get; set; }
    }
}
