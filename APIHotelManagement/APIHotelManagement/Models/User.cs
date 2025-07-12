using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using APIHotelManagement.Enums;

namespace APIHotelManagement.Models;

public class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole UserRole { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
