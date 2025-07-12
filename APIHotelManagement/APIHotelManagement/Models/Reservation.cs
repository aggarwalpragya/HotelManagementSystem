using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using APIHotelManagement.Enums;

namespace APIHotelManagement.Models;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int NumberOfGuest { get; set; }

    public DateTime CheckIn { get; set; }

    public DateTime CheckOut { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]    // Converts enum to string in API responses
    public ReservationStatus ReservationStatus { get; set; }

    public DateTime BookingDate { get; set; }

    public int RoomId { get; set; }

    public int GuestId { get; set; }

    public int UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual Guest Guest { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<ReservationService> ReservationServices { get; set; } = new List<ReservationService>();

    public virtual Room Room { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
