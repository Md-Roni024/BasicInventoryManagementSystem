using System;
using System.Collections.Generic;
using BasicInventoryManagementSystem.Models;

namespace BasicInventoryManagementSystem.ViewModel
{
    public class ProductIndexViewModel
    {
        public List<Product> Products { get; set; }

        public int TotalProducts { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public string SearchTerm { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalProducts / PageSize);
    }
}
