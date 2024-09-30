using System.Collections.Generic;
using BasicInventoryManagementSystem.Models;

namespace BasicInventoryManagementSystem.ViewModel
{
    public class SaleIndexViewModel
    {
        public IEnumerable<Sale> Sales { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
