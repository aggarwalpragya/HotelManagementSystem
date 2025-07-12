using APIHotelManagement.DTOs;
using APIHotelManagement.Enums;
using APIHotelManagement.Models;

namespace APIHotelManagement.Interfaces
{
    public interface IUser
    {
        Task<IEnumerable<UserVM>> GetAllUsers();
        Task<UserVM> GetUserById(int userId);
        Task<IEnumerable<UserVM>> GetUsersByRole(UserRole role);
        Task<bool> AddUser(UserVM userVM);
        Task<bool> UpdateUser(int userId, UserVM userVM);
        Task<bool> DeleteUser(int userId);
        Task<bool> RemoveUser(int userId);
    }
}
