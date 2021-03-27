using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Models
{
	public class AddNoteDetails
	{

		public string Title { get; set; }

		public string Category { get; set; }

		public HttpPostedFileBase NotesPicture { get; set; }

		public HttpPostedFileBase Notespdf { get; set; }

		public string NoteType { get; set; }

		public string Pages { get; set; }

		public string Description { get; set; }

		public string Country { get; set; }

		public string InstitutionName { get; set; }

		public string Ispaid { get; set; }

		public string SellingPrice { get; set; }

		public HttpPostedFileBase NotesPreview { get; set; }

		public string CourseName { get; set; }

		public string CourseCode { get; set; }

		public string ProfessorName { get; set; }

		public string noteid { get; set; }

	}
}