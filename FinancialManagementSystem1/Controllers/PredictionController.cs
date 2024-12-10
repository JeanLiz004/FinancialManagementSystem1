using Microsoft.AspNetCore.Mvc;
using FinancialManagementSystem1.Models;
using Microsoft.ML.Data;
using Microsoft.ML;
using System;
using System.Linq;
using FinancialManagementSystem1.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML.TimeSeries;
using Microsoft.ML.Transforms.TimeSeries;
using FinancialManagementSystem1.ViewModels;



namespace FinancialManagementSystem1.Controllers
{
    public class PredictionController : Controller
    {
        private readonly FinancialDbContext _context;


        public PredictionController(FinancialDbContext context)
        {
            _context = context;

        }

       





        [HttpGet]
        public IActionResult PredictExpenses()
        {
            // Verificar si el usuario está logueado
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Home");
            }

            int userId = int.Parse(userIdString);

            // Obtener el nombre del usuario
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Home"); // Redirige si no se encuentra el usuario
            }

            ViewBag.UserName = user.Username; // Guardar el nombre en ViewBag

            // Obtener los datos históricos de gastos
            var expenseData = _context.Expenses
                .Where(e => e.UserId == userId)
                .OrderBy(e => e.Date)
                .Select(e => new ExpenseData
                {
                    Date = e.Date,
                    Amount = (float)e.Amount // Conversión explícita
                })
                .ToList();

            // Obtener los datos históricos de ingresos
            var incomeData = _context.Incomes
                .Where(i => i.UserId == userId)
                .OrderBy(i => i.Date)
                .Select(i => new IncomeData
                {
                    Date = i.Date,
                    Amount = (float)i.Amount // Conversión explícita
                })
                .ToList();


            // Verificar que haya suficientes datos históricos para ambos
            if (expenseData.Count < 5 || incomeData.Count < 5)
            {
                ViewBag.ErrorMessage = "No hay suficientes datos históricos para realizar la predicción.";
                return View();
            }


            // Preparar el modelo ML.NET para gastos
            var mlContext = new MLContext();
            var expenseDataView = mlContext.Data.LoadFromEnumerable(expenseData);

            var expensePipeline = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedExpenses",
                inputColumnName: "Amount",
                windowSize: 5,
                seriesLength: expenseData.Count,
                trainSize: expenseData.Count,
                horizon: 1);

            var expenseModel = expensePipeline.Fit(expenseDataView);
            var expenseForecastingEngine = expenseModel.CreateTimeSeriesEngine<ExpenseData, ExpenseForecast>(mlContext);
            var expensePrediction = expenseForecastingEngine.Predict();

            // Preparar el modelo ML.NET para ingresos
            var incomeDataView = mlContext.Data.LoadFromEnumerable(incomeData);

            var incomePipeline = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedIncomes",
                inputColumnName: "Amount",
                windowSize: 5,
                seriesLength: incomeData.Count,
                trainSize: incomeData.Count,
                horizon: 1);

            var incomeModel = incomePipeline.Fit(incomeDataView);
            var incomeForecastingEngine = incomeModel.CreateTimeSeriesEngine<IncomeData, IncomeForecast>(mlContext);
            var incomePrediction = incomeForecastingEngine.Predict();

            // Preparar los resultados para la vista
            var result = new PredictionResultViewModel
            {
                // Predicción de gastos
                PredictedExpenseDate = DateTime.Now.AddMonths(1),
                PredictedExpense = expensePrediction.ForecastedExpenses[0],

                // Predicción de ingresos
                PredictedIncomeDate = DateTime.Now.AddMonths(1),
                PredictedIncome = incomePrediction.ForecastedIncomes[0]
            };

            return View(result);


        }

    }


}