using BasicInventoryManagementSystem.Models;

namespace BasicInventoryManagementSystem.ViewModel
{
    public class PurchaseIndexViewModel
    {
        public IEnumerable<Purchase> Purchases { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? SearchQuery { get; set; }
    }
}
