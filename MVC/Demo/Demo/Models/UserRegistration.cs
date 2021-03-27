using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Demo.Models
{
	public class UserRegistration
	{
		[Required(ErrorMessage = "First Name is Required")]
		public string Firstname { get; set; }

		[Required(ErrorMessage = "Last Name is Required")]
		public string LastName { get; set; }

		[Required(ErrorMessage ="Email is Required")]
		public string EmailID { get; set; }

		[Required(ErrorMessage ="Password is Required")]
		public string Password { get; set; }

		[Compare("Password",ErrorMessage ="Confirm Password doesn't match!!")]
		public string ConfirmPassword { get; set; }
	}
}