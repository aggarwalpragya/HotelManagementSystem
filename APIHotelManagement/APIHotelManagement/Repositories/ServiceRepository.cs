using APIHotelManagement.DTOs;
using APIHotelManagement.Interfaces;
using APIHotelManagement.Models;

namespace APIHotelManagement.Repositories
{
    public class ServiceRepository : IService
    {
        private readonly HotelManagementDbContext _context;

        public ServiceRepository(HotelManagementDbContext context)
        {
            _context = context;
        }

        // Get All Services
        public async Task<IEnumerable<ServiceVM>> GetAllServices()
        {
            return _context.Services
                .Select(s => new ServiceVM
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.ServiceName,
                    Price = s.Price,
                    ServiceDescription = s.ServiceDescription,
                    AvailabilityStatus = s.AvailabilityStatus,
                    Category = s.Category,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToList();
        }

        // Add New Service
        public async Task<bool> AddService(ServiceVM serviceVM)
        {
            var service = new Service
            {
                ServiceName = serviceVM.ServiceName,
                Price = serviceVM.Price,
                ServiceDescription = serviceVM.ServiceDescription,
                AvailabilityStatus = true,  // Default to available
                Category = serviceVM.Category,
                CreatedAt = DateTime.UtcNow
            };

            _context.Services.Add(service);
            return _context.SaveChanges() > 0;
        }

        // Update Service
        public async Task<bool> UpdateService(int serviceId, ServiceVM serviceVM)
        {
            var service = _context.Services.Find(serviceId);
            if (service == null)
                throw new Exception("Service not found");

            service.ServiceName = serviceVM.ServiceName;
            service.Price = serviceVM.Price;
            service.ServiceDescription = serviceVM.ServiceDescription;
            service.AvailabilityStatus = serviceVM.AvailabilityStatus;
            service.Category = serviceVM.Category;
            service.UpdatedAt = DateTime.UtcNow;

            return _context.SaveChanges() > 0;
        }

        // Soft Delete (Disable Service)
        public async Task<bool> DeleteService(int serviceId)
        {
            var service = _context.Services.Find(serviceId);
            if (service == null)
                throw new Exception("Service not found");

            service.AvailabilityStatus = false; // Mark as unavailable
            service.UpdatedAt = DateTime.UtcNow;

            return _context.SaveChanges() > 0;
        }

        // Hard Delete (Remove Service Permanently)
        public async Task<bool> RemoveService(int serviceId)
        {
            var service = _context.Services.Find(serviceId);
            if (service == null)
                throw new Exception("Service not found");

            _context.Services.Remove(service);
            return _context.SaveChanges() > 0;
        }
    }
}
