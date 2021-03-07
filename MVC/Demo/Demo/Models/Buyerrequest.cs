using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class Buyerrequest
    {
        public Download download { get; set; }
        public SellerNote note { get; set; }
        public User user { get; set; }
        public UserProfile userprofile { get; set; }
    }
}