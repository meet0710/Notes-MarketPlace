using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Demo.Models
{
	public class UserRegistration
	{
		public string Firstname { get; set; }

		public string LastName { get; set; }

		public string EmailID { get; set; }

		public string Password { get; set; }

		[Compare("Password")]
		public string ConfirmPassword { get; set; }
	}
}