using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class AdminNotesUnderReview
    {
        public SellerNote note { get; set; }
        public User user { get; set; }
        public Category category { get; set; }
        public NoteStatu status { get; set; }
        public int Noteid { get; set; }
        public string remarks { get; set; }
    }
}