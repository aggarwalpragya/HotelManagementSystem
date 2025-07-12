using System;
using System.Collections.Generic;

namespace APIHotelManagement.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = null!;

    public string? DepartmentDescription { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
