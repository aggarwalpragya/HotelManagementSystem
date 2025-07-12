using APIHotelManagement.DTOs;
using APIHotelManagement.Models;

namespace APIHotelManagement.Interfaces
{
    public interface IDepartment
    {
        Task<IEnumerable<DepartmentVM>> GetAllDepartments();
        Task<DepartmentVM> GetDepartmentById(int departmentId);
        Task<IEnumerable<DepartmentVM>> GetDepartmentByName(string departmentName);
        Task<bool> AddDepartment(DepartmentVM departmentVM);
        Task<bool> UpdateDepartment(int departmentId, DepartmentVM departmentVM);
        Task<bool> DeleteDepartment(int departmentId); // Soft Delete
        Task<bool> RemoveDepartment(int departmentId); // Hard Delete
    }
}
