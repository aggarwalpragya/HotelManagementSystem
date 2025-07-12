using APIHotelManagement.DTOs;
using APIHotelManagement.Enums;
using APIHotelManagement.Interfaces;
using APIHotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace APIHotelManagement.Repositories
{
    public class StaffRepository : IStaff
    {
        private readonly HotelManagementDbContext _context;

        public StaffRepository(HotelManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StaffVM>> GetAllStaff()
        {
            var staffList = await _context.Staff
                .Include(s => s.Department)    
                .Include(s => s.Service)       
                .Select(s => new StaffVM
                {
                    StaffId = s.StaffId,
                    StaffName = s.StaffName,
                    Gender = s.Gender.ToString(),
                    DepartmentName = s.Department.DepartmentName,
                    ContactNumber = s.ContactNumber,
                    Email = s.Email,
                    HireDate = s.HireDate,
                    Salary = s.Salary,
                    WorkStatus = s.WorkStatus.ToString(),
                    ServiceName = s.Service.ServiceName
                })
                .ToListAsync();

            return staffList;
        }

        public async Task<StaffVM> GetStaffById(int staffId)
        {
            var staff = _context.Staff.Find(staffId);

            if (staff == null)
            {
                throw new KeyNotFoundException("Staff not found");
            }

            return new StaffVM
            {
                StaffId = staff.StaffId,
                StaffName = staff.StaffName,
                Gender = staff.Gender.ToString(),
                DepartmentId = staff.DepartmentId,
                ContactNumber = staff.ContactNumber,
                Email = staff.Email,
                HireDate = staff.HireDate,
                Salary = staff.Salary,
                WorkStatus = staff.WorkStatus.ToString(),
                ServiceId = staff.ServiceId,
                UserId = staff.UserId,
                CreatedAt = staff.CreatedAt,
                UpdatedAt = staff.UpdatedAt
            };
        }

        public async Task<IEnumerable<StaffVM>> GetStaffByName(string staffName)
        {
            var staff = _context.Staff.FirstOrDefault(s => s.StaffName == staffName);

            if (staff == null)
            {
                throw new KeyNotFoundException("Staff not found");
            }

            var staffList = _context.Staff
                .Where(s => s.StaffName == staffName)
                .Select(s => new StaffVM
                {
                    StaffId = s.StaffId,
                    StaffName = s.StaffName,
                    Gender = s.Gender.ToString(),
                    DepartmentId = s.DepartmentId,
                    ContactNumber = s.ContactNumber,
                    Email = s.Email,
                    HireDate = s.HireDate,
                    Salary = s.Salary,
                    WorkStatus = s.WorkStatus.ToString(),
                    ServiceId = s.ServiceId,
                    UserId = s.UserId,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToList();

            return staffList;
        }

        public async Task<bool> AddStaff(StaffVM staffVM)
        {
            var staff = new Staff
            {
                StaffName = staffVM.StaffName,
                Gender = Enum.Parse<Gender>(staffVM.Gender, true),
                DepartmentId = staffVM.DepartmentId,
                ContactNumber = staffVM.ContactNumber,
                Email = staffVM.Email,
                HireDate = staffVM.HireDate,
                Salary = staffVM.Salary,
                WorkStatus = Enum.Parse<WorkStatus>(staffVM.WorkStatus, true),
                ServiceId = staffVM.ServiceId,
                UserId = staffVM.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Staff.Add(staff);
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> UpdateStaff(StaffVM staffVM)
        {
            var staff = _context.Staff.Find(staffVM.StaffId);

            if (staff == null)
            {
                throw new KeyNotFoundException("Staff not found");
            }

            staff.StaffName = staffVM.StaffName;
            staff.Gender = Enum.Parse<Gender>(staffVM.Gender, true);
            staff.DepartmentId = staffVM.DepartmentId;
            staff.ContactNumber = staffVM.ContactNumber;
            staff.Email = staffVM.Email;
            staff.HireDate = staffVM.HireDate;
            staff.Salary = staffVM.Salary;
            staff.WorkStatus = Enum.Parse<WorkStatus>(staffVM.WorkStatus, true);
            staff.ServiceId = staffVM.ServiceId;
            staff.UserId = staffVM.UserId;
            staff.UpdatedAt = DateTime.UtcNow;

            return _context.SaveChanges() > 0;
        }

        public async Task<bool> DeleteStaff(int staffId)
        {
            var staff = _context.Staff.Find(staffId);
            if (staff == null)
            {
                throw new KeyNotFoundException("Staff not found");
            }

            staff.WorkStatus = WorkStatus.Terminated;  // Setting WorkStatus instead of deleting
            staff.UpdatedAt = DateTime.UtcNow;

            return _context.SaveChanges() > 0;
        }

        public async Task<bool> RemoveStaff(int staffId)
        {
            var staff = _context.Staff.Find(staffId);
            if (staff == null) return false;

            _context.Staff.Remove(staff);
            return _context.SaveChanges() > 0;
        }
    }
}
