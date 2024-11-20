using Microsoft.AspNetCore.Mvc;
using FinancialManagementSystem1.Models;
using System.Linq;
using System.Threading.Tasks;
using FinancialManagementSystem1.Data;
using Microsoft.EntityFrameworkCore;

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
        [HttpGet]
        public IActionResult Income()
        {

            try
            {
                // Verificar si el usuario está logueado
                var userIdString = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userIdString))
                {
                    // Si no hay usuario logueado, redirige a la página de login
                    return RedirectToAction("Login", "Home");
                }

                int userId = int.Parse(userIdString);

                // Buscar los ingresos del usuario logueado
                var incomes = _context.Incomes.Where(i => i.UserId == userId).ToList();

                // Obtener el nombre del usuario logueado
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    ViewData["Username"] = user.Username; // Pasar el nombre del usuario a la vista
                }

                return View(incomes);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en Income: " + ex.Message);
                throw;
            }

        }

        // Vista para agregar un nuevo ingreso
        [HttpGet]
        public IActionResult CreateIncome()
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // Si no hay usuario logueado, redirige a la página de login
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateIncome(Income model)
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // Si no hay usuario logueado, redirige a la página de login
                return RedirectToAction("Login", "Home");
            }

            // Obtener el UserId de la sesión
            int userId = int.Parse(userIdString);

            // Asignar el UserId al nuevo ingreso
            model.UserId = userId;

            // Guardar el nuevo ingreso en la base de datos
            _context.Incomes.Add(model);
            _context.SaveChanges();

            // Redirigir de nuevo a la vista de Ingresos
            return RedirectToAction("Income");
        }

        [HttpGet]
        public IActionResult EditIncome(int id)
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // Si no hay usuario logueado, redirige a la página de login
                return RedirectToAction("Login", "Home");
            }

            int userId = int.Parse(userIdString);

            // Buscar el ingreso por ID
            var income = _context.Incomes.FirstOrDefault(i => i.Id == id);

            if (income == null || income.UserId.ToString() != userIdString)
            {
                return NotFound();
            }

            // Buscar el nombre del usuario logueado
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Pasar el nombre del usuario a la vista a través de ViewData
            ViewData["UserName"] = user.Username;

            return View(income);
        }

        [HttpPost]
        public IActionResult EditIncome(Income model)
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // Si no hay usuario logueado, redirige a la página de login
                return RedirectToAction("Login", "Home");
            }

            // Buscar el ingreso por ID
            var income = _context.Incomes.FirstOrDefault(i => i.Id == model.Id);

            if (income == null || income.UserId.ToString() != userIdString)
            {
                return NotFound();
            }

            // Actualizar los valores del ingreso con los datos proporcionados por el usuario
            income.Source = model.Source;
            income.Amount = model.Amount;
            income.Date = model.Date;

            // Guardar los cambios en la base de datos
            _context.SaveChanges();

            // Redirigir a la lista de ingresos
            return RedirectToAction("Income");
        }



        // Método para eliminar un ingreso
        [HttpGet]
        public IActionResult DeleteIncome(int id)
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // Si no hay usuario logueado, redirige a la página de login
                return RedirectToAction("Login", "Home");
            }

            int userId = int.Parse(userIdString);

            // Buscar el ingreso por ID
            var income = _context.Incomes.FirstOrDefault(i => i.Id == id);

            if (income == null || income.UserId.ToString() != userIdString)
            {
                return NotFound();
            }

            // Buscar el nombre del usuario logueado
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Pasar el nombre del usuario a la vista a través de ViewData
            ViewData["UserName"] = user.Username;


            return View(income);
        }

        [HttpPost]
        public IActionResult DeleteIncomeConfirmed(int id)
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // Si no hay usuario logueado, redirige a la página de login
                return RedirectToAction("Login", "Home");
            }

            // Buscar el ingreso por ID
            var income = _context.Incomes.FirstOrDefault(i => i.Id == id);

            if (income == null || income.UserId.ToString() != userIdString)
            {
                return NotFound();
            }

            // Eliminar el ingreso
            _context.Incomes.Remove(income);
            _context.SaveChanges();

            // Redirigir a la lista de ingresos
            return RedirectToAction("Income");
        }

        [HttpGet]
        public IActionResult Expense()
        {
            try
            {
                // Verificar si el usuario está logueado
                var userIdString = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userIdString))
                {
                    // Si no hay usuario logueado, redirige a la página de login
                    return RedirectToAction("Login", "Home");
                }

                int userId = int.Parse(userIdString);

                // Buscar los gastos del usuario logueado y cargar la categoría asociada
                var expenses = _context.Expenses
                                       .Where(i => i.UserId == userId)
                                       .Include(e => e.Category) // Eager Loading de Category
                                       .ToList();

                // Obtener el nombre del usuario logueado
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    ViewData["Username"] = user.Username; // Pasar el nombre del usuario a la vista
                }

                return View(expenses);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en Expense: " + ex.Message);
                throw;
            }
        }

        // Acción para crear un nuevo gasto
        [HttpGet]
        public IActionResult CreateExpense()
        {
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Home");
            }

            // Cargar las categorías desde la base de datos
            ViewBag.Categories = _context.Categories.ToList();



            var model = new Expense();
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateExpense(Expense model)
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // Si no hay usuario logueado, redirige a la página de login
                return RedirectToAction("Login", "Home");
            }

            // Obtener el UserId de la sesión
            int userId = int.Parse(userIdString);

            // Asignar el UserId al nuevo gasto
            model.UserId = userId;

            // Guardar el nuevo ingreso en la base de datos
            _context.Expenses.Add(model);
            _context.SaveChanges();

            // Redirigir de nuevo a la vista de Gastos
            return RedirectToAction("Expense");
        }

        // Acción para editar un gasto
        [HttpGet]
        public IActionResult EditExpense(int id)
        {
            try
            {
                // Verificar si el usuario está logueado
                var userIdString = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userIdString))
                {
                    // Si no hay usuario logueado, redirige a la página de login
                    return RedirectToAction("Login", "Home");
                }

                int userId = int.Parse(userIdString);

                // Buscar el gasto por su Id y asegurarse de que pertenece al usuario logueado
                var expense = _context.Expenses
                                      .Include(e => e.Category)
                                      .FirstOrDefault(e => e.Id == id && e.UserId == userId);

                if (expense == null)
                {
                    return NotFound();
                }

                // Cargar todas las categorías para el dropdown de selección en la vista
                ViewBag.Categories = _context.Categories.ToList();

                // Buscar el nombre del usuario logueado
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return RedirectToAction("Login", "Home");
                }

                // Pasar el nombre del usuario a la vista a través de ViewData
                ViewData["UserName"] = user.Username;

                return View(expense);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en EditExpense: " + ex.Message);
                throw;
            }
        }


        [HttpPost]
        public async Task<IActionResult> EditExpense(Expense model)
        {
            try
            {
                // Verificar si el usuario está logueado
                var userIdString = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userIdString))
                {
                    // Si no hay usuario logueado, redirige a la página de login
                    return RedirectToAction("Login", "Home");
                }

                int userId = int.Parse(userIdString);

                // Verificar si el gasto realmente pertenece al usuario logueado
                var expense = _context.Expenses.FirstOrDefault(e => e.Id == model.Id && e.UserId == userId);

                if (expense == null)
                {
                    return NotFound();
                }

                // Actualizar los campos del gasto
                expense.Description = model.Description;
                expense.Amount = model.Amount;
                expense.Date = model.Date;
                expense.CategoryId = model.CategoryId;  // Si se cambia la categoría

                // Guardar los cambios en la base de datos
                _context.SaveChanges();

                // Redirigir a la lista de gastos después de la edición
                return RedirectToAction("Expense");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en EditExpense (POST): " + ex.Message);
                throw;
            }
        }


        // Acción para eliminar un gasto
        [HttpGet]
        public IActionResult DeleteExpense(int id)
        {
            try
            {
                // Verificar si el usuario está logueado
                var userIdString = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userIdString))
                {
                    // Si no hay usuario logueado, redirige a la página de login
                    return RedirectToAction("Login", "Home");
                }

                int userId = int.Parse(userIdString);

                // Buscar el gasto por su Id y asegurarse de que pertenece al usuario logueado
                var expense = _context.Expenses
                                      .Include(e => e.Category)
                                      .FirstOrDefault(e => e.Id == id && e.UserId == userId);

                if (expense == null)
                {
                    return NotFound();
                }

                // Buscar el nombre del usuario logueado
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return RedirectToAction("Login", "Home");
                }

                // Pasar el nombre del usuario a la vista a través de ViewData
                ViewData["UserName"] = user.Username;
                // Pasar el gasto a la vista para confirmación
                return View(expense);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteExpense (GET): " + ex.Message);
                throw;
            }
        }

        [HttpPost, ActionName("DeleteExpense")]
        public async Task<IActionResult> DeleteExpenseConfirmed(int id)
        {
            try
            {
                // Verificar si el usuario está logueado
                var userIdString = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userIdString))
                {
                    // Si no hay usuario logueado, redirige a la página de login
                    return RedirectToAction("Login", "Home");
                }

                int userId = int.Parse(userIdString);

                // Buscar el gasto por su Id y asegurarse de que pertenece al usuario logueado
                var expense = _context.Expenses.FirstOrDefault(e => e.Id == id && e.UserId == userId);

                if (expense == null)
                {
                    return NotFound();
                }

                // Eliminar el gasto
                _context.Expenses.Remove(expense);
                _context.SaveChanges();

                // Redirigir de nuevo a la vista de Gastos después de la eliminación
                return RedirectToAction("Expense");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteExpense (POST): " + ex.Message);
                throw;
            }
        }



        [HttpGet]
        public IActionResult Budget()
        {
            try
            {
                // Verificar si el usuario está logueado
                var userIdString = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userIdString))
                {
                    // Si no hay usuario logueado, redirige a la página de login
                    return RedirectToAction("Login", "Home");
                }

                int userId = int.Parse(userIdString);

                // Obtener los presupuestos del usuario logueado
                var budgets = _context.Budgets.Where(b => b.UserId == userId).ToList();

                // Obtener el nombre del usuario logueado
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    ViewData["Username"] = user.Username; // Pasar el nombre del usuario a la vista
                }

                return View(budgets);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en Budget (GET): " + ex.Message);
                throw;
            }
        }


        [HttpGet]
        public IActionResult CreateBudget()
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        [HttpPost]
        public IActionResult CreateBudget(Budget model)
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Home");
            }

            // Obtener el UserId de la sesión
            int userId = int.Parse(userIdString);

            // Asignar el UserId al nuevo presupuesto
            model.UserId = userId;

            // Guardar el nuevo presupuesto en la base de datos
            _context.Budgets.Add(model);
            _context.SaveChanges();

            // Redirigir de nuevo a la vista de Presupuestos
            return RedirectToAction("Budget");
        }

        [HttpGet]
        public IActionResult EditBudget(int id)
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Home");
            }

            int userId = int.Parse(userIdString);

            // Buscar el presupuesto por su Id y asegurarse de que pertenece al usuario logueado
            var budget = _context.Budgets.FirstOrDefault(b => b.Id == id && b.UserId == userId);

            if (budget == null)
            {
                return NotFound();
            }

            // Buscar el nombre del usuario logueado
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Pasar el nombre del usuario a la vista a través de ViewData
            ViewData["UserName"] = user.Username;

            return View(budget);
        }

        [HttpPost]
        public IActionResult EditBudget(Budget model)
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Home");
            }

            // Verificar si el presupuesto pertenece al usuario
            var budget = _context.Budgets.FirstOrDefault(b => b.Id == model.Id && b.UserId == int.Parse(userIdString));

            if (budget == null)
            {
                return NotFound();
            }

            // Actualizar los valores del presupuesto
            budget.Name = model.Name;
            budget.TotalAmount = model.TotalAmount;
            budget.AmountSpent = model.AmountSpent;
            budget.StartDate = model.StartDate;
            budget.EndDate = model.EndDate;

            // Guardar los cambios
            _context.SaveChanges();

            // Redirigir a la lista de presupuestos
            return RedirectToAction("Budget");
        }


        // Acción para confirmar la eliminación de un presupuesto
        [HttpGet]
        public IActionResult DeleteBudget(int id)
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // Si no hay usuario logueado, redirige a la página de login
                return RedirectToAction("Login", "Home");
            }

            int userId = int.Parse(userIdString);

            // Buscar el presupuesto por su ID
            var budget = _context.Budgets.FirstOrDefault(b => b.Id == id);

            if (budget == null || budget.UserId != int.Parse(userIdString))
            {
                // Si el presupuesto no existe o no pertenece al usuario logueado
                return NotFound();
            }

            // Buscar el nombre del usuario logueado
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Pasar el nombre del usuario a la vista a través de ViewData
            ViewData["UserName"] = user.Username;

            return View(budget); // Mostrar la vista de confirmación
        }

        // Acción para eliminar el presupuesto
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // Si no hay usuario logueado, redirige a la página de login
                return RedirectToAction("Login", "Home");
            }

            // Buscar el presupuesto por su ID
            var budget = _context.Budgets.FirstOrDefault(b => b.Id == id);

            if (budget == null || budget.UserId != int.Parse(userIdString))
            {
                // Si el presupuesto no existe o no pertenece al usuario logueado
                return NotFound();
            }

            // Eliminar el presupuesto
            _context.Budgets.Remove(budget);
            _context.SaveChanges();

            // Redirigir a la vista de presupuestos
            return RedirectToAction("Budget");
        }



        // Acción para mostrar la lista de reportes
        [HttpGet]
        public IActionResult Report()
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // Si no hay usuario logueado, redirige a la página de login
                return RedirectToAction("Login", "Home");
            }

            int userId = int.Parse(userIdString);

            // Obtener los reportes del usuario logueado
            var reports = _context.Reports.Where(r => r.UserId == userId).ToList();

            // Obtener el nombre del usuario logueado
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                ViewData["Username"] = user.Username; // Pasar el nombre del usuario a la vista
            }



            return View(reports);
        }

        // Acción para crear un nuevo reporte
        [HttpGet]
        public IActionResult CreateReport()
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // Si no hay usuario logueado, redirige a la página de login
                return RedirectToAction("Login", "Home");
            }

            int userId = int.Parse(userIdString);

            // Calcular el total de ingresos y gastos del usuario
            var totalIncome = _context.Incomes.Where(i => i.UserId == userId).Sum(i => (decimal?)i.Amount) ?? 0;
            var totalExpenses = _context.Expenses.Where(e => e.UserId == userId).Sum(e => (decimal?)e.Amount) ?? 0;

            // Crear un nuevo reporte con los valores calculados
            var model = new Report
            {
                ReportDate = DateTime.Now,
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                NetProfit = totalIncome - totalExpenses
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport(Report model)
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // Si no hay usuario logueado, redirige a la página de login
                return RedirectToAction("Login", "Home");
            }

            int userId = int.Parse(userIdString);

            // Asignar el UserId al nuevo reporte
            model.UserId = userId;

            // Guardar el nuevo reporte en la base de datos
            _context.Reports.Add(model);
            await _context.SaveChangesAsync();

            // Redirigir a la vista de reportes
            return RedirectToAction("Report");
        }

        // Acción para mostrar el formulario de edición de un reporte
        [HttpGet]
        public IActionResult EditReport(int id)
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // Si no hay usuario logueado, redirige a la página de login
                return RedirectToAction("Login", "Home");
            }

            int userId = int.Parse(userIdString);

            // Buscar el ingreso por ID
            var report = _context.Reports.FirstOrDefault(i => i.Id == id);

            if (report == null || report.UserId.ToString() != userIdString)
            {
                return NotFound();
            }

            // Buscar el nombre del usuario logueado
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Pasar el nombre del usuario a la vista a través de ViewData
            ViewData["UserName"] = user.Username;

            return View(report);
        }

        [HttpPost]
        public async Task<IActionResult> EditReport(Report model)
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // Si no hay usuario logueado, redirige a la página de login
                return RedirectToAction("Login", "Home");
            }

            // Buscar el ingreso por ID
            var report = _context.Reports.FirstOrDefault(i => i.Id == model.Id);

            if (report == null || report.UserId.ToString() != userIdString)
            {
                return NotFound();
            }

            // Actualizar los valores del ingreso con los datos proporcionados por el usuario
            report.ReportDate = model.ReportDate;
            report.TotalIncome = model.TotalIncome;
            report.TotalExpenses = model.TotalExpenses;
            report.NetProfit = model.NetProfit;

            // Guardar los cambios en la base de datos
            _context.SaveChanges();

            // Redirigir a la lista de ingresos
            return RedirectToAction("Report");
        }

        // Método para eliminar un ingreso
        [HttpGet]
        public IActionResult DeleteReport(int id)
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // Si no hay usuario logueado, redirige a la página de login
                return RedirectToAction("Login", "Home");
            }

            int userId = int.Parse(userIdString);

            // Buscar el ingreso por ID
            var report = _context.Reports.FirstOrDefault(i => i.Id == id);

            if (report == null || report.UserId.ToString() != userIdString)
            {
                return NotFound();
            }

            // Buscar el nombre del usuario logueado
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Pasar el nombre del usuario a la vista a través de ViewData
            ViewData["UserName"] = user.Username;

            return View(report);
        }

        [HttpPost]
        public IActionResult DeleteReportConfirmed(int id)
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // Si no hay usuario logueado, redirige a la página de login
                return RedirectToAction("Login", "Home");
            }

            // Buscar el ingreso por ID
            var report = _context.Reports.FirstOrDefault(i => i.Id == id);

            if (report == null || report.UserId.ToString() != userIdString)
            {
                return NotFound();
            }

            // Eliminar el ingreso
            _context.Reports.Remove(report);
            _context.SaveChanges();

            // Redirigir a la lista de ingresos
            return RedirectToAction("Report");
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
