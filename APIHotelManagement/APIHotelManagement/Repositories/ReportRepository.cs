using APIHotelManagement.DTOs;
using APIHotelManagement.Enums;
using APIHotelManagement.Interfaces;
using APIHotelManagement.Models;

namespace APIHotelManagement.Repositories
{
    public class ReportRepository : IReport
    {
        private readonly HotelManagementDbContext _context;

        public ReportRepository(HotelManagementDbContext context)
        {
            _context = context;
        }

        // 🔹 GET Monthly Financial Report
        public async Task<FinancialReportVM> GetMonthlyReport(int year, int month)
        {
            return await GenerateReport("Monthly", year, month);
        }

        // 🔹 GET Yearly Financial Report
        public async Task<FinancialReportVM> GetYearlyReport(int year)
        {
            return await GenerateReport("Yearly", year, null);
        }

        // 🔹 Common Report Generation Function
        private async Task<FinancialReportVM> GenerateReport(string reportType, int year, int? month)
        {
            var startDate = new DateTime(year, month ?? 1, 1);
            var endDate = month.HasValue ? startDate.AddMonths(1).AddDays(-1) : startDate.AddYears(1).AddDays(-1);

            // 🔸 Total Room Revenue
            var totalRoomRevenue = _context.Bills
                .Where(b => b.BillingDate >= startDate && b.BillingDate <= endDate)
                .Sum(b => b.TotalAmount);

            // 🔸 Total Service Revenue
            var totalServiceRevenue = _context.ReservationServices
                .Where(rs => rs.Reservation.CreatedAt >= startDate && rs.Reservation.CreatedAt <= endDate)
                .Join(_context.Services, rs => rs.ServiceId, s => s.ServiceId, (rs, s) => rs.Quantity * s.Price)
                .Sum();

            // 🔸 Total Income
            var totalIncome = totalRoomRevenue + totalServiceRevenue;

            // 🔸 Pending Payments
            var pendingPayments = _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate && p.PaymentStatus == PaymentStatus.Pending)
                .Sum(p => p.PaymentAmount ?? 0);

            // 🔸 Total Staff Salaries
            var totalStaffSalaries = _context.Staff.Sum(s => s.Salary);

            // 🔸 Calculate Net Profit
            var netProfit = totalIncome - totalStaffSalaries;

            // 🔸 Calculate Profit/Loss Percentage
            decimal profitPercentage = totalIncome > 0 ? (netProfit / totalIncome) * 100 : 0;

            // 🔸 Determine Financial Status
            string financialStatus = netProfit >= 0 ? "Profit" : "Loss";

            return new FinancialReportVM
            {
                ReportType = reportType,
                Year = year,
                Month = month,

                // Income
                TotalRoomRevenue = totalRoomRevenue,
                TotalServiceRevenue = totalServiceRevenue,
                TotalIncome = totalIncome,
                PendingPayments = pendingPayments,

                // Staff Expenses
                TotalStaffSalaries = totalStaffSalaries,

                // Profit Analysis
                NetProfit = netProfit,
                ProfitPercentage = profitPercentage,
                FinancialStatus = financialStatus
            };
        }
    }
}
