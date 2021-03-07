using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Demo.Models;
using Demo.Utility;

namespace Demo.Controllers
{
	public class HomeController : Controller
	{
		private NotesEntities _Context;
		public ActionResult Index()
		{
			return View();
		}

		public HomeController()
		{
			_Context = new NotesEntities();
		}

		[HttpGet]
		public ActionResult SearchNotes()
		{
			var typeList = _Context.Types.ToList();
			var categoryList = _Context.Categories.ToList();
			var universitylist = _Context.SellerNotes.ToList();
			var countrylist = _Context.Countries.ToList();

			ViewBag.TypeList = typeList;
			ViewBag.CategoryList = categoryList;
			ViewBag.UniversityList = universitylist;
			ViewBag.CountryList = countrylist;

			var search = (from s in _Context.SellerNotes
						  let avgratings = (from Review in _Context.SellerNotesReviews
											where Review.NoteID == s.ID
											group Review by Review.NoteID into grp
											select new AvgRatings
											{
												Rating = Math.Round(grp.Average(m => m.Ratings)),
												Total = grp.Count()
											})
						  let totalspam = (from Spam in _Context.SellerNotesReportedIssues
										   where Spam.NoteID == s.ID
										   group Spam by Spam.NoteID into grp
										   select new SpamNote
										   {
											   TotalSpam = grp.Count()
										   })

						  select new SearchNote
						  {
							  Note = s,
							  Total = avgratings.Select(a=>a.Total).FirstOrDefault(),
							  Rating = avgratings.Select(a=>a.Rating).FirstOrDefault(),
							  TotalSpam = totalspam.Select(a=>a.TotalSpam).FirstOrDefault()
						  }).ToList();
			return View(search);
        }

		public ActionResult NoteDetails(int id)
        {
			List<SellerNote> Notes = _Context.SellerNotes.ToList();
			List<SellerNotesReview> reviews = _Context.SellerNotesReviews.ToList();
			List<Country> country = _Context.Countries.ToList();
			List<Category> category = _Context.Categories.ToList();
			List<User> users = _Context.Users.ToList();
			var details = (from s in Notes
						  where (s.ID == id)
						  join c in country on s.Country equals c.ID 
						  join cat in category on s.Category equals cat.ID
						   let ratingandreview =(from r in _Context.SellerNotesReviews where r.NoteID == id
												join u in _Context.Users on r.ReviewedByID equals u.ID
												join up in _Context.UserProfiles on r.ReviewedByID equals up.ID
												select new RatingandReview
                                                {
													IndRating = r.Ratings,
													Review = r.Comments,
													FirstName = u.FirstName,
													LastName = u.LastName,
													ProfilePicture = up.ProfilePicture,
                                                })

						   let avgratings = (from Review in _Context.SellerNotesReviews
											 where Review.NoteID == s.ID
											 group Review by Review.NoteID into grp
											 select new AvgRatings
											 {
												 Rating = Math.Round(grp.Average(m => m.Ratings)),
												 Total = grp.Count()
											 })
						   let totalspam = (from Spam in _Context.SellerNotesReportedIssues
											where Spam.NoteID == s.ID
											group Spam by Spam.NoteID into grp
											select new SpamNote
											{
												TotalSpam = grp.Count()
											})



						   select new NoteDetail 
						  { 
								note = s,
								country = c,
								category = cat,
								//review = r,
								//user = u,

								IndRating = ratingandreview.Select(a=>a.IndRating).FirstOrDefault(),
								Review = ratingandreview.Select(a=>a.Review).FirstOrDefault(),
							    FirstName = ratingandreview.Select(a => a.FirstName).FirstOrDefault(),
							    LastName = ratingandreview.Select(a => a.LastName).FirstOrDefault(),
								ProfilePicture = ratingandreview.Select(a=>a.ProfilePicture).FirstOrDefault(),



							   Total = avgratings.Select(a => a.Total).FirstOrDefault(),
							    Rating = avgratings.Select(a => a.Rating).FirstOrDefault(),
							    TotalSpam = totalspam.Select(a => a.TotalSpam).FirstOrDefault()

						   }).ToList();
			return View(details);
        }

		[HttpGet]
		public ActionResult ContactUs()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ContactUs(ContactUs user)
		{
			if (!ModelState.IsValid)
			{
				return RedirectToAction("ContactUs", "Home");
			}

			var emailto = _Context.Users.Where(m => m.RoleID == 3).FirstOrDefault();

			string body = string.Empty;
			using (StreamReader reader = new StreamReader(Server.MapPath("~/Mail_Template/ContactUs.html")))
			{
				body = reader.ReadToEnd();
			}

			body = body.Replace("{Description}", user.Comments);
			body = body.Replace("{Name}", user.Fullname);

			try
			{
				bool IsSendEmail = SendEmail.EmailSend(emailto.EmailID, user.Fullname + "- Query", body, true);
			}

			catch (Exception e)
			{
				throw e;
			}

			return RedirectToAction("ContactUs", "Home");
		}
	}
}