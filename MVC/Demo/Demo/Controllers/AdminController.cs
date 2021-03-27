using Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Demo.Controllers
{
    public class AdminController : Controller
    {
        private NotesEntities _Context;

        public AdminController()
        {
            _Context = new NotesEntities();
        }


        // GET: Admin
        //[Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Index()
        {
            DateTime dt = DateTime.Now.AddDays(-7);
            var published = (from s in _Context.SellerNotes
                             where s.Status == 4
                             join category in _Context.Categories on s.Category equals category.ID
                             join user in _Context.Users on s.SellerID equals user.ID
                             let avgratings = (from downloads in _Context.Downloads
                                               where downloads.NoteID == s.ID
                                               group downloads by downloads.NoteID into grp
                                               select new AvgRatings
                                               {

                                                   Total = grp.Count()
                                               })

                             let inreview = (from note in _Context.SellerNotes
                                             where note.Status == 3
                                             group note by note.ID into grp
                                             select new InReview
                                             {
                                                 InreviewNotes = grp.Count()
                                             })

                             let registartion = (from u in _Context.Users
                                                 where u.CreatedDate > dt
                                                 group u by u.ID into grp
                                                 select new NewRegistration
                                                 {
                                                     totalnewreg = grp.Count()
                                                 })

                             select new AdminDashboard
                             {
                                 Note = s,
                                 Total = avgratings.Select(a => a.Total).FirstOrDefault(),
                                 category = category,
                                 user = user,
                                 notesinreview = inreview.Select(a => a.InreviewNotes).FirstOrDefault(),
                                 newregistration = registartion.Select(a=>a.totalnewreg).FirstOrDefault()

                             }).ToList();
            return View(published);
        }

        [HttpPost]
        public ActionResult Unpublish(AdminDashboard note)
        {
            var result = _Context.SellerNotes.Where(m => m.ID == note.noteid).FirstOrDefault();
            result.Status = 5;
            result.AdminRemarks = note.remarks;
            _Context.SaveChanges();
            return RedirectToAction("Index","Admin");
        }

        [HttpPost]
        public ActionResult Index(AdminDashboard note)
        {
            var unpublish = _Context.SellerNotes.Where(m => m.ID == note.noteid).FirstOrDefault();
            unpublish.Status = 5;
            unpublish.AdminRemarks = note.remarks;
            _Context.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public ActionResult AddAdmin()
        {
            var countrylist = _Context.Countries.ToList();
            ViewBag.CountryList = countrylist;
            return View();
        }

        [HttpPost]
        public ActionResult AddAdmin(AddAdmin user)
        {
            var register = _Context.Set<User>();
            register.Add(new User
            {
                RoleID = 2,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailID = user.Email,
                Password = "admin",
                isEmailVerified = true,
                CreatedDate = DateTime.Now,
                isActive = true
            });

            _Context.SaveChanges();
            return RedirectToAction("Index","Admin");
        }

        [HttpGet]
        public ActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCategory(AddCategory category)
        {
            var result = _Context.Set<Category>();
            result.Add(new Category
            {
                Name = category.Name,
                Description = category.Description,
                CreatedDate = DateTime.Now,
                isActive = true
                
            });
            _Context.SaveChanges();
            return View();
        }

        [HttpGet]
        public ActionResult AddType()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddType(AddType type)
        {
            var result = _Context.Set<Type>();
            result.Add(new Type
            {
                Name = type.Name,
                Description = type.Description,
                CreatedDate = DateTime.Now,
                isActive = true
            });
            _Context.SaveChanges();
            return View();
        }

        [HttpGet]
        public ActionResult AddCountry()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCountry(AddCountry country)
        {
            var result = _Context.Set<Country>();
            result.Add(new Country
            {
                Name = country.Name,
                CountryCode = country.CountryCode,
                CreatedDate = DateTime.Now,
                isActive = true
            });
            _Context.SaveChanges();
            return View();
        }

        [HttpGet]
        public ActionResult AdminDownloads()
        {
            var downloads = (from download in _Context.Downloads
                             join notes in _Context.SellerNotes on download.NoteID equals notes.ID
                             
                             join buyer in _Context.Users on download.DownloaderID equals buyer.ID
                             join seller in _Context.Users on download.SellerID equals seller.ID
                             select new AdminDownload
                             {
                                 note = notes,
                                 
                                 buyer = buyer,
                                 seller = seller,
                                 download = download
                             });
            return View(downloads);
        }

        public ActionResult AdminPublishedNotes()
        {
            var published = (from s in _Context.SellerNotes
                             where s.Status == 4
                             join category in _Context.Categories on s.Category equals category.ID
                             join user in _Context.Users on s.SellerID equals user.ID
                             let avgratings = (from downloads in _Context.Downloads
                                               where downloads.NoteID == s.ID
                                               group downloads by downloads.NoteID into grp
                                               select new AvgRatings
                                               {

                                                   Total = grp.Count()
                                               })

                             select new AdminDashboard
                             {
                                 Note = s,
                                 Total = avgratings.Select(a => a.Total).FirstOrDefault(),
                                 category = category,
                                 user = user,
                                 
                                 
                             }).ToList();
            return View(published);
        }

        public ActionResult AdminRejectedNotes()
        {
            var rejected = (from note in _Context.SellerNotes
                            where note.Status == 5
                            join category in _Context.Categories on note.Category equals category.ID
                            join user in _Context.Users on note.ActionedBY equals user.ID
                            join seller in _Context.Users on note.SellerID equals seller.ID
                            select new AdminRejected
                            {
                                Note = note,
                                Category = category,
                                Seller = seller,
                                Admin = user,
                            }).ToList();
            return View(rejected);
        }

        public ActionResult AdminSpamReports()
        {
            var spam = (from spamreport in _Context.SellerNotesReportedIssues
                        join note in _Context.SellerNotes on spamreport.NoteID equals note.ID
                        join category in _Context.Categories on note.Category equals category.ID
                        join user in _Context.Users on spamreport.ReportedByID equals user.ID
                        select new AdminSpamReport
                        {
                            Note = note,
                            Category = category,
                            user = user,
                            spamreport = spamreport
                        });
            return View(spam);
        }

        public ActionResult DeleteSpamReport(int id)
        {
            var result = _Context.SellerNotesReportedIssues.Where(m => m.ID == id).FirstOrDefault();
            _Context.SellerNotesReportedIssues.Remove(result);
            _Context.SaveChanges();
            return RedirectToAction("AdminSpamReports", "Admin");
        }

        public ActionResult AdminMembers()
        {
            var members = (from user in _Context.Users
                           where user.RoleID == 1

                           let underreviewnotes = (from underreview in _Context.SellerNotes
                                                   where (underreview.Status == 3 && underreview.SellerID == user.ID)
                                                   group underreview by underreview.SellerID into grp
                                                   select new InReview
                                                   {
                                                       InreviewNotes = grp.Count()
                                                   })

                           let published = (from underreview in _Context.SellerNotes
                                                   where (underreview.Status == 4 && underreview.SellerID == user.ID)
                                                   group underreview by underreview.SellerID into grp
                                                   select new PublishedNotes
                                                   {
                                                       totalpublishednotes = grp.Count()
                                                   })

                           let downloads = (from download in _Context.Downloads
                                            where (download.DownloaderID == user.ID && download.isSellerHasAllowedDownload == true)
                                            group download by download.DownloaderID into grp
                                            select new DownloadedNotes
                                            {
                                                totaldownloads = grp.Count()
                                            })

                           let expense = (from download in _Context.Downloads
                                            where (download.DownloaderID == user.ID && download.isSellerHasAllowedDownload == true)
                                            group download by download.DownloaderID into grp
                                            select new Expense
                                            {
                                                totalexpense = (int)grp.Sum(m=>m.PurchasedPrice)
                                            })

                           let earning = (from download in _Context.Downloads
                                          where (download.SellerID == user.ID && download.isSellerHasAllowedDownload == true)
                                          group download by download.SellerID into grp
                                          select new TotalEarning
                                          {
                                              totalearning = (int)grp.Sum(m => m.PurchasedPrice)
                                          })


                           select new AdminMember
                           {
                               user = user,
                               underreviewnotes = underreviewnotes.Select(m=>m.InreviewNotes).FirstOrDefault(),
                               totalpublishednotes = published.Select(m=>m.totalpublishednotes).FirstOrDefault(),
                               totaldownloads = downloads.Select(m=>m.totaldownloads).FirstOrDefault(),
                               totalexpense = expense.Select(m=>m.totalexpense).FirstOrDefault(),
                               totalearning = earning.Select(m=>m.totalearning).FirstOrDefault()
                           }).ToList();
                                                   
            return View(members);
        }

        public ActionResult AdminMemberDetails(int memberid)
        {
            List<SellerNote> notes1 = _Context.SellerNotes.ToList();
            var details = (from user in _Context.Users
                           where user.ID == memberid && user.RoleID == 1
                           join userprofile in _Context.UserProfiles on user.ID equals userprofile.UserID

                           let notes = (from note in _Context.SellerNotes
                                        where note.SellerID == memberid
                                        join category in _Context.Categories on note.Category equals category.ID
                                        join status in _Context.NoteStatus on note.Status equals status.ID
                                        
                                        select new membernotes
                                        {
                                            NoteID = note.ID,
                                            NoteTitle = note.Title,
                                            category = category.Name,
                                            status = status.Name
                                            
                                        }).ToList()

                           let downloads = (
                                            from download in _Context.Downloads
                                            where (download.SellerID == memberid && download.isSellerHasAllowedDownload == true)
                                            group download by download.NoteID  into grp
                                            select new DownloadedNotes
                                            {
                                                totaldownloads = grp.Count()
                                            }).ToList()
                                            
                           select new MemberDetails
                           {
                               user = user,
                               up = userprofile,
                               Note = notes,
                               downloads = downloads
                           }).FirstOrDefault();

            
            return View(details);
        }

        public ActionResult AdminNotesUnderReview()
        {
            var notesunderreview = (from note in _Context.SellerNotes
                                    where (note.Status == 2 || note.Status == 3)
                                    join category in _Context.Categories on note.Category equals category.ID
                                    join status in _Context.NoteStatus on note.Status equals status.ID
                                    join user in _Context.Users on note.SellerID equals user.ID
                                    select new AdminNotesUnderReview
                                    {
                                        user = user,
                                        note = note,
                                        category = category,
                                        status = status
                                    }).ToList();
            return View(notesunderreview);
        }

        public ActionResult ApproveNotes(int id)
        {
            var result = _Context.SellerNotes.Where(m => m.ID == id).FirstOrDefault();
            result.Status = 4;
            result.AdminRemarks = null;
            _Context.SaveChanges();
            return RedirectToAction("AdminNotesUnderReview", "Admin");
        }

        [HttpPost]
        public ActionResult RejectNotes(AdminNotesUnderReview note)
        {
            var result = _Context.SellerNotes.Where(m => m.ID == note.Noteid).FirstOrDefault();
            result.Status = 5;
            result.AdminRemarks = note.remarks;
            _Context.SaveChanges();
            return RedirectToAction("AdminNotesUnderReview", "Admin");
        }

        public ActionResult InReviewNotes(int id)
        {
            var result = _Context.SellerNotes.Where(m => m.ID == id).FirstOrDefault();
            result.Status = 3;
            _Context.SaveChanges();
            return RedirectToAction("AdminNotesUnderReview", "Admin");
        }
    }
}