using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class AddCountry
    {
        public int id { get; set; }

        [Required(ErrorMessage = "This Field is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "This Field is Required")]
        public string CountryCode { get; set; }
    }
}