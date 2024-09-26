using System;
using System.ComponentModel.DataAnnotations;

namespace BasicInventoryManagementSystem.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; } // Primary Key

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }


        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Automatically set to current date and time

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Automatically set to current date and time
    }
}
