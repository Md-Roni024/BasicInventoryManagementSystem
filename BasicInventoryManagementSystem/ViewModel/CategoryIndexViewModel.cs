using BasicInventoryManagementSystem.Models;

namespace BasicInventoryManagementSystem.ViewModel
{
    public class CategoryIndexViewModel
    {
        public IEnumerable<Category> Categories { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}
