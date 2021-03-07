using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class NoteDetail
    {
        public SellerNote note { get; set; }
        public SellerNotesReview review { get; set; }
        public Country country { get; set;  }
        public Category category { get; set; }
        public User user { get; set; }

        public double Rating { get; set; }
        public int Total { get; set; }
        public int TotalSpam { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Review { get; set; }
        public double IndRating { get; set; }

        public string ProfilePicture { get; set; }
    }
}