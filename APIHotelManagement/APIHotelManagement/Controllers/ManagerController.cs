using System.Text.Json;
using System.Text;
using APIHotelManagement.DTOs;
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
    [Authorize(Roles = "Owner, Manager")]
    public class ManagerController : ControllerBase
    {
        private readonly ILogger<ManagerController> _logger;
        private readonly IStaff _staffRepository;
        private readonly IRoom _roomRepo;
        private readonly IInventory _inventoryRepo;
        private readonly IDepartment _departmentRepo;
        private readonly IService _serviceRepo;
        private readonly IRoomType _roomTypeRepo;
        private readonly IHttpClientFactory _clientFactory;

        public ManagerController(ILogger<ManagerController> logger, IHttpClientFactory clientFactory, IRoomType roomTypeRepo, IStaff staffRepository, IRoom roomRepo, IInventory inventoryRepo, IDepartment departmentRepo, IService serviceRepo)
        {
            _logger = logger;
            _staffRepository = staffRepository;
            _roomRepo = roomRepo;
            _inventoryRepo = inventoryRepo;
            _departmentRepo = departmentRepo;
            _serviceRepo = serviceRepo;
            _roomTypeRepo = roomTypeRepo;
            _clientFactory = clientFactory;
        }

        // Staff Details------------------------------------------------------------

        [HttpGet("GetAllStaffs")]
        public async Task<IActionResult> GetAllStaff()
        {
            try
            {
                _logger.LogInformation("Fetching all staff details");
                var staffList = await _staffRepository.GetAllStaff();
                return Ok(staffList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all staff details");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetStaffById/{id}")]
        public async Task<IActionResult> GetStaffById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching staff details for ID: {StaffId}", id);
                var staff = await _staffRepository.GetStaffById(id);
                return Ok(staff);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Staff ID {StaffId} not found", id);
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetStaffByName/{name}")]
        public async Task<IActionResult> GetStaffByName(string name)
        {
            try
            {
                _logger.LogInformation("Fetching staff by name: {Name}", name);
                var staffList = await _staffRepository.GetStaffByName(name);

                if (staffList == null || !staffList.Any())
                {
                    _logger.LogWarning("No staff found with name: {Name}", name);
                    return NotFound(new { message = "No staff found with the given name" });
                }

                return Ok(staffList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching staff by name: {Name}", name);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
        
        [HttpPost("AddStaff")]
        public async Task<IActionResult> AddStaff([FromBody] StaffVM staffVM)
        {
            try
            {
                var createdStaff = await _staffRepository.AddStaff(staffVM);
                if (createdStaff == null)
                    return BadRequest(new { message = "Failed to add staff" });

                // Create an email request object for notification
                var emailRequest = new
                {
                    ToEmail = staffVM.Email,
                    Subject = "Welcome to the Team!",
                    Body = $"Dear {staffVM.StaffName},\n\nWelcome to our team. We are excited to have you on board!"
                };

                var json = JsonSerializer.Serialize(emailRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                
                var emailServiceUrl = "https://localhost:7291/api/Email/send";  //url of emailx
                var client = _clientFactory.CreateClient();

                var response = await client.PostAsync(emailServiceUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Email service returned status code: {response.StatusCode}. Response: {responseContent}");
                }

                return StatusCode(201, createdStaff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding staff");
                return BadRequest("Internal server error");
            }
        }


        [HttpPut("UpdateStaff")]
        public async Task<IActionResult> UpdateStaff([FromBody] StaffVM staffVM)
        {
            try
            {
                _logger.LogInformation("Updating staff with ID: {StaffId}", staffVM.StaffId);
                var result = await _staffRepository.UpdateStaff(staffVM);

                if (result)
                {
                    _logger.LogInformation("Successfully updated staff with ID: {StaffId}", staffVM.StaffId);
                    return Ok(new { message = "Staff updated successfully" });
                }

                _logger.LogWarning("Failed to update staff with ID: {StaffId}", staffVM.StaffId);
                return BadRequest(new { message = "Failed to update staff" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Staff not found with ID: {StaffId}", staffVM.StaffId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating staff with ID: {StaffId}", staffVM.StaffId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("DeleteStaff/{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            try
            {
                _logger.LogInformation("Deleting staff with ID: {StaffId}", id);
                var result = await _staffRepository.DeleteStaff(id);
                return result ? Ok(new { message = "Staff marked as Terminated" }) : BadRequest(new { message = "Failed to delete staff" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Staff ID {StaffId} not found", id);
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("RemoveStaff/{staffId}")]
        public async Task<IActionResult> RemoveStaff(int staffId)
        {
            try
            {
                _logger.LogInformation("Removing staff permanently with ID: {StaffId}", staffId);
                bool removed = await _staffRepository.RemoveStaff(staffId);

                if (removed)
                {
                    _logger.LogInformation("Successfully removed staff with ID: {StaffId}", staffId);
                    return Ok(new { message = "Staff permanently removed from database" });
                }

                _logger.LogWarning("Failed to remove staff with ID: {StaffId}", staffId);
                return BadRequest(new { message = "Failed to remove staff" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing staff with ID: {StaffId}", staffId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // Room Details------------------------------------------------------------

        [HttpGet("GetAllRooms")]
        public async Task<IActionResult> GetAllRooms()
        {
            try
            {
                _logger.LogInformation("Fetching all room details");
                var rooms = await _roomRepo.GetAllRooms();
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all rooms");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetRoomsByStatus/{roomStatus}")]
        public async Task<IActionResult> GetRoomsByStatus(string roomStatus)
        {
            try
            {
                _logger.LogInformation("Fetching rooms with status: {RoomStatus}", roomStatus);
                var rooms = await _roomRepo.GetRoomsByStatus(roomStatus);

                if (rooms == null || !rooms.Any())
                {
                    _logger.LogWarning("No rooms found with status: {RoomStatus}", roomStatus);
                    return NotFound(new { message = "No rooms found with the given status" });
                }

                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching rooms with status: {RoomStatus}", roomStatus);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("GetRoomById/{roomId}")]
        public async Task<IActionResult> GetRoomById(int roomId)
        {
            try
            {
                _logger.LogInformation("Fetching room details for Room ID: {RoomId}", roomId);
                var room = await _roomRepo.GetRoomById(roomId);

                if (room == null)
                {
                    _logger.LogWarning("Room not found with ID: {RoomId}", roomId);
                    return NotFound(new { message = "Room not found" });
                }

                return Ok(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching room details for Room ID: {RoomId}", roomId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }


        [HttpPost("AddRoom")]
        public async Task<IActionResult> AddRoom([FromBody] RoomVM roomVM)
        {
            try
            {
                _logger.LogInformation("Adding new room");
                var result = await _roomRepo.AddRoom(roomVM);
                return result ? Ok(new { message = "Room added successfully" }) : BadRequest(new { message = "Failed to add room" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding room");
                return BadRequest("Internal server error");
            }
        }

        [HttpPut("UpdateRoom/{roomId}")]
        public async Task<IActionResult> UpdateRoom(int roomId, [FromBody] RoomVM roomVM)
        {
            try
            {
                _logger.LogInformation("Updating room with ID: {RoomId}", roomId);

                if (roomVM == null)
                {
                    _logger.LogWarning("Update request failed: RoomVM is null for Room ID: {RoomId}", roomId);
                    return BadRequest(new { message = "Invalid room data" });
                }

                bool updated = await _roomRepo.UpdateRoom(roomId, roomVM);
                if (updated)
                {
                    _logger.LogInformation("Room updated successfully with ID: {RoomId}", roomId);
                    return Ok(new { message = "Room updated successfully" });
                }

                _logger.LogWarning("Update failed: No room found with ID: {RoomId}", roomId);
                return NotFound(new { message = "Room not found or update failed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating room with ID: {RoomId}", roomId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }


        [HttpDelete("DeleteRoom/{roomId}")]
        public async Task<IActionResult> DeleteRoom(int roomId)
        {
            try
            {
                _logger.LogInformation("Deleting room with ID: {RoomId}", roomId);
                var result = await _roomRepo.DeleteRoom(roomId);
                return result ? Ok(new { message = "Room marked as Maintenance" }) : BadRequest(new { message = "Failed to delete room" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting room");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpDelete("RemoveRoom/{roomId}")]
        public async Task<IActionResult> RemoveRoom(int roomId)
        {
            try
            {
                _logger.LogInformation("Attempting to remove room with ID: {RoomId}", roomId);

                bool removed = await _roomRepo.RemoveRoom(roomId);
                if (removed)
                {
                    _logger.LogInformation("Room with ID: {RoomId} was successfully removed", roomId);
                    return Ok(new { message = "Room permanently removed from database" });
                }

                _logger.LogWarning("Failed to remove room: Room with ID {RoomId} not found or already deleted", roomId);
                return NotFound(new { message = "Room not found or already removed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing room with ID: {RoomId}", roomId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }


        // Inventory Details------------------------------------------------------------

        [HttpGet("GetAllInventory")]
        public async Task<IActionResult> GetAllInventory()
        {
            try
            {
                _logger.LogInformation("Fetching all inventory details");
                var inventory = await _inventoryRepo.GetAllInventory();
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching inventory");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetInventoryById/{inventoryId}")]
        public async Task<IActionResult> GetInventoryById(int inventoryId)
        {
            try
            {
                _logger.LogInformation("Fetching inventory with ID: {InventoryId}", inventoryId);

                var inventory = await _inventoryRepo.GetInventoryById(inventoryId);

                if (inventory == null)
                {
                    _logger.LogWarning("Inventory with ID {InventoryId} not found", inventoryId);
                    return NotFound(new { message = "Inventory not found" });
                }

                _logger.LogInformation("Successfully retrieved inventory with ID: {InventoryId}", inventoryId);
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching inventory with ID: {InventoryId}", inventoryId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }


        [HttpPost("AddInventory")]
        public async Task<IActionResult> AddInventory([FromBody] Inventory inventory)
        {
            try
            {
                _logger.LogInformation("Adding new inventory item");
                var result = await _inventoryRepo.AddInventory(inventory);
                return result ? Ok(new { message = "Inventory item added successfully" }) : BadRequest(new { message = "Failed to add inventory item" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding inventory item");
                return BadRequest("Internal server error");
            }
        }

        [HttpPut("UpdateInventory/{inventoryId}")]
        public async Task<IActionResult> UpdateInventory(int inventoryId, [FromBody] Inventory inventory)
        {
            try
            {
                _logger.LogInformation("Updating inventory item with ID: {InventoryId}", inventoryId);

                bool updated = await _inventoryRepo.UpdateInventory(inventoryId, inventory);
                if (updated)
                {
                    _logger.LogInformation("Inventory item with ID: {InventoryId} updated successfully", inventoryId);
                    return Ok(new { message = "Inventory item updated successfully" });
                }

                _logger.LogWarning("Failed to update inventory item with ID: {InventoryId}", inventoryId);
                return BadRequest(new { message = "Failed to update inventory item" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Inventory item with ID {InventoryId} not found", inventoryId);
                return NotFound(new { message = "Inventory item not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating inventory item with ID: {InventoryId}", inventoryId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("DeleteInventory/{inventoryId}")]
        public async Task<IActionResult> DeleteInventory(int inventoryId)
        {
            try
            {
                _logger.LogInformation("Deleting inventory item with ID: {InventoryId}", inventoryId);

                bool deleted = await _inventoryRepo.DeleteInventory(inventoryId);
                if (deleted)
                {
                    _logger.LogInformation("Inventory item with ID: {InventoryId} deleted successfully", inventoryId);
                    return Ok(new { message = "Inventory item deleted successfully" });
                }

                _logger.LogWarning("Failed to delete inventory item with ID: {InventoryId}", inventoryId);
                return BadRequest(new { message = "Failed to delete inventory item" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Inventory item with ID {InventoryId} not found", inventoryId);
                return NotFound(new { message = "Inventory item not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting inventory item with ID: {InventoryId}", inventoryId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }


        // Department Details------------------------------------------------------------

        [HttpGet("GetAllDepartments")]
        public async Task<IActionResult> GetAllDepartments()
        {
            try
            {
                _logger.LogInformation("Fetching all departments from the database.");

                var departments = await _departmentRepo.GetAllDepartments();

                if (departments == null || !departments.Any())
                {
                    _logger.LogWarning("No departments found.");
                    return NotFound(new { message = "No departments found" });
                }

                _logger.LogInformation("Successfully retrieved {Count} departments.", departments.Count());
                return Ok(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching departments.");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // get all room types

        [HttpGet("GetAllRoomTypes")]
        public async Task<IActionResult> GetAllRoomTypes()
        {
            try
            {
                var roomTypes = await _roomTypeRepo.GetAllRoomTypes();
                return Ok(roomTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching room types");
                return StatusCode(500, "An error occurred while retrieving room types.");
            }
        }


        //[HttpGet("GetAllServices")]
        //public async Task<IActionResult> GetAllServices()
        //{
        //    var services = await _serviceRepo.GetAllServices();
        //    return Ok(services);
        //}
    }
}
