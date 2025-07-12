using APIHotelManagement.Models;

namespace APIHotelManagement.Interfaces
{
    public interface IRoomType
    {
        Task<IEnumerable<RoomType>> GetAllRoomTypes();
        Task<bool> SetRoomRate(int roomTypeId, decimal newRate);
    }
}
