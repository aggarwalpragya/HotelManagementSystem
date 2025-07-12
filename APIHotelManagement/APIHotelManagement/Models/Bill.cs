using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using APIHotelManagement.Enums;

namespace APIHotelManagement.Models;

public partial class Bill
{
    public int BillId { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? Tax { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]    // Converts enum to string in API responses
    public PaymentStatus PaymentStatus { get; set; }

    public DateTime? BillingDate { get; set; }

    public int ReservationId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Reservation Reservation { get; set; } = null!;
}
