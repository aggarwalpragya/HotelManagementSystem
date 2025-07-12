using APIHotelManagement.Enums;
using System.Text.Json.Serialization;

namespace APIHotelManagement.DTOs
{
    public class ReservationVM
    {
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

    }
}

