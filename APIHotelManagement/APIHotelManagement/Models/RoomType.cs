using System;
using System.Collections.Generic;

namespace APIHotelManagement.Models;

public partial class RoomType
{
    public int RoomTypeId { get; set; }

    public string RoomTypeName { get; set; } = null!;

    public int Capacity { get; set; }

    public string? RoomDescription { get; set; }

    public decimal RoomRate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
