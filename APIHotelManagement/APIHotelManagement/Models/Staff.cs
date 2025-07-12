using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using APIHotelManagement.Enums;

namespace APIHotelManagement.Models;

public class Staff
{
    public int StaffId { get; set; }

    public string StaffName { get; set; } = null!;

    [JsonConverter(typeof(JsonStringEnumConverter))]    // Converts enum to string in API responses
    public Gender Gender { get; set; }

    public int DepartmentId { get; set; }

    public long ContactNumber { get; set; }

    public string Email { get; set; } = null!;

    public DateTime? HireDate { get; set; }

    public decimal Salary { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]    // Converts enum to string in API responses
    public WorkStatus WorkStatus { get; set; }

    public int ServiceId { get; set; }

    public int UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
