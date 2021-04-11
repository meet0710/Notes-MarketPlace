using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class AdminDashboard
    {
        public SellerNote Note { get; set; }
        public Category category { get; set; }
        public User user { get; set; }
        public User approver { get; set; }
        public int Total { get; set; }
        public int noteid { get; set; }
        public string remarks { get; set; }
        public int notesinreview { get; set; }
        public int newregistration { get; set; }
        public int totaldownloads { get; set; }
        
        
    }
}