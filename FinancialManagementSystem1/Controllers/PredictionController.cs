using Microsoft.AspNetCore.Mvc;
using FinancialManagementSystem1.Models;
using Microsoft.ML;
using System;
using System.Linq;
using FinancialManagementSystem1.Data;


namespace FinancialManagementSystem1.Controllers
{
    public class PredictionController : Controller
    {
        private readonly FinancialDbContext _context;
        private readonly MLContext _mlContext;

        public PredictionController(FinancialDbContext context)
        {
            _context = context;
            _mlContext = new MLContext();
        }

        [HttpGet]
        public IActionResult CashFlowPrediction()
        {
            // Obtener el ID del usuario logueado
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Home");
            }
            int userId = int.Parse(userIdString);

            // Obtener los datos históricos de ingresos y gastos del usuario
            var incomes = _context.Incomes
                .Where(i => i.UserId == userId)
                .Select(i => i.Amount)
                .ToList();

            var expenses = _context.Expenses
                .Where(e => e.UserId == userId)
                .Select(e => e.Amount)
                .ToList();

            // Aquí puedes integrar ML.NET para entrenar un modelo de predicción de flujo de caja
            // con una serie temporal (por ejemplo, ARIMA, Prophet o una RNN).

            // Simulación de resultados de predicción
            var predictedIncome = incomes.Average(); // Reemplazar con predicción real
            var predictedExpenses = expenses.Average(); // Reemplazar con predicción real
            var predictedNetCashFlow = predictedIncome - predictedExpenses;

            ViewBag.PredictedIncome = predictedIncome;
            ViewBag.PredictedExpenses = predictedExpenses;
            ViewBag.PredictedNetCashFlow = predictedNetCashFlow;

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                ViewData["Username"] = user.Username; // Pasar el nombre del usuario a la vista
            }

            return View();
        }
    }
}
