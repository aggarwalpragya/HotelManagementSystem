using System;
using System.Collections.Generic;

namespace APIHotelManagement.Models;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public string ItemName { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public DateOnly? BestBefore { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
