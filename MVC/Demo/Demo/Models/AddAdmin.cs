using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class AddAdmin
    {
        public int id { get; set; }

        [Required(ErrorMessage = "This Field is Required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This Field is Required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "This Field is Required")]
        public string Email { get; set; }
        
        public string Code { get; set; }
        
        public string Phoneno { get; set; }
    }
}