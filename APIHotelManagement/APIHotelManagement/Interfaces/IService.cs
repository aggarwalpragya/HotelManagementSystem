using APIHotelManagement.DTOs;
using System.Threading.Tasks;

namespace APIHotelManagement.Interfaces
{
    public interface IService
    {
        Task<IEnumerable<ServiceVM>> GetAllServices();
        Task<bool> AddService(ServiceVM serviceVM);
        Task<bool> UpdateService(int serviceId, ServiceVM serviceVM);
        Task<bool> DeleteService(int serviceId);  // Soft Delete (Disable Service)
        Task<bool> RemoveService(int serviceId);  // Hard Delete (Permanently Remove)
    }
}
