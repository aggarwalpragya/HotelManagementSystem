using APIHotelManagement.DTOs;
using APIHotelManagement.Interfaces;
using APIHotelManagement.Models;

namespace APIHotelManagement.Repositories
{
    public class DepartmentRepository : IDepartment
    {
        private readonly HotelManagementDbContext _context;

        public DepartmentRepository(HotelManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DepartmentVM>> GetAllDepartments()
        {
            return _context.Departments
                .Select(d => new DepartmentVM
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName,
                    DepartmentDescription = d.DepartmentDescription,
                    IsActive = d.IsActive,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                })
                .ToList();
        }

        public async Task<DepartmentVM> GetDepartmentById(int departmentId)
        {
            var department = _context.Departments.Find(departmentId);
            if (department == null)
                throw new Exception("Department not found");

            return new DepartmentVM
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.DepartmentName,
                DepartmentDescription = department.DepartmentDescription,
                IsActive = department.IsActive,
                CreatedAt = department.CreatedAt,
                UpdatedAt = department.UpdatedAt
            };
        }

        public async Task<IEnumerable<DepartmentVM>> GetDepartmentByName(string departmentName)
        {
            var department = _context.Departments.FirstOrDefault(d => d.DepartmentName == departmentName);
            if (department == null)
                throw new Exception("Department not found");

            return _context.Departments
                .Where(d => d.DepartmentName.Contains(departmentName))
                .Select(d => new DepartmentVM
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName,
                    DepartmentDescription = d.DepartmentDescription,
                    IsActive = d.IsActive,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                })
                .ToList();
        }

        public async Task<bool> AddDepartment(DepartmentVM departmentVM)
        {
            var department = new Department
            {
                DepartmentName = departmentVM.DepartmentName,
                DepartmentDescription = departmentVM.DepartmentDescription,
                IsActive = departmentVM.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Departments.Add(department);
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> UpdateDepartment(int departmentId, DepartmentVM departmentVM)
        {
            var department = _context.Departments.Find(departmentId);
            if (department == null)
                throw new Exception("Department not found");

            department.DepartmentName = departmentVM.DepartmentName;
            department.DepartmentDescription = departmentVM.DepartmentDescription;
            department.IsActive = departmentVM.IsActive;
            department.UpdatedAt = DateTime.UtcNow;

            return _context.SaveChanges() > 0;
        }

        public async Task<bool> DeleteDepartment(int departmentId)
        {
            var department = _context.Departments.Find(departmentId);
            if (department == null)
                throw new Exception("Department not found");

            department.IsActive = false; // Soft delete
            department.UpdatedAt = DateTime.UtcNow;

            return _context.SaveChanges() > 0;
        }

        public async Task<bool> RemoveDepartment(int departmentId)
        {
            var department = _context.Departments.Find(departmentId);
            if (department == null)
                throw new Exception("Department not found");

            _context.Departments.Remove(department);
            return _context.SaveChanges() > 0;
        }
    }
}
