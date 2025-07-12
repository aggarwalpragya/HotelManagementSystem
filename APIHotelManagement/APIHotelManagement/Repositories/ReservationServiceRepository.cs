using APIHotelManagement.DTOs;
using APIHotelManagement.Interfaces;
using APIHotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace APIHotelManagement.Repositories
{
    public class ReservationServiceRepository : IReservationService
    {
        private readonly HotelManagementDbContext _context;

        public ReservationServiceRepository(HotelManagementDbContext context)
        {
            _context = context;
        }

        // Get All Reservation Services
        public async Task<IEnumerable<ReservationServiceVM>> GetAllReservationServices()
        {
            return _context.ReservationServices
                .Include(rs => rs.Service)
                .Select(rs => new ReservationServiceVM
                {
                    ReservationId = rs.ReservationId,
                    ServiceId = rs.ServiceId,
                    ServiceName = rs.Service.ServiceName,
                    Quantity = rs.Quantity
                })
                .ToList();
        }
    }
}
