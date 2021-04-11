using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Models
{
	public class SearchNote
	{
		public SellerNote Note { get; set; }
		public SellerNotesReview review { get; set; }
		public double Rating { get; set; }
		public int Total { get; set; }
		public int TotalSpam { get; set; }
		public int totalnotes { get; set; }
		
	}
}