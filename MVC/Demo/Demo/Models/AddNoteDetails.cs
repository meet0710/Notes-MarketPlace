using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Demo.Models
{
	public class AddNoteDetails
	{

		[Required(ErrorMessage = "This Field is Required")]
		public string Title { get; set; }

		[Required(ErrorMessage = "This Field is Required")]
		public string Category { get; set; }

		public HttpPostedFileBase NotesPicture { get; set; }

		public HttpPostedFileBase Notespdf { get; set; }

		public string NoteType { get; set; }

		[Range (0,int.MaxValue,ErrorMessage ="Please enter valid number")]
		public string Pages { get; set; }

		[Required(ErrorMessage = "This Field is Required")]
		public string Description { get; set; }

		public string Country { get; set; }

		public string InstitutionName { get; set; }

		public string Ispaid { get; set; }

		[Required(ErrorMessage = "This Field is Required")]
		public string SellingPrice { get; set; }

		public HttpPostedFileBase NotesPreview { get; set; }

		public string CourseName { get; set; }

		public string CourseCode { get; set; }

		public string ProfessorName { get; set; }

		public string noteid { get; set; }

	}
}