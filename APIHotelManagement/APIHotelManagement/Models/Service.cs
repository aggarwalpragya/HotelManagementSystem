using System;
using System.Collections.Generic;

namespace APIHotelManagement.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public decimal Price { get; set; }

    public string? ServiceDescription { get; set; }

    public bool? AvailabilityStatus { get; set; }

    public string Category { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ReservationService> ReservationServices { get; set; } = new List<ReservationService>();

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
