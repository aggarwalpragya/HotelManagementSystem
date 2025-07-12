using APIHotelManagement.DTOs;
using APIHotelManagement.Enums;
using APIHotelManagement.Interfaces;
using APIHotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace APIHotelManagement.Repositories
{
    public class RoomRepository : IRoom
    {
        private readonly HotelManagementDbContext _context;

        public RoomRepository(HotelManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoomVM>> GetAllRooms()
        {
            return _context.Rooms
                .Include(r => r.RoomType) // Ensure RoomType is loaded
                .Select(r => new RoomVM
                {
                    RoomId = r.RoomId,
                    RoomNumber = r.RoomNumber,
                    RoomStatus = r.RoomStatus.ToString(),
                    RoomType = r.RoomType.RoomTypeName,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .ToList();
        }

        public async Task<IEnumerable<RoomVM>> GetRoomsByStatus(string roomStatus)
        {
            return _context.Rooms
                .Where(r => r.RoomStatus.ToString() == roomStatus)
                .Select(r => new RoomVM
                {
                    RoomId = r.RoomId,
                    RoomNumber = r.RoomNumber,
                    RoomStatus = r.RoomStatus.ToString(),
                    RoomTypeId = r.RoomTypeId,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .ToList();
        }

        public async Task<RoomVM> GetRoomById(int roomId)
        {
            var room = _context.Rooms.Find(roomId);
            if (room == null)
                throw new Exception("Room not found");

            return new RoomVM
            {
                RoomId = room.RoomId,
                RoomNumber = room.RoomNumber,
                RoomStatus = room.RoomStatus.ToString(),
                RoomTypeId = room.RoomTypeId,
                CreatedAt = room.CreatedAt,
                UpdatedAt = room.UpdatedAt
            };
        }

        public async Task<bool> AddRoom(RoomVM roomVM)
        {
            var room = new Room
            {
                RoomNumber = roomVM.RoomNumber,
                RoomStatus = Enum.Parse<RoomStatus>(roomVM.RoomStatus, true),
                RoomTypeId = roomVM.RoomTypeId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Rooms.Add(room);
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> UpdateRoom(int roomId, RoomVM roomVM)
        {
            var room = _context.Rooms.Find(roomId);
            if (room == null)
                throw new Exception("Room not found");

            room.RoomNumber = roomVM.RoomNumber;
            room.RoomStatus = Enum.Parse<RoomStatus>(roomVM.RoomStatus, true);
            room.RoomTypeId = roomVM.RoomTypeId;
            room.UpdatedAt = DateTime.UtcNow;

            return _context.SaveChanges() > 0;
        }

        public async Task<bool> DeleteRoom(int roomId)
        {
            var room = _context.Rooms.Find(roomId);
            if (room == null)
                throw new Exception("Room not found");

            room.RoomStatus = RoomStatus.Maintenance; // Soft delete by changing status
            room.UpdatedAt = DateTime.UtcNow;

            return _context.SaveChanges() > 0;
        }

        public async Task<bool> RemoveRoom(int roomId)
        {
            var room = _context.Rooms.Find(roomId);
            if (room == null)
                throw new Exception("Room not found");

            _context.Rooms.Remove(room);

            return _context.SaveChanges() > 0;
        }

        public async Task<IEnumerable<RoomVM>> SearchAvailableRooms(string roomTypeName, DateTime checkIn, DateTime checkOut)
        {
            var availableRooms = _context.Rooms
            .Where(r => r.RoomType.RoomTypeName == roomTypeName &&
                        !r.Reservations.Any(res =>
                            (checkIn < res.CheckOut && checkOut > res.CheckIn)
                        )
            )
            .Select(r => new RoomVM
            {
                RoomId = r.RoomId,
                RoomNumber = r.RoomNumber,
                RoomStatus = r.RoomStatus.ToString(),  // Convert enum to string
                RoomTypeId = r.RoomTypeId,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .ToList();

            return availableRooms;
        }
    }
}
