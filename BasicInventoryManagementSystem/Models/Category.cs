using System;

namespace BasicInventoryManagementSystem.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string CategoryName { get; set; } 

        public DateTime CreatedDate { get; set; } 

        public DateTime UpdatedDate { get; set; } 
    }
}
