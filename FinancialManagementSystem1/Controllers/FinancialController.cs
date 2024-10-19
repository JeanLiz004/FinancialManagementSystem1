using Microsoft.AspNetCore.Mvc;
using FinancialManagementSystem1.Models;
using System.Linq;
using System.Threading.Tasks;
using FinancialManagementSystem1.Data;

namespace FinancialManagementSystem1.Controllers
{
    public class FinancialController : Controller
    {

        private readonly FinancialDbContext _context;

        public FinancialController(FinancialDbContext context)
        {
            _context = context;
        }

        // Mostrar los ingresos del usuario logueado
        public IActionResult Income()
        {
            // Obtener el Id del usuario logueado desde la sesión
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Home");
            }

            // Convertir el UserId a entero
            int parsedUserId = int.Parse(userId);

            // Obtener los ingresos del usuario logueado
            var incomes = _context.Incomes.Where(i => i.UserId == parsedUserId).ToList();

            return View(incomes);
        }



        public IActionResult Expense()
        {
            // Lógica para mostrar gastos
            return View();
        }

        public IActionResult Budget()
        {
            // Lógica para mostrar presupuesto
            return View();
        }

        public IActionResult Report()
        {
            // Lógica para mostrar reporte
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
