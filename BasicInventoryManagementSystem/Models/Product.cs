//using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel.DataAnnotations;

//namespace BasicInventoryManagementSystem.Models
//{
//    public class Product
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        [StringLength(100)]
//        public string Name { get; set; }

//        [Required]
//        [StringLength(50)]
//        public string Category { get; set; }

//        [Required]
//        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
//        [Column(TypeName = "decimal(18, 2)")]
//        public decimal Price { get; set; }

//        [Required]
//        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative integer.")]
//        public int Quantity { get; set; }
//    }
//}


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BasicInventoryManagementSystem.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        // Initialize Name and Category to avoid null warnings
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
    }
}
