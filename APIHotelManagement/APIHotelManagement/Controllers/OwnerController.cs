using APIHotelManagement.DTOs;
using APIHotelManagement.Enums;
using APIHotelManagement.Interfaces;
using APIHotelManagement.Models;
using APIHotelManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIHotelManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Owner")]
    public class OwnerController : ControllerBase
    {
        private readonly ILogger<OwnerController> _logger;
        private readonly IDepartment _departmentRepo;
        private readonly IService _serviceRepo;
        private readonly IRoomType _roomTypeRepo;
        private readonly IUser _userRepo;
        private readonly IReport _reportRepo;


        public OwnerController(ILogger<OwnerController> logger, IDepartment departmentRepo, IService serviveRepo, IRoomType roomTypeRepo, IUser userRepo, IReport reportRepo)
        {
            _logger = logger;
            _departmentRepo = departmentRepo;
            _serviceRepo = serviveRepo;
            _roomTypeRepo = roomTypeRepo;
            _userRepo = userRepo;
            _reportRepo = reportRepo;
        }


        // Report-----------------------------------------------------------------------


        // GET Monthly Financial Report
        [HttpGet("MonthlyReport/{year}/{month}")]
        public async Task<IActionResult> GetMonthlyReport(int year, int month)
        {
            try
            {
                var report = await _reportRepo.GetMonthlyReport(year, month);
                if (report == null) return NotFound(new { message = "No data available for the given month and year." });

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating monthly report for Year: {Year}, Month: {Month}", year, month);
                return StatusCode(500, "An internal server error occurred while generating the monthly report.");
            }
        }

        // GET Yearly Financial Report
        [HttpGet("YearlyReport/{year}")]
        public async Task<IActionResult> GetYearlyReport(int year)
        {
            try
            {
                var report = await _reportRepo.GetYearlyReport(year);
                if (report == null) return NotFound(new { message = "No data available for the given year." });

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating yearly report for Year: {Year}", year);
                return StatusCode(500, "An internal server error occurred while generating the yearly report.");
            }
        }



        // Department Details------------------------------------------------------------

        //[HttpGet("GetAllDepartments")]
        //public async Task<IActionResult> GetAllDepartments()
        //{
        //    try
        //    {
        //        var departments = await _departmentRepo.GetAllDepartments();
        //        return Ok(departments);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error: {ex.Message}");
        //    }
        //}

        [HttpGet("GetDepartmentById/{departmentId}")]
        public async Task<IActionResult> GetDepartmentById(int departmentId)
        {
            try
            {
                var department = await _departmentRepo.GetDepartmentById(departmentId);
                if (department == null) return NotFound(new { message = "Department not found." });

                return Ok(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching department with ID: {DepartmentId}", departmentId);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // GET Department by Name
        [HttpGet("GetDepartmentByName/{departmentName}")]
        public async Task<IActionResult> GetDepartmentByName(string departmentName)
        {
            try
            {
                var departments = await _departmentRepo.GetDepartmentByName(departmentName);
                if (departments == null || !departments.Any()) return NotFound(new { message = "No matching departments found." });

                return Ok(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching department with Name: {DepartmentName}", departmentName);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // POST Add Department
        [HttpPost("AddDepartment")]
        public async Task<IActionResult> AddDepartment([FromBody] DepartmentVM departmentVM)
        {
            try
            {
                bool added = await _departmentRepo.AddDepartment(departmentVM);
                if (added)
                    return Ok(new { message = "Department added successfully." });

                return BadRequest(new { message = "Failed to add department." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding department: {DepartmentVM}", departmentVM);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // PUT Update Department
        [HttpPut("UpdateDepartment/{departmentId}")]
        public async Task<IActionResult> UpdateDepartment(int departmentId, [FromBody] DepartmentVM departmentVM)
        {
            try
            {
                bool updated = await _departmentRepo.UpdateDepartment(departmentId, departmentVM);
                if (updated)
                    return Ok(new { message = "Department updated successfully." });

                return BadRequest(new { message = "Failed to update department." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating department with ID: {DepartmentId}, Data: {DepartmentVM}", departmentId, departmentVM);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // DELETE Soft Delete Department
        [HttpDelete("DeleteDepartment/{departmentId}")]
        public async Task<IActionResult> DeleteDepartment(int departmentId)
        {
            try
            {
                bool deleted = await _departmentRepo.DeleteDepartment(departmentId);
                if (deleted)
                    return Ok(new { message = "Department deleted successfully." });

                return BadRequest(new { message = "Failed to delete department." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department with ID: {DepartmentId}", departmentId);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // DELETE Permanent Remove Department
        [HttpDelete("RemoveDepartment/{departmentId}")]
        public async Task<IActionResult> RemoveDepartment(int departmentId)
        {
            try
            {
                bool removed = await _departmentRepo.RemoveDepartment(departmentId);
                if (removed)
                    return Ok(new { message = "Department removed permanently." });

                return BadRequest(new { message = "Failed to remove department." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error permanently removing department with ID: {DepartmentId}", departmentId);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // Service Details------------------------------------------------------------

        //[HttpGet("GetAllServices")]
        //public async Task<IActionResult> GetAllServices()
        //{
        //    var services = await _serviceRepo.GetAllServices();
        //    return Ok(services);
        //}


        // POST Add Service
        [HttpPost("AddService")]
        public async Task<IActionResult> AddService([FromBody] ServiceVM serviceVM)
        {
            try
            {
                var result = await _serviceRepo.AddService(serviceVM);
                if (result)
                    return Ok(new { message = "Service added successfully." });

                return BadRequest(new { message = "Failed to add service." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding service: {ServiceVM}", serviceVM);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // PUT Update Service
        [HttpPut("UpdateService/{serviceId}")]
        public async Task<IActionResult> UpdateService(int serviceId, [FromBody] ServiceVM serviceVM)
        {
            try
            {
                var result = await _serviceRepo.UpdateService(serviceId, serviceVM);
                if (result)
                    return Ok(new { message = "Service updated successfully." });

                return BadRequest(new { message = "Failed to update service." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating service with ID: {ServiceId}, Data: {ServiceVM}", serviceId, serviceVM);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // DELETE Soft Delete Service (Disable Service)
        [HttpDelete("DeleteService/{serviceId}")]
        public async Task<IActionResult> DeleteService(int serviceId)
        {
            try
            {
                var result = await _serviceRepo.DeleteService(serviceId);
                if (result)
                    return Ok(new { message = "Service disabled successfully." });

                return BadRequest(new { message = "Failed to disable service." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling service with ID: {ServiceId}", serviceId);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // DELETE Permanent Remove Service
        [HttpDelete("RemoveService/{serviceId}")]
        public async Task<IActionResult> RemoveService(int serviceId)
        {
            _logger.LogInformation("Request received to remove service with ID: {ServiceId}", serviceId);

            try
            {
                var result = await _serviceRepo.RemoveService(serviceId);
                if (result)
                {
                    _logger.LogInformation("Service with ID {ServiceId} removed successfully.", serviceId);
                    return Ok(new { message = "Service removed successfully." });
                }

                _logger.LogWarning("Failed to remove service with ID: {ServiceId}", serviceId);
                return BadRequest(new { message = "Failed to remove service." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing service with ID: {ServiceId}", serviceId);
                return StatusCode(500, new { message = "An internal server error occurred." });
            }
        }


        // Room Type Details------------------------------------------------------------

        //[HttpGet("GetAllRoomTypes")]
        //public async Task<IActionResult> GetAllRoomTypes()
        //{
        //    _logger.LogInformation("Fetching all room types.");

        //    try
        //    {
        //        var roomTypes = await _roomTypeRepo.GetAllRoomTypes();
        //        return Ok(roomTypes);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error fetching all room types.");
        //        return StatusCode(500, new { message = "An internal server error occurred while fetching room types." });
        //    }
        //}

        [AllowAnonymous]
        [HttpPatch("SetRoomRate/{roomTypeId}")]
        public async Task<IActionResult> SetRoomRate([FromRoute] int roomTypeId, [FromQuery] decimal newRate)
        {
            _logger.LogInformation("Request received to update room rate. RoomTypeId: {RoomTypeId}, NewRate: {NewRate}", roomTypeId, newRate);

            try
            {
                var success = await _roomTypeRepo.SetRoomRate(roomTypeId, newRate);
                if (!success)
                {
                    _logger.LogWarning("Failed to update room rate for RoomTypeId: {RoomTypeId}", roomTypeId);
                    return BadRequest(new { message = "Failed to update room rate." });
                }

                _logger.LogInformation("Successfully updated room rate for RoomTypeId: {RoomTypeId}", roomTypeId);
                return Ok(new { message = "Room rate updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating room rate for RoomTypeId: {RoomTypeId}", roomTypeId);
                return StatusCode(500, new { message = "An internal server error occurred while updating room rate." });
            }
        }


        
        // GET all users
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Fetching all users.");

            try
            {
                var users = await _userRepo.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users.");
                return StatusCode(500, new { message = "An error occurred while fetching users." });
            }
        }

        // GET user by ID
        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            _logger.LogInformation("Fetching user with ID: {UserId}", id);

            try
            {
                var user = await _userRepo.GetUserById(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found. ID: {UserId}", id);
                    return NotFound(new { message = "User not found." });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user with ID: {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while fetching user details." });
            }
        }

        // GET users by role
        [HttpGet("GetUserByRole/{role}")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            _logger.LogInformation("Fetching users by role: {UserRole}", role);

            try
            {
                if (!Enum.TryParse<UserRole>(role, true, out var userRole))
                {
                    _logger.LogWarning("Invalid user role provided: {UserRole}", role);
                    return BadRequest(new { message = "Invalid role." });
                }

                var users = await _userRepo.GetUsersByRole(userRole);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users by role: {UserRole}", role);
                return StatusCode(500, new { message = "An error occurred while fetching users by role." });
            }
        }

        // POST: Add user
        //[HttpPost("AddUser")]
        //public async Task<IActionResult> AddUser([FromBody] UserVM userVM)
        //{
        //    _logger.LogInformation("Adding new user: {UserName}", userVM.UserName);

        //    try
        //    {
        //        var isSuccess = await _userRepo.AddUser(userVM);
        //        if (!isSuccess)
        //        {
        //            _logger.LogWarning("Failed to add user: {UserName}", userVM.UserName);
        //            return BadRequest(new { message = "Failed to add user." });
        //        }

        //        _logger.LogInformation("User added successfully: {UserName}", userVM.UserName);
        //        return Ok(new { message = "User added successfully!" });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error adding user: {UserName}", userVM.UserName);
        //        return StatusCode(500, new { message = "An error occurred while adding the user." });
        //    }
        //}

        // PUT: Update user
        [HttpPut("UpdateUser/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserVM userVM)
        {
            _logger.LogInformation("Updating user with ID: {UserId}", userId);

            try
            {
                var isSuccess = await _userRepo.UpdateUser(userId, userVM);
                if (!isSuccess)
                {
                    _logger.LogWarning("User not found for update. ID: {UserId}", userId);
                    return NotFound(new { message = "User not found." });
                }

                _logger.LogInformation("User updated successfully. ID: {UserId}", userId);
                return Ok(new { message = "User updated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while updating the user." });
            }
        }
        

        // PATCH: Soft delete user
        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            _logger.LogInformation("Attempting to deactivate user with ID: {UserId}", userId);

            try
            {
                var isSuccess = await _userRepo.DeleteUser(userId);
                if (!isSuccess)
                {
                    _logger.LogWarning("User not found for deactivation. ID: {UserId}", userId);
                    return NotFound(new { message = "User not found." });
                }

                _logger.LogInformation("User deactivated successfully. ID: {UserId}", userId);
                return Ok(new { message = "User deactivated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user with ID: {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while deactivating the user." });
            }
        }

        // DELETE: Remove user permanently
        [HttpDelete("RemoveUser/{userId}")]
        public async Task<IActionResult> RemoveUser(int userId)
        {
            _logger.LogInformation("Attempting to permanently remove user with ID: {UserId}", userId);

            try
            {
                var isSuccess = await _userRepo.RemoveUser(userId);
                if (!isSuccess)
                {
                    _logger.LogWarning("User not found for permanent removal. ID: {UserId}", userId);
                    return NotFound(new { message = "User not found." });
                }

                _logger.LogInformation("User permanently removed. ID: {UserId}", userId);
                return Ok(new { message = "User removed permanently." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user with ID: {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while removing the user." });
            }
        }

    }
}
