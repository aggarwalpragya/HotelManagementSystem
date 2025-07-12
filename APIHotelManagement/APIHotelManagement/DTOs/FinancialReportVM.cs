namespace APIHotelManagement.DTOs
{
    public class FinancialReportVM
    {
        // Report Type
        public string ReportType { get; set; }  // "Monthly" or "Yearly"
        public int Year { get; set; }
        public int? Month { get; set; } // Null for yearly reports

        // Income
        public decimal TotalRoomRevenue { get; set; }
        public decimal TotalServiceRevenue { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal PendingPayments { get; set; }

        // Staff Expenses
        public decimal TotalStaffSalaries { get; set; }

        // Profit/Loss Analysis
        public decimal NetProfit { get; set; }
        public decimal ProfitPercentage { get; set; }
        public string FinancialStatus { get; set; } // "Profit" or "Loss"

        public DateTime ReportGeneratedAt { get; set; } = DateTime.UtcNow;
    }
}
