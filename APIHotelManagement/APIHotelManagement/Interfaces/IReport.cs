using APIHotelManagement.DTOs;

namespace APIHotelManagement.Interfaces
{
    public interface IReport
    {
        Task<FinancialReportVM> GetMonthlyReport(int year, int month);
        Task<FinancialReportVM> GetYearlyReport(int year);
    }
}
