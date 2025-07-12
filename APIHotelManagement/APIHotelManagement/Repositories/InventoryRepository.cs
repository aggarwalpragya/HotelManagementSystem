using APIHotelManagement.Interfaces;
using APIHotelManagement.Models;

namespace APIHotelManagement.Repositories
{
    public class InventoryRepository : IInventory
    {
        private readonly HotelManagementDbContext _context;

        public InventoryRepository(HotelManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Inventory>> GetAllInventory()
        {
            return _context.Inventories.ToList();
        }

        public async Task<Inventory> GetInventoryById(int inventoryId)
        {
            var inventory = _context.Inventories.Find(inventoryId);
            if (inventory == null)
                throw new Exception("Inventory item not found");

            return inventory;
        }

        public async Task<bool> AddInventory(Inventory inventory)
        {
            inventory.CreatedAt = DateTime.UtcNow;
            _context.Inventories.Add(inventory);
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> UpdateInventory(int inventoryId, Inventory inventory)
        {
            var existingInventory = _context.Inventories.Find(inventoryId);
            if (existingInventory == null)
                throw new Exception("Inventory item not found");

            existingInventory.ItemName = inventory.ItemName;
            existingInventory.Quantity = inventory.Quantity;
            existingInventory.UnitPrice = inventory.UnitPrice;
            existingInventory.BestBefore = inventory.BestBefore;
            existingInventory.UpdatedAt = DateTime.UtcNow;

            return _context.SaveChanges() > 0;
        }

        public async Task<bool> DeleteInventory(int inventoryId)
        {
            var inventory = _context.Inventories.Find(inventoryId);
            if (inventory == null)
                throw new Exception("Inventory item not found");

            _context.Inventories.Remove(inventory);
            return _context.SaveChanges() > 0;
        }
    }
}
