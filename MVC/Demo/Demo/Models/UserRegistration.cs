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
		[RegularExpression("^(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,24}$", ErrorMessage = "Must Contain 1 lowercase, 1 special and 1 digit character")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Confirm Password is Required")]
		[Compare("Password",ErrorMessage ="Confirm Password doesn't match!!")]
		public string ConfirmPassword { get; set; }
	}
}