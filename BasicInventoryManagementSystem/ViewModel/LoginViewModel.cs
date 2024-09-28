using System.ComponentModel.DataAnnotations;

namespace BasicInventoryManagementSystem.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
