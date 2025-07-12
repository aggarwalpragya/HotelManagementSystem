using APIHotelManagement.DTOs;

namespace APIHotelManagement.Interfaces
{
    public interface IGuest
    {
        Task<IEnumerable<GuestVM>> GetAllGuests();
        Task<GuestVM> GetGuestById(int guestId);
        Task<bool> AddGuest(GuestVM guestVM);
        Task<bool> UpdateGuest(int guestId, GuestVM guestVM);
        Task<bool> DeleteGuest(int guestId); // Soft delete
        Task<bool> RemoveGuest(int guestId); // Hard delete
    }
}
