using System;

namespace BasicInventoryManagementSystem.Models
{
    public class Category
    {
        public int Id { get; set; } // Primary Key

        public string CategoryName { get; set; } // Name of the category

        public DateTime CreatedDate { get; set; } // Date when the category was created

        public DateTime UpdatedDate { get; set; } // Date when the category was last updated
    }
}
