using FinancialManagementSystem1.Data;
using FinancialManagementSystem1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;


public class AdminController : Controller
{
    private readonly FinancialDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AdminController(FinancialDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    public IActionResult RegisterAdmin()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RegisterAdmin(string Username, string Email, string Password, string ConfirmPassword)
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
            RoleId = 1 // Supongamos que 2 es un rol de usuario regular
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



    [HttpGet]
    public IActionResult AdminDashboard()
    {
        // Verificar si el usuario tiene permisos
        var roleId = HttpContext.Session.GetString("RoleId");
        if (string.IsNullOrEmpty(roleId) || roleId != "1")
        {
            return RedirectToAction("Login", "Home");
        }

        // Obtener el UserId desde la sesión
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


        return View(user);
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Home");
    }


    [HttpGet]
    public IActionResult UserList()
    {
        // Verificar si el usuario tiene permisos de administrador
        var roleId = HttpContext.Session.GetString("RoleId");
        if (string.IsNullOrEmpty(roleId) || roleId != "1")
        {
            return RedirectToAction("Login", "Home");
        }

        // Recuperar el nombre del administrador logueado
        var adminName = HttpContext.Session.GetString("Username");

        // Recuperar la lista de usuarios registrados
        var users = _context.Users
            .Select(u => new
            {
                Username = u.Username,
                Email = u.Email
            })
            .ToList();

        // Pasar el nombre del administrador a la vista
        ViewBag.AdminName = adminName;

        return View(users);
    }

    [HttpGet]
    public IActionResult UserOpinions()
    {// Verificar si el usuario tiene permisos de administrador
        var roleId = HttpContext.Session.GetString("RoleId");
        if (string.IsNullOrEmpty(roleId) || roleId != "1")
        {
            return RedirectToAction("Login", "Home");
        }

        // Recuperar el nombre del administrador logueado
        var adminName = HttpContext.Session.GetString("Username");

        // Recuperar usuarios y sus opiniones
        var userOpinions = _context.Opinions
            .Select(o => new
            {
                UserName = o.User.Username,
                OpinionContent = o.Content
            })
            .ToList();

        // Pasar el nombre del administrador a la vista
        ViewBag.AdminName = adminName;

        return View(userOpinions);
    }


}
