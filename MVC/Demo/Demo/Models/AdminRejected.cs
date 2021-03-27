using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class AdminRejected
    {
        public SellerNote Note { get; set; }
        public Category Category { get; set; }
        public User Seller { get; set; }
        public User Admin { get; set; }
    }
}