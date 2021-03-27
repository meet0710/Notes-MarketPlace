using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class MyDownloadReviews
    {
        public int NoteID { get; set; }
        public int AgainstDownloadsID { get; set; }
        public int ratings { get; set; }
        public string reviews { get; set; }
       
    }
}