using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class AdminProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Secondaryemail { get; set; }
        public string code { get; set; }
        public string Phoneno { get; set; }
        public HttpPostedFileBase ProfilePicture { get; set; }
    }
}