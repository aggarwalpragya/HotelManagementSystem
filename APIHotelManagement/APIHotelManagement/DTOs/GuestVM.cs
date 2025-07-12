using APIHotelManagement.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APIHotelManagement.DTOs
{
    public class GuestVM
    {
        public int GuestId { get; set; }

        public string GuestName { get; set; } = null!;

        public string IdProofType { get; set; } // Enum stored as string in ViewModel

        public string IdProofNumber { get; set; } = null!;

        [RegularExpression(@"^\+?1?\d{9,15}$", ErrorMessage = "Invalid phone number")]
        public long ContactNumber { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
