using System.ComponentModel.DataAnnotations;

namespace APIHotelManagement.DTOs
{
    public class StaffVM
    {
        public int StaffId { get; set; }

        public string StaffName { get; set; } = null!;

        public string Gender { get; set; } // Enum as string

        public int DepartmentId { get; set; }

        [RegularExpression(@"^\+?1?\d{9,15}$", ErrorMessage = "Invalid phone number")]
        public long ContactNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; } = null!;

        public DateTime? HireDate { get; set; }

        public decimal Salary { get; set; }

        public string WorkStatus { get; set; } // Enum as string

        public int ServiceId { get; set; }

        public int UserId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? DepartmentName { get; set; }

        public string? ServiceName { get; set; }
    }
}
