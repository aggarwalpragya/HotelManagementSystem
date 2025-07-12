namespace APIHotelManagement.DTOs
{
    public class BillDetailVM
    {
        public int BillId { get; set; }
        public string GuestName { get; set; }
        public string IdProofNumber { get; set; }
        public long ContactNumber { get; set; }

        public string RoomTypeName { get; set; }
        public decimal RoomRate { get; set; }

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        // we use List<string> to hold service descriptions
        public List<string> Services { get; set; } = new List<string>();

        public decimal TotalRoomCharges { get; set; }
        public decimal TotalServiceCharges { get; set; }
        public decimal FinalAmount { get; set; }
        public DateTime BillingDate { get; set; }
        public string PaymentStatus { get; set; }
    }

}
