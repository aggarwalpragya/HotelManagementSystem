namespace APIHotelManagement.DTOs
{
    public class DepartmentVM
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = null!;

        public string? DepartmentDescription { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool? IsActive { get; set; }
    }
}
