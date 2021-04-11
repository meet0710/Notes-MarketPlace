using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Demo.Models
{
	public class MyProfile
	{
		[Required(ErrorMessage = "This Field is Required")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "This Field is Required")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "This Field is Required")]
		public string EmailID { get; set; }

		public DateTime DOB { get; set; }

		public string Gender { get; set; }

		public string CountryCode { get; set; }

        public string PhoneNo { get; set; }

        public HttpPostedFileBase ProfilePicture { get; set; }

		[Required(ErrorMessage = "This Field is Required")]
		public string AddLine1 { get; set; }

		public string AddLine2 { get; set; }

		[Required(ErrorMessage = "This Field is Required")]
		public string City { get; set; }

		[Required(ErrorMessage = "This Field is Required")]
		public string State { get; set; }

		[Required(ErrorMessage = "This Field is Required")]
		public string Zipcode { get; set; }

		[Required(ErrorMessage = "This Field is Required")]
		public string Country { get; set; }

		public string University { get; set; }

		public string College { get; set; }

	}
}