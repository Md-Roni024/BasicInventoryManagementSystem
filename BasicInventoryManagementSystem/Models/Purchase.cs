// Models/Purchase.cs
using System.ComponentModel.DataAnnotations;

namespace BasicInventoryManagementSystem.Models
{
    public class Purchase
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Supplier { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.Now;
    }
}
