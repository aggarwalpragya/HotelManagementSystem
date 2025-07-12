namespace APIHotelManagement.DTOs
{
    public class BillVM
    {
        public int BillId { get; set; }
        public decimal RoomRate { get; set; } // Room charges
        public decimal ServiceCharges { get; set; } // Services used
        public decimal TotalAmount { get; set; }
        public decimal? Tax { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? BillingDate { get; set; }
        public int ReservationId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
