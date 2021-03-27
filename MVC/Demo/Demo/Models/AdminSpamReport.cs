using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class AdminSpamReport
    {
        public SellerNote Note { get; set; }
        public Category Category { get; set; }
        public SellerNotesReportedIssue spamreport { get; set; }
        public User user { get; set; }
    }
}