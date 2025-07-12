using APIHotelManagement.DTOs;
using APIHotelManagement.Enums;
using APIHotelManagement.Interfaces;
using APIHotelManagement.Models;

namespace APIHotelManagement.Repositories
{
    public class GuestRepository : IGuest
    {
        private readonly HotelManagementDbContext _context;

        public GuestRepository(HotelManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GuestVM>> GetAllGuests()
        {
            return _context.Guests
                .Select(g => new GuestVM
                {
                    GuestId = g.GuestId,
                    GuestName = g.GuestName,
                    IdProofType = g.IdProofType.ToString(),
                    IdProofNumber = g.IdProofNumber,
                    ContactNumber = g.ContactNumber,
                    IsActive = g.IsActive,
                    CreatedAt = g.CreatedAt,
                    UpdatedAt = g.UpdatedAt
                })
                .ToList();
        }

        public async Task<GuestVM> GetGuestById(int guestId)
        {
            var guest = _context.Guests.Find(guestId);
            if (guest == null)
                throw new Exception("Guest not found");

            return new GuestVM
            {
                GuestId = guest.GuestId,
                GuestName = guest.GuestName,
                IdProofType = guest.IdProofType.ToString(),
                IdProofNumber = guest.IdProofNumber,
                ContactNumber = guest.ContactNumber,
                IsActive = guest.IsActive,
                CreatedAt = guest.CreatedAt,
                UpdatedAt = guest.UpdatedAt
            };
        }

        public async Task<bool> AddGuest(GuestVM guestVM)
        {
            var guest = new Guest
            {
                GuestName = guestVM.GuestName,
                IdProofType = Enum.Parse<IdProofType>(guestVM.IdProofType, true),
                IdProofNumber = guestVM.IdProofNumber,
                ContactNumber = guestVM.ContactNumber,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Guests.Add(guest);
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> UpdateGuest(int guestId, GuestVM guestVM)
        {
            var guest = _context.Guests.Find(guestId);
            if (guest == null)
                throw new Exception("Guest not found");

            guest.GuestName = guestVM.GuestName;
            guest.IdProofType = Enum.Parse<IdProofType>(guestVM.IdProofType, true);
            guest.IdProofNumber = guestVM.IdProofNumber;
            guest.ContactNumber = guestVM.ContactNumber;
            guest.UpdatedAt = DateTime.UtcNow;

            return _context.SaveChanges() > 0;
        }

        public async Task<bool> DeleteGuest(int guestId)
        {
            var guest = _context.Guests.Find(guestId);
            if (guest == null)
                throw new Exception("Guest not found");

            guest.IsActive = false; // Soft delete
            guest.UpdatedAt = DateTime.UtcNow;

            return _context.SaveChanges() > 0;
        }

        public async Task<bool> RemoveGuest(int guestId)
        {
            var guest = _context.Guests.Find(guestId);
            if (guest == null)
                throw new Exception("Guest not found");

            _context.Guests.Remove(guest);  // Hard delete
            return _context.SaveChanges() > 0;
        }
    }
}
