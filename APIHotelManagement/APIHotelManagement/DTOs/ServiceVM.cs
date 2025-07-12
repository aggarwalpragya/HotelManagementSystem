namespace APIHotelManagement.DTOs
{
    public class ServiceVM
    {
        public int ServiceId { get; set; }

        public string ServiceName { get; set; } = null!;

        public decimal Price { get; set; }

        public string? ServiceDescription { get; set; }

        public bool? AvailabilityStatus { get; set; }

        public string Category { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
