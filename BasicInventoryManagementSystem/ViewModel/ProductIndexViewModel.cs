using System;
using System.Collections.Generic;
using BasicInventoryManagementSystem.Models; // Assuming Product model is under Models namespace

namespace BasicInventoryManagementSystem.ViewModels // Or Models if you have put ViewModels there
{
    public class ProductIndexViewModel
    {
        public List<Product> Products { get; set; }
        public int TotalProducts { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; }

        // Add TotalPages property
        public int TotalPages => (int)Math.Ceiling((double)TotalProducts / PageSize);
    }
}
