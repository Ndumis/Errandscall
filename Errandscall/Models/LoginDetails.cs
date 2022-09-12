using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Errandscall.Models
{
    public class LoginDetails
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Required.")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

        [Display(Name = "RememberMe")]
        public bool? RememberMe { get; set; }

        [Required(ErrorMessage = "Required.")]
        public string Cellphoner { get; set; }
        public string SMS { get; set; }
        public string Role { get; set; }
        public string SessionID { get; set; }
    }


    public class CustomSerializeModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RoleName { get; set; }
        public string Initials { get; set; }
        public string Email { get; set; }
    }
}