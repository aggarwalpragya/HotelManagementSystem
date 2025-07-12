namespace APIHotelManagement.DTOs
{
    public class RoomVM
    {
        public int RoomId { get; set; }

        public int RoomNumber { get; set; }

        public string RoomStatus { get; set; }  // Enum stored as string

        public int RoomTypeId { get; set; }

        public string? RoomType { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
