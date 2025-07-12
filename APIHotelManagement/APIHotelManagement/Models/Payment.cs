using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using APIHotelManagement.Enums;

namespace APIHotelManagement.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public decimal? PaymentAmount { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]    // Converts enum to string in API responses
    public PaymentMethod PaymentMethod { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]    // Converts enum to string in API responses
    public PaymentStatus PaymentStatus { get; set; }

    public DateTime? PaymentDate { get; set; }

    public int ReservationId { get; set; }

    public virtual Reservation Reservation { get; set; } = null!;
}
