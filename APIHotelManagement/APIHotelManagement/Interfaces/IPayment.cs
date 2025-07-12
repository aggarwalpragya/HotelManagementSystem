namespace APIHotelManagement.Interfaces
{
    public interface IPayment
    {
        Task<bool> UpdatePaymentAmount(int paymentId, decimal amount);
        Task<bool> UpdatePaymentStatus(int paymentId, string newStatus);
        Task<bool> DeletePayment(int paymentId);
    }
}
