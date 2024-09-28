//using System;
//using System.ComponentModel.DataAnnotations;

//namespace BasicInventoryManagementSystem.Models
//{
//    public class User
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        [StringLength(100)]
//        public string Name { get; set; }

//        [Required]
//        [EmailAddress]
//        [StringLength(255)]
//        public string Email { get; set; }

//        [Required]
//        [DataType(DataType.Password)]
//        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
//        public string Password { get; set; }


//        [Required]
//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

//        [Required]
//        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; 
//    }
//}


//using Microsoft.AspNetCore.Identity;
//using System;
//using System.ComponentModel.DataAnnotations;

//namespace BasicInventoryManagementSystem.Models
//{
//    public class User
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        [StringLength(100)]
//        public string Name { get; set; }

//        [Required]
//        [EmailAddress]
//        [StringLength(255)]
//        public string Email { get; set; }

//        [Required]
//        public string PasswordHash { get; set; } // Store the password hash

//        [Required]
//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

//        [Required]
//        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
//    }
//}


//using Microsoft.AspNetCore.Identity;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

//namespace BasicInventoryManagementSystem.Models
//{
//    public class User : IdentityUser
//    {
//        [Key]
//        public override string Id { get; set; }

//        [Required]
//        [StringLength(100)]
//        public string Name { get; set; }

//        [Required]
//        [EmailAddress]
//        [StringLength(255)]
//        public override string Email { get; set; }

//        [Required]
//        public string PasswordHash { get; set; }


//        public virtual ICollection<IdentityRole> Roles { get; set; }

//        [DataType(DataType.DateTime)]
//        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

//        [DataType(DataType.DateTime)]
//        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
//    }
//}


//using Microsoft.AspNetCore.Identity;
//using System;
//using System.ComponentModel.DataAnnotations;

//namespace BasicInventoryManagementSystem.Models
//{
//    public class User : IdentityUser
//    {
//        [PersonalData]
//        [Required]
//        [StringLength(100)]
//        public string Name { get; set; }

//        [PersonalData]
//        [DataType(DataType.DateTime)]
//        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

//        [PersonalData]
//        [DataType(DataType.DateTime)]
//        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
//    }
//}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BasicInventoryManagementSystem.Models
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}

