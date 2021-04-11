using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class AdminDownload
    {
        public Download download { get; set; }
        public SellerNote note { get; set; }
        public Category category { get; set; }
        public User buyer { get; set; }
        public User seller { get; set; }

    }
}