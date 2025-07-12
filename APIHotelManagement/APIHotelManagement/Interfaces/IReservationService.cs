using APIHotelManagement.DTOs;

namespace APIHotelManagement.Interfaces
{
    public interface IReservationService
    {
        Task<IEnumerable<ReservationServiceVM>> GetAllReservationServices();
    }
}
