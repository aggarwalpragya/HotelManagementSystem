using APIHotelManagement.Models;

namespace APIHotelManagement.Interfaces
{
    public interface IInventory
    {
        Task<IEnumerable<Inventory>> GetAllInventory();
        Task<Inventory> GetInventoryById(int inventoryId);
        Task<bool> AddInventory(Inventory inventory);
        Task<bool> UpdateInventory(int inventoryId, Inventory inventory);
        Task<bool> DeleteInventory(int inventoryId);
    }
}
