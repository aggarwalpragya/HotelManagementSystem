using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using APIHotelManagement.Enums;

namespace APIHotelManagement.Models;

public partial class Room
{
    public int RoomId { get; set; }

    public int RoomNumber { get; set; }

    public RoomStatus RoomStatus { get; set; }

    public int RoomTypeId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual RoomType RoomType { get; set; } = null!;
}
