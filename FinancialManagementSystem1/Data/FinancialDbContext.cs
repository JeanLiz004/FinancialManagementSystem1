using FinancialManagementSystem1.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagementSystem1.Data
{
    public class FinancialDbContext:DbContext
    {
        public FinancialDbContext(DbContextOptions<FinancialDbContext> options): base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de relaciones User - Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de relaciones User - Income
            modelBuilder.Entity<Income>()
                .HasOne(i => i.User)
                .WithMany(u => u.Incomes)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de relaciones User - Expense
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.User)
                .WithMany(u => u.Expenses)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de relaciones Expense - Category
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Expenses)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de relaciones User - Budget
            modelBuilder.Entity<Budget>()
                .HasOne(b => b.User)
                .WithMany(u => u.Budgets)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de relaciones User - Report
            modelBuilder.Entity<Report>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configurar decimal con precisión y escala para evitar truncamientos.

            // Entidad Budget
            modelBuilder.Entity<Budget>()
                .Property(b => b.AmountSpent)
                .HasPrecision(18, 2); // Por ejemplo, 18 dígitos en total y 2 después del punto decimal.

            modelBuilder.Entity<Budget>()
                .Property(b => b.TotalAmount)
                .HasPrecision(18, 2);

            // Entidad Expense
            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasPrecision(18, 2);

            // Entidad Income
            modelBuilder.Entity<Income>()
                .Property(i => i.Amount)
                .HasPrecision(18, 2);

            // Entidad Report
            modelBuilder.Entity<Report>()
                .Property(r => r.NetProfit)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Report>()
                .Property(r => r.TotalExpenses)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Report>()
                .Property(r => r.TotalIncome)
                .HasPrecision(18, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}
