using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class MemberDetails
    {
        public User user { get; set; }
        public UserProfile up { get; set; }
        public List<membernotes> Note { get; set; }
        public List<DownloadedNotes> downloads { get; set; }
    }
}