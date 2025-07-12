using APIHotelManagement.DTOs;
using APIHotelManagement.Enums;
using APIHotelManagement.Models;

namespace APIHotelManagement.Interfaces
{
    public interface IBill
    {
        Task<BillDetailVM> IssueBill(int reservationId);
        Task<IEnumerable<BillVM>> GetAllBills();
        Task<IEnumerable<BillVM>> GetBillsByPaymentStatus(PaymentStatus status);
        Task<BillVM> GetBillById(int billId);
        Task<bool> UpdateBill(int billId, BillVM billVM);
        Task<bool> DeleteBill(int billId);
    }
}
