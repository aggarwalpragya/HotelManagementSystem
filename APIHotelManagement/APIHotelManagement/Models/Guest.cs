using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using APIHotelManagement.Enums;

namespace APIHotelManagement.Models;

public class Guest
{
    public int GuestId { get; set; }

    public string GuestName { get; set; } = null!;

    public IdProofType IdProofType { get; set; }

    public string IdProofNumber { get; set; } = null!;

    public long ContactNumber { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
