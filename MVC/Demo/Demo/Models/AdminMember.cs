using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class AdminMember
    {
        public User user { get; set; }
        public int underreviewnotes { get; set; }
        public int totalpublishednotes { get; set; }
        public int totaldownloads { get; set; }
        public int totalexpense { get; set; }
        public int totalearning { get; set; }
    }
}