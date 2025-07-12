using APIHotelManagement.DTOs;
using APIHotelManagement.Interfaces;
using APIHotelManagement.Models;

namespace APIHotelManagement.Repositories
{
    public class RoomTypeRepository : IRoomType
    {
        private readonly HotelManagementDbContext _context;

        public RoomTypeRepository(HotelManagementDbContext context)
        {
            _context = context;
        }

        // Get All Room Types
        public async Task<IEnumerable<RoomType>> GetAllRoomTypes()
        {
            return _context.RoomTypes
                .Select(rt => new RoomType
                {
                    RoomTypeId = rt.RoomTypeId,
                    RoomTypeName = rt.RoomTypeName,
                    Capacity = rt.Capacity,
                    RoomDescription = rt.RoomDescription,
                    RoomRate = rt.RoomRate,
                    CreatedAt = rt.CreatedAt,
                    UpdatedAt = rt.UpdatedAt,
                    IsActive = rt.IsActive
                })
                .ToList();
        }

        // Set Room Rate (Update Rate)
        public async Task<bool> SetRoomRate(int roomTypeId, decimal newRate)
        {
            var roomType = _context.RoomTypes.Find(roomTypeId);
            if (roomType == null)
                throw new Exception("Room Type not found.");

            roomType.RoomRate = newRate;
            roomType.UpdatedAt = DateTime.UtcNow;

            return _context.SaveChanges() > 0;
        }
    }
}
