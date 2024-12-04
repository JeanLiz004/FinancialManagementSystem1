using Microsoft.ML.Data;

namespace FinancialManagementSystem1.ViewModels
{
    public class PredictionResultViewModel
    {
        // Predicción de ingresos
        public DateTime PredictedIncomeDate { get; set; }
        public float PredictedIncome { get; set; }

        // Predicción de gastos
        public DateTime PredictedExpenseDate { get; set; }
        public float PredictedExpense { get; set; }
    }

    public class IncomeData
    {
        public DateTime Date { get; set; }
        public float Amount { get; set; }
    }

    public class IncomeForecast
    {
        [VectorType(1)]
        public float[] ForecastedIncomes { get; set; }
    }

    public class ExpenseData
    {
        public DateTime Date { get; set; }
        public float Amount { get; set; }

    }

    public class ExpenseForecast
    {
        public float[] ForecastedExpenses { get; set; }
    }
}
