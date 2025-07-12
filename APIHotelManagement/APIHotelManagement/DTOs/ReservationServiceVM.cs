namespace APIHotelManagement.DTOs
{
    public class ReservationServiceVM
    {
        public int ReservationId { get; set; }

        public int ServiceId { get; set; }

        public string ServiceName { get; set; } // To display service details

        public int Quantity { get; set; }
    }
}
