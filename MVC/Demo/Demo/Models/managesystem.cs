using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class managesystem
    {
        public string supportemail { get; set; }
        public string supportphoneno { get; set; }
        public string email { get; set; }
        public string fburl { get; set; }
        public string twitterurl { get; set; }
        public string linkedinurl { get; set; }
        public HttpPostedFileBase defaultnote { get; set; }
        public HttpPostedFileBase defaultprofile { get; set; }
    }
}