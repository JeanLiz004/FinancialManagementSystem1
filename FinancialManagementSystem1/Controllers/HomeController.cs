using FinancialManagementSystem1.Models;
using FinancialManagementSystem1.Data;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using Microsoft.AspNetCore.Identity; // Para encriptar la contraseña
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // Para manejar la sesión
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
            // Buscar el usuario por su nombre de usuario
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o contraseña incorrectos.");
                return View();
            }

            // Verificar la contraseña utilizando el PasswordHasher
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o contraseña incorrectos.");
                return View();
            }

            // Guardar el UserId en la sesión
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Username", user.Username);

            // Redirigir al perfil del usuario
            return RedirectToAction("Profile", new { id = user.Id });
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
                ModelState.AddModelError("", "Las contraseñas no coinciden.");
                return View();
            }

            var userExists = _context.Users.Any(u => u.Email == Email);
            if (userExists)
            {
                ModelState.AddModelError("", "Ya existe un usuario con este correo electrónico.");
                return View();
            }

            // Crear nuevo usuario
            var user = new User
            {
                Username = Username,
                Email = Email,
                RoleId = 2 // Supongamos que 2 es un rol de usuario regular
            };

            // Encriptar la contraseña
            user.PasswordHash = _passwordHasher.HashPassword(user, Password);

            // Guardar usuario en la base de datos
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Iniciar sesión
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Username", user.Username);

            // Redirigir al perfil del usuario
            return RedirectToAction("Profile", new { id = user.Id });
        }

        public IActionResult Profile(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Método para verificar la contraseña (deberías tener la lógica de encriptación)
        private bool VerifyPasswordHash(string password, string storedHash)
        {
            // Lógica de comparación de hash de la contraseña
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
            HttpContext.Session.Clear(); // Cerrar la sesión
            return RedirectToAction("Index", "Home");
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
    }
}
