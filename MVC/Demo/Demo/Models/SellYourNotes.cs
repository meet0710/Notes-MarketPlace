using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class SellYourNotes
    {
        public SellerNote Note { get; set; }
        public ReferenceData Refdata { get; set; }
        public Category category { get; set; }

        public int Notessold { get; set; }
        public decimal TotalMoney { get; set; }
        public int Downloads { get; set; }
        public int RejectedNotes { get; set; }
        public int Buyerrequests { get; set; }
    }
}