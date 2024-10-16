namespace FinancialManagementSystem1.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } // Ejemplo: "Admin", "User"

        // Relación con la clase Usuario
        public ICollection<User> Users { get; set; }
    }
}
