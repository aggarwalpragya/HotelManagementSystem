using APIHotelManagement.DTOs;
using APIHotelManagement.Enums;
using APIHotelManagement.Interfaces;
using APIHotelManagement.Models;
using BCrypt.Net;

namespace APIHotelManagement.Repositories
{
    public class UserRepository : IUser
    {
        private readonly HotelManagementDbContext _context;

        public UserRepository(HotelManagementDbContext context)
        {
            _context = context;
        }

        // Get all users
        public async Task<IEnumerable<UserVM>> GetAllUsers()
        {
            return _context.Users
                .Select(u => new UserVM
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    UserRole = u.UserRole.ToString(),
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToList();
        }

        // Get user by ID
        public async Task<UserVM> GetUserById(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null) return null;

            return new UserVM
            {
                UserId = user.UserId,
                UserName = user.UserName,
                UserRole = user.UserRole.ToString(),
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        // Get users by role
        public async Task<IEnumerable<UserVM>> GetUsersByRole(UserRole role)
        {
            return _context.Users
                .Where(u => u.UserRole == role)
                .Select(u => new UserVM
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    UserRole = u.UserRole.ToString(),
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToList();
        }

        // Add a new user
        public async Task<bool> AddUser(UserVM userVM)
        {
            var user = new User
            {
                UserName = userVM.UserName,
                Password = userVM.Password,  // Hash password in production
                UserRole = Enum.Parse<UserRole>(userVM.UserRole, true),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            return _context.SaveChanges() > 0;
        }

        // Update user details
        public async Task<bool> UpdateUser(int userId, UserVM userVM)
        {
            var user = _context.Users.Find(userId);
            if (user == null) return false;

            user.UserName = userVM.UserName;
            user.Password = BCrypt.Net.BCrypt.HashPassword(userVM.Password);
            user.UserRole = Enum.Parse<UserRole>(userVM.UserRole, true);
            user.IsActive = userVM.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            return _context.SaveChanges() > 0;
        }

        // Delete user (soft delete: mark as inactive)
        public async Task<bool> DeleteUser(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null) return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            return _context.SaveChanges() > 0;
        }

        // Remove user (hard delete)
        public async Task<bool> RemoveUser(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null) return false;

            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
