using FinancialManagementSystem1.Models;
using FinancialManagementSystem1.Data;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using Microsoft.AspNetCore.Identity; // Para encriptar la contrase�a
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // Para manejar la sesi�n
using System.Security.Cryptography;
using System.Text;

namespace FinancialManagementSystem1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FinancialDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public HomeController(ILogger<HomeController> logger, FinancialDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _logger = logger;
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o contrase�a incorrectos.");
                return View();
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o contrase�a incorrectos.");
                return View();
            }

            // Verificar si la sesi�n se guarda correctamente
            Console.WriteLine("Iniciando sesi�n para usuario: " + user.Id);

            // Guardar el UserId en la sesi�n
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("RoleId", user.RoleId.ToString());

            // Redirigir seg�n el rol
            if (user.RoleId == 1) // Administrador
            {
                return RedirectToAction("AdminDashboard", "Admin");
            }
            else if (user.RoleId == 2) // Usuario regular
            {
                return RedirectToAction("Profile", new { id = user.Id });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Rol de usuario no reconocido.");
                return View();
            }
        }


        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string Username, string Email, string Password, string ConfirmPassword)
        {
            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError("", "Las contrase�as no coinciden.");
                return View();
            }

            var userExists = _context.Users.Any(u => u.Email == Email);
            if (userExists)
            {
                ModelState.AddModelError("", "Ya existe un usuario con este correo electr�nico.");
                return View();
            }

            // Crear nuevo usuario
            var user = new User
            {
                Username = Username,
                Email = Email,
                RoleId = 2 // Supongamos que 2 es un rol de usuario regular
            };

            // Encriptar la contrase�a
            user.PasswordHash = _passwordHasher.HashPassword(user, Password);

            // Guardar usuario en la base de datos
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Iniciar sesi�n
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Username", user.Username);

            // Redirigir al perfil del usuario
            return RedirectToAction("Profile", new { id = user.Id });
        }


        [HttpGet]
        public IActionResult Profile()
        {

            // Obtener el UserId desde la sesi�n
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Home");
            }

            int userId = int.Parse(userIdString);

            // Buscar al usuario por ID
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            // Calcular el total de ingresos y gastos
            var totalIncome = _context.Incomes.Where(i => i.UserId == userId).Sum(i => i.Amount);
            var totalExpenses = _context.Expenses.Where(e => e.UserId == userId).Sum(e => e.Amount);

            // Obtener datos de ingresos mensuales
            var monthlyIncomes = _context.Incomes
                .Where(i => i.UserId == userId)
                .AsEnumerable() // Cambia a evaluaci�n en cliente
                .GroupBy(i => i.Date.ToString("yyyy-MM"))
                .Select(g => new
                {
                    Month = g.Key,
                    TotalAmount = g.Sum(i => i.Amount)
                })
                .OrderBy(x => x.Month)
                .ToList();

            // Obtener datos de gastos mensuales
            var monthlyExpenses = _context.Expenses
                .Where(e => e.UserId == userId)
                .AsEnumerable() // Cambia a evaluaci�n en cliente
                .GroupBy(e => e.Date.ToString("yyyy-MM"))
                .Select(g => new
                {
                    Month = g.Key,
                    TotalAmount = g.Sum(e => e.Amount)
                })
                .OrderBy(x => x.Month)
                .ToList();



            // Pasar datos a la vista
            ViewData["TotalIncome"] = totalIncome;
            ViewData["TotalExpenses"] = totalExpenses;
            ViewData["MonthlyIncomes"] = monthlyIncomes;
            ViewData["MonthlyExpenses"] = monthlyExpenses;

            return View(user);
        }

        // M�todo para verificar la contrase�a (deber�as tener la l�gica de encriptaci�n)
        private bool VerifyPasswordHash(string password, string storedHash)
        {
            // L�gica de comparaci�n de hash de la contrase�a
            using (var sha256 = SHA256.Create())
            {
                var computedHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var computedHashString = Convert.ToBase64String(computedHash);
                return storedHash == computedHashString;
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult CreateOpinion()
        { // Verificar si el usuario est� logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // Si no hay usuario logueado, redirige a la p�gina de login
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        // Acci�n para procesar la opini�n enviada por el usuario
        [HttpPost]
        public IActionResult CreateOpinion(Opinion model)
        {
            // Verificar si el usuario est� logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // Si no hay usuario logueado, redirige a la p�gina de login
                return RedirectToAction("Login", "Home");
            }

            // Obtener el UserId de la sesi�n
            int userId = int.Parse(userIdString);

            // Asignar el UserId al nuevo ingreso
            model.UserId = userId;

            // Guardar el nuevo ingreso en la base de datos
            _context.Opinions.Add(model);
            _context.SaveChanges();

            // Redirigir de nuevo a la vista de Ingresos
            return RedirectToAction("Profile");
        }
    }
}
