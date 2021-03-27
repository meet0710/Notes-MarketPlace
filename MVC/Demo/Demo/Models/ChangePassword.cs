using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace Demo.Models
{
    public class ChangePassword
    {
        [Required(ErrorMessage = "Password is Required")]
        public string oldpwd { get; set; }

        [StringLength(24, MinimumLength = 6, ErrorMessage = "Must be at least 6 characters long.")]
        [RegularExpression("^(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,24}$", ErrorMessage = "Must Contain 1 lowercase, 1 special and 1 digit character")]
        public string newpwd { get; set; }

        [Compare("newpwd",ErrorMessage = "Confirm Password doesn't match!!")]
        public string confirmpwd { get; set; }
    }
}