using APIHotelManagement.DTOs;
using APIHotelManagement.Models;

namespace APIHotelManagement.Interfaces
{
    public interface IReservation
    {
        Task<List<MakeReservationVM>> GetAllReservations();
        Task<bool> MakeReservation(MakeReservationVM makeReservationVM);
        Task<bool> UpdateReservation(int reservationId, MakeReservationVM reservationVM);
        Task<bool> DeleteReservation(int reservationId);
        Task<bool> RemoveReservation(int reservationId);
    }
}
