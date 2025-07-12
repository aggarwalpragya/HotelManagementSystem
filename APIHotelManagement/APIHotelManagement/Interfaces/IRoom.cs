using APIHotelManagement.DTOs;
using APIHotelManagement.Models;

namespace APIHotelManagement.Interfaces
{
    public interface IRoom
    {
        Task<IEnumerable<RoomVM>> GetAllRooms();
        Task<IEnumerable<RoomVM>> GetRoomsByStatus(string roomStatus);
        Task<RoomVM> GetRoomById(int roomId);
        Task<bool> AddRoom(RoomVM roomVM);
        Task<bool> UpdateRoom(int roomId, RoomVM roomVM);
        Task<bool> DeleteRoom(int roomId);
        Task<bool> RemoveRoom(int roomId);
        Task<IEnumerable<RoomVM>> SearchAvailableRooms(string roomTypeName, DateTime checkIn, DateTime checkOut);
    }
}
