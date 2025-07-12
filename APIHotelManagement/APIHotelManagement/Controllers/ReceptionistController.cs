using APIHotelManagement.DTOs;
using APIHotelManagement.Enums;
using APIHotelManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIHotelManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Owner, Manager, Receptionist")]
    public class ReceptionistController : ControllerBase
    {
        private readonly ILogger<ReceptionistController> _logger;
        private readonly IGuest _guestRepo;
        private readonly IRoom _roomRepo;
        private readonly IReservation _reservationRepo;
        private readonly IReservationService _reservationServiceRepository;
        private readonly IPayment _paymentRepo;
        private readonly IBill _billRepo;
        private readonly IRoomType _roomTypeRepo;
        private readonly IService _serviceRepo;

        public ReceptionistController(ILogger<ReceptionistController> logger, IGuest guestRepo, IRoom roomRepo, IReservation reservationRepo, IReservationService reservationServiceRepository, IPayment paymentRepo, IBill billRepo, IRoomType roomTypeRepo, IService serviceRepo)
        {
            _logger = logger;
            _guestRepo = guestRepo;
            _roomRepo = roomRepo;
            _reservationRepo = reservationRepo;
            _reservationServiceRepository = reservationServiceRepository;
            _paymentRepo = paymentRepo;
            _billRepo = billRepo;
            _roomTypeRepo = roomTypeRepo;
            _serviceRepo = serviceRepo;
        }

        // Guest Details------------------------------------------------------------

        [HttpGet("GetAllGuests")]
        public async Task<IActionResult> GetAllGuests()
        {
            try
            {
                var guests = await _guestRepo.GetAllGuests();
                return Ok(guests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving guests");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("GetGuestById/{guestId}")]
        public async Task<IActionResult> GetGuestById(int guestId)
        {
            try
            {
                var guest = await _guestRepo.GetGuestById(guestId);
                if (guest == null)
                {
                    return NotFound(new { message = "Guest not found." });
                }
                return Ok(guest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching guest with ID {GuestId}", guestId);
                return StatusCode(500, "An error occurred while retrieving the guest.");
            }
        }

        [HttpPost("AddGuest")]
        public async Task<IActionResult> AddGuest([FromBody] GuestVM guestVM)
        {
            try
            {
                bool added = await _guestRepo.AddGuest(guestVM);
                if (added)
                    return Ok(new { message = "Guest added successfully" });
                return BadRequest(new { message = "Failed to add guest" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding guest");
                return StatusCode(500, "An error occurred while adding the guest.");
            }
        }

        [HttpPut("UpdateGuest/{guestId}")]
        public async Task<IActionResult> UpdateGuest(int guestId, [FromBody] GuestVM guestVM)
        {
            try
            {
                bool updated = await _guestRepo.UpdateGuest(guestId, guestVM);
                if (updated)
                    return Ok(new { message = "Guest updated successfully" });
                return BadRequest(new { message = "Failed to update guest" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating guest with ID {GuestId}", guestId);
                return StatusCode(500, "An error occurred while updating the guest.");
            }
        }

        [HttpDelete("DeleteGuest/{guestId}")]
        public async Task<IActionResult> DeleteGuest(int guestId)
        {
            try
            {
                bool deleted = await _guestRepo.DeleteGuest(guestId);
                if (deleted)
                    return Ok(new { message = "Guest deactivated successfully" });
                return BadRequest(new { message = "Failed to deactivate guest" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating guest with ID {GuestId}", guestId);
                return StatusCode(500, "An error occurred while deactivating the guest.");
            }
        }

        [HttpDelete("RemoveGuest/{guestId}")]
        public async Task<IActionResult> RemoveGuest(int guestId)
        {
            try
            {
                bool removed = await _guestRepo.RemoveGuest(guestId);
                if (removed)
                    return Ok(new { message = "Guest removed permanently" });

                return BadRequest(new {message = "Failed to remove guest" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting guest with ID {GuestId}", guestId);
                return StatusCode(500, "An error occurred while deleting the guest.");
            }
        }

        // Room Search------------------------------------------------------------

        [HttpGet("SearchAvailableRooms")]
        public async Task<IActionResult> SearchAvailableRooms(string roomTypeName, DateTime checkIn, DateTime checkOut)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roomTypeName))
                    return BadRequest(new { message = "Room type name is required." });
                if (checkIn >= checkOut)
                    return BadRequest(new { message = "Check-out date must be after check-in date." });
                var availableRooms = await _roomRepo.SearchAvailableRooms(roomTypeName, checkIn, checkOut);
                if (!availableRooms.Any())
                    return NotFound(new { message = "No rooms available for the selected dates." });
                return Ok(availableRooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching available rooms");
                return StatusCode(500, "An error occurred while searching for rooms.");
            }
        }

        // Reservation Details------------------------------------------------------

        [HttpGet("GetAllReservations")]
        public async Task<IActionResult> GetAllReservations()
        {
            try
            {
                var reservations = await _reservationRepo.GetAllReservations();
                return reservations.Any() ? Ok(reservations) : NotFound(new { message = "No reservations found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all reservations");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost("MakeReservation")]
        public async Task<IActionResult> MakeReservation([FromBody] MakeReservationVM makeReservationVM)
        {
            try
            {
                bool isSuccess = await _reservationRepo.MakeReservation(makeReservationVM);
                return isSuccess ? Ok(new { message = "Reservation successful!" }) : BadRequest(new { message = "Failed to make reservation." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making reservation");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("UpdateReservation/{reservationId}")]
        public async Task<IActionResult> UpdateReservation(int reservationId, [FromBody] MakeReservationVM makeReservationVM)
        {
            try
            {
                var result = await _reservationRepo.UpdateReservation(reservationId, makeReservationVM);
                if (!result)
                    return BadRequest(new { message = "Failed to update reservation." });

                return Ok(new { message = "Reservation updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reservation");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("DeleteReservation/{reservationId}")]
        public async Task<IActionResult> DeleteReservation(int reservationId)
        {
            try
            {
                var result = await _reservationRepo.DeleteReservation(reservationId);
                if (!result)
                    return BadRequest(new { message = "Failed to cancel reservation." });

                return Ok(new { message = "Reservation cancelled successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating reservation");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("RemoveReservation/{reservationId}")]
        public async Task<IActionResult> RemoveReservation(int reservationId)
        {
            try
            {
                var result = await _reservationRepo.RemoveReservation(reservationId);
                if (!result)
                    return BadRequest(new { message = "Failed to remove reservation." });
                return Ok(new { message = "Reservation removed permanently." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing reservation with ID {reservationId}", reservationId);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Reservation Service Details------------------------------------------------------

        [HttpGet("ReservationServicesUsed")]
        public async Task<IActionResult> GetAllReservationServices()
        {
            try
            {
                var reservationServices = await _reservationServiceRepository.GetAllReservationServices();
                if (reservationServices.Any())
                    return Ok(reservationServices);
                return BadRequest(new { message = "No reservation services found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reservation services");
                return StatusCode(500, "Internal Server Error");
            }
        }

        //Payment Details------------------------------------------------------

        [HttpPatch("UpdatePaymentStatus/{paymentId}")]
        public async Task<IActionResult> UpdatePaymentStatus(int paymentId, [FromQuery] string newStatus)
        {
            try
            {
                var success = await _paymentRepo.UpdatePaymentStatus(paymentId, newStatus);
                if (!success)
                    return BadRequest(new { message = "Failed to update payment status." });
                return Ok(new { message = "Payment status updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment status for Payment ID {PaymentId}", paymentId);
                return StatusCode(500, "An error occurred while updating payment status.");
            }
        }

        [HttpDelete("DeletePayment/{paymentId}")]
        public async Task<IActionResult> DeletePayment(int paymentId)
        {
            try
            {
                bool deleted = await _paymentRepo.DeletePayment(paymentId);
                return deleted ? Ok(new { message = "Payment deleted successfully" }) : NotFound(new { message = "Payment not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting payment");
                return StatusCode(500, "Internal server error");
            }
        }

        // Bill Details--------------------------------------------------------

        [HttpGet("GetAllBills")]
        public async Task<IActionResult> GetAllBills()
        {
            try
            {
                var bills = await _billRepo.GetAllBills();
                return Ok(bills);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all bills");
                return StatusCode(500, "Internal server error");
            }
        }

        
        [HttpGet("GetBillById/{id}")]
        public async Task<IActionResult> GetBillById(int id)
        {
            try
            {
                var bill = await _billRepo.GetBillById(id);
                if (bill == null)
                    return NotFound(new { message = "Bill not found." });
                return Ok(bill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bill with ID {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

       
        [HttpGet("GetBillByPaymentStatus/{status}")]
        public async Task<IActionResult> GetBillsByPaymentStatus(string status)
        {
            try
            {
                if (!Enum.TryParse<PaymentStatus>(status, true, out var paymentStatus))
                    return BadRequest(new { message = "Invalid payment status." });
                var bills = await _billRepo.GetBillsByPaymentStatus(paymentStatus);
                return Ok(bills);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bills with payment status {status}", status);
                return StatusCode(500, "Internal Server Error");
            }
        }

        
        [HttpPut("UpdateBill/{id}")]
        public async Task<IActionResult> UpdateBill(int id, [FromBody] BillVM billVM)
        {
            var isSuccess = await _billRepo.UpdateBill(id, billVM);
            if (!isSuccess) return NotFound("Bill not found.");
            return Ok(new { message = "Bill updated successfully!" });
        }

        
        [HttpDelete("DeleteBill/{id}")]
        public async Task<IActionResult> DeleteBill(int id)
        {
            try
            {
                var isSuccess = await _billRepo.DeleteBill(id);
                if (!isSuccess) return NotFound(new { message = "Bill not found." });
                return Ok(new { message = "Bill deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting bill with ID {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        
        [AllowAnonymous]
        [HttpPost("IssueBill/{reservationId}")]
        public async Task<IActionResult> IssueBill(int reservationId)
        {
            try
            {
                var bill = await _billRepo.IssueBill(reservationId);
                return bill != null ? Ok(bill) : BadRequest(new { message = "Failed to issue bill." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error issuing bill for reservationId: {reservationId}", reservationId);
                return StatusCode(500, "Internal Server Error");
            }
        }


        // Room Type Details------------------------------------------------------------

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
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Sevices Details--------------------------------------------------------------

        [HttpGet("GetAllServices")]
        public async Task<IActionResult> GetAllServices()
        {
            try
            {
                var services = await _serviceRepo.GetAllServices();
                return Ok(services);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving services");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}

