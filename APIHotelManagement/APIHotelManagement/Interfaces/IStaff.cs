using APIHotelManagement.DTOs;

namespace APIHotelManagement.Interfaces
{
    public interface IStaff
    {
        Task<IEnumerable<StaffVM>> GetAllStaff();
        Task<StaffVM?> GetStaffById(int staffId);
        Task<IEnumerable<StaffVM>> GetStaffByName(string name);
        Task<bool> AddStaff(StaffVM staffVm);
        Task<bool> UpdateStaff(StaffVM staffVm);
        Task<bool> DeleteStaff(int staffId);
        Task<bool> RemoveStaff(int staffId);

    }
}
