using APIHotelManagement.Enums;
using APIHotelManagement.Interfaces;
using APIHotelManagement.Models;

namespace APIHotelManagement.Repositories
{
    public class PaymentRepository : IPayment
    {
        private readonly HotelManagementDbContext _context;

        public PaymentRepository(HotelManagementDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UpdatePaymentAmount(int paymentId, decimal amount)
        {
            var payment = _context.Payments.Find(paymentId);
            if (payment == null)
                throw new Exception("Payment not found.");

            payment.PaymentAmount = amount;
            payment.PaymentDate = DateTime.UtcNow; // Update payment date

            return _context.SaveChanges() > 0;
        }

        // Update Payment Status
        public async Task<bool> UpdatePaymentStatus(int paymentId, string newStatus)
        {
            var payment = _context.Payments.Find(paymentId);
            if (payment == null)
                throw new Exception("Payment not found.");

            // Convert string to enum safely
            if (!Enum.TryParse<PaymentStatus>(newStatus, true, out var statusEnum))
                throw new Exception("Invalid payment status.");

            payment.PaymentStatus = statusEnum;
            payment.PaymentDate = DateTime.UtcNow; // Update payment date

            return _context.SaveChanges() > 0;
        }

        // Delete Payment Permanently

        public async Task<bool> DeletePayment(int paymentId)
        {
            var payment = _context.Payments.Find(paymentId);
            if (payment == null)
                throw new Exception("Payment not found.");

            _context.Payments.Remove(payment);
            return _context.SaveChanges() > 0;
        }
    }
}
