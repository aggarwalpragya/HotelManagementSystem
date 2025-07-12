using APIHotelManagement.DTOs;
using APIHotelManagement.Enums;
using APIHotelManagement.Interfaces;
using APIHotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace APIHotelManagement.Repositories
{
    public class ReservationRepository : IReservation
    {
        private readonly HotelManagementDbContext _context;

        public ReservationRepository(HotelManagementDbContext context)
        {
            _context = context;
        }

        public async Task<List<MakeReservationVM>> GetAllReservations()
        {
            var reservations = _context.Reservations
                                 .Include(r => r.Room)
                                     .ThenInclude(room => room.RoomType)
                                 .Include(r => r.Guest)
                                 .Include(r => r.ReservationServices) 
                                     .ThenInclude(rs => rs.Service)   
                                 .ToList();

            var reservationVMs = reservations.Select(r => new MakeReservationVM
            {
                ReservationId = r.ReservationId,
                RoomTypeName = r.Room.RoomType.RoomTypeName,
                NumberOfGuest = r.NumberOfGuest,
                CheckIn = r.CheckIn,
                CheckOut = r.CheckOut,
                GuestId = r.GuestId,
                UserId = r.UserId,
                RoomId = r.RoomId,
                ReservationStatus = r.ReservationStatus.ToString(),
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,

                // Guest Table Data
                GuestName = r.Guest.GuestName,
                IdProofType = r.Guest.IdProofType.ToString(),
                IdProofNumber = r.Guest.IdProofNumber,
                ContactNumber = r.Guest.ContactNumber,

                // Reservation Services Data
                Services = r.ReservationServices.Select(rs => new ReservationServiceVM
                {
                    ReservationId = rs.ReservationId,
                    ServiceId = rs.ServiceId,
                    ServiceName = rs.Service.ServiceName,
                    Quantity = rs.Quantity
                }).ToList()

            }).ToList();

            return reservationVMs;
        }


        public async Task<bool> MakeReservation(MakeReservationVM makeReservationVM)
        {
            // Find RoomType by Name
            var roomType = _context.RoomTypes
                .FirstOrDefault(rt => rt.RoomTypeName == makeReservationVM.RoomTypeName);

            if (roomType == null)
                throw new Exception("Room Type not found.");

            // Find an available room
            var availableRoom = _context.Rooms
                .FirstOrDefault(r => r.RoomTypeId == roomType.RoomTypeId && r.RoomStatus == RoomStatus.Available);

            if (availableRoom == null)
                throw new Exception("No available rooms of this type.");

            // Convert IdProofType and ReservationStatus safely
            if (!Enum.TryParse<IdProofType>(makeReservationVM.IdProofType, true, out var idProofEnum))
                throw new Exception("Invalid IdProofType provided.");

            if (!Enum.TryParse<ReservationStatus>(makeReservationVM.ReservationStatus, true, out var reservationStatusEnum))
                throw new Exception("Invalid IdProofType provided.");


            // Check if the guest exists
            var guest = _context.Guests.Find(makeReservationVM.GuestId);
            if (guest == null)
            {
                // Create new guest
                guest = new Guest
                {
                    GuestName = makeReservationVM.GuestName,
                    IdProofType = idProofEnum,
                    IdProofNumber = makeReservationVM.IdProofNumber,
                    ContactNumber = makeReservationVM.ContactNumber,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Guests.Add(guest);
                _context.SaveChanges(); // Save to get generated GuestId
            }

            // Create a new Reservation
            var reservation = new Reservation
            {
                RoomId = availableRoom.RoomId,
                GuestId = guest.GuestId,
                UserId = makeReservationVM.UserId,
                NumberOfGuest = makeReservationVM.NumberOfGuest,
                ReservationStatus = reservationStatusEnum,
                CheckIn = makeReservationVM.CheckIn,
                CheckOut = makeReservationVM.CheckOut,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reservations.Add(reservation);
            _context.SaveChanges(); // Save once to generate ReservationId for linking services

            // Handle Reservation Services if provided
            if (makeReservationVM.Services != null && makeReservationVM.Services.Any())
            {
                foreach (var serviceVM in makeReservationVM.Services)
                {
                    // Validate if service exists
                    var service = _context.Services.FirstOrDefault(s => s.ServiceId == serviceVM.ServiceId);
                    if (service == null)
                        throw new Exception($"Service with ID {serviceVM.ServiceId} not found.");

                    var reservationService = new ReservationService
                    {
                        ReservationId = reservation.ReservationId,
                        ServiceId = serviceVM.ServiceId,
                        Quantity = serviceVM.Quantity
                    };
                    _context.ReservationServices.Add(reservationService);
                }
            }

            // Mark Room as Booked
            availableRoom.RoomStatus = RoomStatus.Occupied;
            availableRoom.UpdatedAt = DateTime.UtcNow;

            // Final Save
            return _context.SaveChanges() > 0;
        }


        public async Task<bool> UpdateReservation(int reservationId, MakeReservationVM reservationVM)
        {
            var reservation = _context.Reservations.Find(reservationId);
            if (reservation == null)
                throw new Exception("Reservation not found.");

            // Update Room Type if it's changed
            if (!string.IsNullOrEmpty(reservationVM.RoomTypeName))
            {
                var roomType = _context.RoomTypes.FirstOrDefault(rt => rt.RoomTypeName == reservationVM.RoomTypeName);
                if (roomType == null)
                    throw new Exception("Room Type not found.");

                var newAvailableRoom = _context.Rooms.FirstOrDefault(r => r.RoomTypeId == roomType.RoomTypeId && r.RoomStatus == RoomStatus.Available);
                if (newAvailableRoom == null)
                    throw new Exception("No available rooms of this type.");

                // Release old room
                var oldRoom = _context.Rooms.Find(reservation.RoomId);
                if (oldRoom != null)
                {
                    oldRoom.RoomStatus = RoomStatus.Available;
                    oldRoom.UpdatedAt = DateTime.UtcNow;
                }

                // Assign new room
                reservation.RoomId = newAvailableRoom.RoomId;
                newAvailableRoom.RoomStatus = RoomStatus.Occupied;
                newAvailableRoom.UpdatedAt = DateTime.UtcNow;
            }

            // Update reservation details
            reservation.NumberOfGuest = reservationVM.NumberOfGuest;
            reservation.ReservationStatus = Enum.Parse<ReservationStatus>(reservationVM.ReservationStatus, true);
            reservation.CheckIn = reservationVM.CheckIn;
            reservation.CheckOut = reservationVM.CheckOut;
            reservation.UpdatedAt = DateTime.UtcNow;

            // Update associated services
            if (reservationVM.Services != null && reservationVM.Services.Any())
            {
                // Remove existing services associated with the reservation
                var existingServices = _context.ReservationServices.Where(rs => rs.ReservationId == reservationId).ToList();
                _context.ReservationServices.RemoveRange(existingServices);

                // Add the new services from the input
                foreach (var service in reservationVM.Services)
                {
                    var newService = new ReservationService
                    {
                        ReservationId = reservationId,
                        ServiceId = service.ServiceId,
                        Quantity = service.Quantity
                    };
                    _context.ReservationServices.Add(newService);
                }
            }

            // Save all changes to the database
            return _context.SaveChanges() > 0;
        }


        // Soft Delete (Cancel Reservation)
        public async Task<bool> DeleteReservation(int reservationId)
        {
            var reservation = _context.Reservations.Find(reservationId);
            if (reservation == null)
                throw new Exception("Reservation not found.");

            // update room status
            var room = _context.Rooms.Find(reservation.RoomId);
            if (room != null)
            {
                room.RoomStatus = RoomStatus.Available;
                room.UpdatedAt = DateTime.UtcNow;
            }
            // delete guest (soft delete)
            var guest = _context.Guests.Find(reservation.GuestId);
            guest.IsActive = false;

            reservation.ReservationStatus = ReservationStatus.Cancelled;
            reservation.CheckOut = DateTime.UtcNow;  // Mark as checked out
            reservation.UpdatedAt = DateTime.UtcNow;

            return _context.SaveChanges() > 0;
        }

        // Hard Delete (Remove from DB)
        public async Task<bool> RemoveReservation(int reservationId)
        {
            var reservation = _context.Reservations.Find(reservationId);
            if (reservation == null)
                throw new Exception("Reservation not found.");

            // update room status
            var room = _context.Rooms.Find(reservation.RoomId);
            if (room != null)
            {
                room.RoomStatus = RoomStatus.Available;
                room.UpdatedAt = DateTime.UtcNow;
            }

            // Remove Guest (hard delete)
            var guest = _context.Guests.Find(reservation.GuestId);
            _context.Guests.Remove(guest);

            _context.Reservations.Remove(reservation);

            return _context.SaveChanges() > 0;
        }
    }
}
