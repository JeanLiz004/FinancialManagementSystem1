namespace FinancialManagementSystem1.Models
{
    public class Opinion
    {
        public int Id { get; set; } // Identificador único de la opinión

        public string Content { get; set; } // Contenido de la opinión

        public DateTime CreatedAt { get; set; } // Fecha de creación de la opinión

        // Relación con el usuario
        public int UserId { get; set; } // Clave foránea que referencia al usuario
        public User User { get; set; } // Propiedad de navegación
    }
}
