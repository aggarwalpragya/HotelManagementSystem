using System;
using System.Collections.Generic;

namespace APIHotelManagement.Models;

public partial class ReservationService
{
    public int ReservationId { get; set; }

    public int ServiceId { get; set; }

    public int Quantity { get; set; }

    public virtual Reservation Reservation { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
