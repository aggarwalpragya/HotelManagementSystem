using System.ComponentModel.DataAnnotations;

namespace APIHotelManagement.DTOs
{
    public class MakeReservationVM
    {
        public int ReservationId { get; set; }
        public string RoomTypeName { get; set; }  // Search for this room type

        public int NumberOfGuest { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }

        public int GuestId { get; set; }

        public int UserId { get; set; }

        public int RoomId { get; set; }

        public string ReservationStatus { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // From Guest Table
        public string GuestName { get; set; }

        public string IdProofType { get; set; }

        public string IdProofNumber { get; set; }

        [RegularExpression(@"^\+?1?\d{9,15}$", ErrorMessage = "Invalid phone number")]
        public long ContactNumber { get; set; }

        // List of services associated with this reservation
        public List<ReservationServiceVM> Services { get; set; } = new List<ReservationServiceVM>();
    }
}
