using Demo.Models;
using Demo.Utility;
using System;
using System.Collections.Generic;
using System.IO;
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
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        public ActionResult Index()
        {
            DateTime dt = DateTime.Now.AddDays(-7);
            string date = dt.ToString("dd MMM yyyy, HH:mm:ss");
            var published = (from s in _Context.SellerNotes
                             where s.Status == 4
                             join category in _Context.Categories on s.Category equals category.ID
                             join user in _Context.Users on s.SellerID equals user.ID
                             join attachment in _Context.SellerNotesAttachements on s.ID equals attachment.NoteID
                             let avgratings = (from downloads in _Context.Downloads
                                               where downloads.NoteID == s.ID
                                               group downloads by downloads.NoteID into grp
                                               select new AvgRatings
                                               {

                                                   Total = grp.Count()
                                               })

                             let inreview = (from note in _Context.SellerNotes
                                             where (note.Status == 3 || note.Status == 2)
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

                             let downloads = (from d in _Context.Downloads
                                              where (d.CreatedDate > dt && d.isSellerHasAllowedDownload == true)
                                              group d by d.ID into grp
                                              select new NewDownloads
                                              {
                                                  totalnewdownloads = grp.Count()
                                              })
                             orderby avgratings.Select(a=>a.Total).FirstOrDefault() descending
                             select new AdminDashboard
                             {
                                 Note = s,
                                 noteattachment = attachment,
                                 Total = avgratings.Select(a => a.Total).FirstOrDefault(),
                                 category = category,
                                 user = user,
                                 notesinreview = inreview.Select(a => a.InreviewNotes).FirstOrDefault(),
                                 newregistration = registartion.Select(a=>a.totalnewreg).FirstOrDefault(),
                                 totaldownloads = downloads.Select(a => a.totalnewdownloads).FirstOrDefault()

                             }).ToList();
            return View(published);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        public ActionResult Dashboard()
        {
            DateTime dt = DateTime.Now.AddDays(-7);
            string date = dt.ToString("dd MMM yyyy, HH:mm:ss");
            var published = (from s in _Context.SellerNotes
                             where s.Status == 4
                             join category in _Context.Categories on s.Category equals category.ID
                             join user in _Context.Users on s.SellerID equals user.ID
                             join attachment in _Context.SellerNotesAttachements on s.ID equals attachment.NoteID
                             let avgratings = (from downloads in _Context.Downloads
                                               where downloads.NoteID == s.ID
                                               group downloads by downloads.NoteID into grp
                                               select new AvgRatings
                                               {

                                                   Total = grp.Count()
                                               })

                             let inreview = (from note in _Context.SellerNotes
                                             where (note.Status == 2 || note.Status == 3)
                                             group note by note.Status into grp
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

                             let downloads = (from d in _Context.Downloads
                                              where (d.CreatedDate > dt && d.isSellerHasAllowedDownload == true)
                                              group d by d.ID into grp
                                              select new NewDownloads
                                              {
                                                  totalnewdownloads = grp.Count()
                                              })
                             orderby avgratings.Select(a => a.Total).FirstOrDefault() descending
                             select new AdminDashboard
                             {
                                 Note = s,
                                 noteattachment = attachment,
                                 Total = avgratings.Select(a => a.Total).FirstOrDefault(),
                                 category = category,
                                 user = user,
                                 notesinreview = inreview.Select(a => a.InreviewNotes).FirstOrDefault(),
                                 newregistration = registartion.Select(a => a.totalnewreg).FirstOrDefault(),
                                 totaldownloads = downloads.Select(a => a.totalnewdownloads).FirstOrDefault()

                             }).ToList();
            return View(published);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Unpublish(AdminDashboard note)
        {
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var result = _Context.SellerNotes.Where(m => m.ID == note.noteid).FirstOrDefault();
            result.Status = 5;
            result.AdminRemarks = note.remarks;
            result.ActionedBY = currentuser;
            _Context.SaveChanges();

            var emailto = _Context.Users.Where(m => m.ID == result.SellerID).FirstOrDefault();

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Mail_Template/Unpublishnotes.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{SellerName}", emailto.FirstName + emailto.LastName);
            body = body.Replace("{NoteTitle}", result.Title);

            try
            {
                bool IsSendEmail = SendEmail.EmailSend(emailto.EmailID,"Sorry! We need to remove your notes from our portal.", body, true);
            }

            catch (Exception e)
            {
                throw e;
            }
            return RedirectToAction("Index","Admin");
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Index(AdminDashboard note)
        {
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var unpublish = _Context.SellerNotes.Where(m => m.ID == note.noteid).FirstOrDefault();
            unpublish.Status = 5;
            unpublish.AdminRemarks = note.remarks;
            unpublish.ActionedBY = currentuser;
            _Context.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult AddAdmin()
        {
            var countrylist = _Context.Countries.Where(m=>m.isActive == true).ToList();
            ViewBag.CountryList = countrylist;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult AddAdmin(AddAdmin user)
        {
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var email = _Context.Users.Where(m => m.EmailID == user.Email).FirstOrDefault();
            if (email == null)
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
                    isActive = true,
                    CreatedBy = currentuser,
                    
                    
                });
                _Context.SaveChanges();

                var e = _Context.Users.Where(m => m.EmailID == user.Email).FirstOrDefault();
                var country = _Context.Countries.Where(m => m.CountryCode == user.Code).FirstOrDefault();
                var defaultprofile = _Context.SystemConfigurations.Where(m => m.Key == "ProfilePicture").FirstOrDefault();
                var profile = _Context.Set<UserProfile>();
                profile.Add(new UserProfile
                { 
                    UserID = e.ID,
                    CountryCode = user.Code,
                    PhoneNo = user.Phoneno,
                    AddressLine1 = "NULL",
                    AddressLine2 = "NUll",
                    City = "NULL",
                    State = "NULL",
                    ZipCode = "NULL",
                    ProfilePicture = defaultprofile.Value,
                    Country = country.Name,
                    CreatedBy = currentuser,
                    CreatedDate = DateTime.Now,

                });
                _Context.SaveChanges();
            }
            else
            {
                ViewBag.Message = "Email Already Exists!!";
                return View();
            }
            return RedirectToAction("Index", "Admin");
            
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult AddCategory(AddCategory category)
        {
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var result = _Context.Set<Category>();
            result.Add(new Category
            {
                Name = category.Name,
                Description = category.Description,
                CreatedDate = DateTime.Now,
                isActive = true,
                CreatedBy = currentuser
                
            });
            _Context.SaveChanges();
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult AddType()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult AddType(AddType type)
        {
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var result = _Context.Set<Type>();
            result.Add(new Type
            {
                Name = type.Name,
                Description = type.Description,
                CreatedDate = DateTime.Now,
                isActive = true,
                CreatedBy = currentuser
            });
            _Context.SaveChanges();
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult AddCountry()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult AddCountry(AddCountry country)
        {
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var result = _Context.Set<Country>();
            result.Add(new Country
            {
                Name = country.Name,
                CountryCode = country.CountryCode,
                CreatedDate = DateTime.Now,
                isActive = true,
                CreatedBy = currentuser
            });
            _Context.SaveChanges();
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult AdminDownloads(string id,string noteid)
        {
            var notelist = _Context.SellerNotes.ToList();
            ViewBag.notelist = notelist;
            var sellerandbuyer = _Context.Users.ToList();
            ViewBag.sellerandbuyer = sellerandbuyer;
            if (id != null)
            {
                var kid = Convert.ToInt32(id);
                var sellernames = _Context.Users.ToList();
                ViewBag.UserList = sellernames;
                var inddownloads = (from download in _Context.Downloads
                                    where download.DownloaderID == kid
                                    join notes in _Context.SellerNotes on download.NoteID equals notes.ID

                                    join buyer in _Context.Users on download.DownloaderID equals buyer.ID
                                    join seller in _Context.Users on download.SellerID equals seller.ID
                                    orderby download.CreatedDate descending
                                    select new AdminDownload
                                    {
                                        note = notes,

                                        buyer = buyer,
                                        seller = seller,
                                        download = download
                                    });
                return View(inddownloads);

                
            }
            else if(noteid != null)
            {
                var kid = Convert.ToInt32(noteid);
                var sellernames = _Context.Users.ToList();
                ViewBag.UserList = sellernames;
                var inddownloads = (from download in _Context.Downloads
                                    
                                    join notes in _Context.SellerNotes on download.NoteID equals notes.ID
                                    where download.NoteID == kid
                                    join buyer in _Context.Users on download.DownloaderID equals buyer.ID
                                    join seller in _Context.Users on download.SellerID equals seller.ID
                                    orderby download.CreatedDate descending
                                    select new AdminDownload
                                    {
                                        note = notes,

                                        buyer = buyer,
                                        seller = seller,
                                        download = download
                                    });
                return View(inddownloads);

                

            }
            else
            {
                var sellernames = _Context.Users.ToList();
                ViewBag.UserList = sellernames;
                var downloads = (from download in _Context.Downloads
                                 join notes in _Context.SellerNotes on download.NoteID equals notes.ID

                                 join buyer in _Context.Users on download.DownloaderID equals buyer.ID
                                 join seller in _Context.Users on download.SellerID equals seller.ID
                                 orderby download.CreatedDate descending
                                 select new AdminDownload
                                 {
                                     note = notes,

                                     buyer = buyer,
                                     seller = seller,
                                     download = download
                                 });
                return View(downloads);
            }
        }

        

        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult AdminPublishedNotes(string id)
        {
            var sellername = _Context.Users.Where(m => m.RoleID == 1).ToList();
            ViewBag.sellerlist = sellername;
            if (id == null)
            {
                
                var published = (from s in _Context.SellerNotes
                                 where s.Status == 4
                                 join category in _Context.Categories on s.Category equals category.ID
                                 join user in _Context.Users on s.SellerID equals user.ID
                                 join approver in _Context.Users on s.ActionedBY equals approver.ID
                                 let avgratings = (from downloads in _Context.Downloads
                                                   where downloads.NoteID == s.ID
                                                   group downloads by downloads.NoteID into grp
                                                   select new AvgRatings
                                                   {

                                                       Total = grp.Count()
                                                   })
                                 orderby s.ModifiedDate descending
                                 select new AdminDashboard
                                 {
                                     Note = s,
                                     Total = avgratings.Select(a => a.Total).FirstOrDefault(),
                                     category = category,
                                     user = user,
                                     approver = approver


                                 }).ToList();
                return View(published);
            }
            else
            {
                var kid = Convert.ToInt32(id);
                var publishednotes = (from s in _Context.SellerNotes
                                 where (s.Status == 4 && s.SellerID == kid)
                                 join category in _Context.Categories on s.Category equals category.ID
                                 join user in _Context.Users on s.SellerID equals user.ID
                                 join approver in _Context.Users on s.ModifiedBy equals approver.ID
                                 let avgratings = (from downloads in _Context.Downloads
                                                   where downloads.NoteID == s.ID
                                                   group downloads by downloads.NoteID into grp
                                                   select new AvgRatings
                                                   {

                                                       Total = grp.Count()
                                                   })
                                 orderby s.ModifiedDate descending
                                 select new AdminDashboard
                                 {
                                     Note = s,
                                     Total = avgratings.Select(a => a.Total).FirstOrDefault(),
                                     category = category,
                                     user = user,
                                     approver = approver


                                 }).ToList();
                return View(publishednotes);
            }
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult AdminRejectedNotes()
        {
            var sellerlist = _Context.Users.Where(m => m.RoleID == 1).ToList();
            ViewBag.sellername = sellerlist;
            var rejected = (from note in _Context.SellerNotes
                            where note.Status == 5
                            join category in _Context.Categories on note.Category equals category.ID
                            join user in _Context.Users on note.ActionedBY equals user.ID
                            join seller in _Context.Users on note.SellerID equals seller.ID
                            orderby note.ModifiedDate descending
                            select new AdminRejected
                            {
                                Note = note,
                                Category = category,
                                Seller = seller,
                                Admin = user,
                            }).ToList();
            return View(rejected);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult AdminSpamReports()
        {
            var spam = (from spamreport in _Context.SellerNotesReportedIssues
                        join note in _Context.SellerNotes on spamreport.NoteID equals note.ID
                        join category in _Context.Categories on note.Category equals category.ID
                        join user in _Context.Users on spamreport.ReportedByID equals user.ID
                        orderby spamreport.CreatedDate descending
                        select new AdminSpamReport
                        {
                            Note = note,
                            Category = category,
                            user = user,
                            spamreport = spamreport
                        });
            return View(spam);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult DeleteSpamReport(int id)
        {
            var result = _Context.SellerNotesReportedIssues.Where(m => m.ID == id).FirstOrDefault();
            _Context.SellerNotesReportedIssues.Remove(result);
            _Context.SaveChanges();
            return RedirectToAction("AdminSpamReports", "Admin");
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
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
                            orderby user.CreatedDate descending

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

        [Authorize(Roles = "SuperAdmin,Admin")]
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

                                        let downloads = (from download in _Context.Downloads
                                                         where download.NoteID == note.ID
                                                         group download by download.NoteID into grp
                                                         select new notedownload
                                                         { down = grp.Count() }).ToList()

                                        let earnings = (from download in _Context.Downloads
                                                         where download.NoteID == note.ID
                                                         group download by download.NoteID into grp
                                                         select new notesearning
                                                         { earning = grp.Sum(m=>(int)m.PurchasedPrice) }).ToList()

                                        select new membernotes
                                        {
                                            NoteID = note.ID,
                                            NoteTitle = note.Title,
                                            category = category.Name,
                                            status = status.Name,
                                            downloads = downloads.Select(m=>m.down).FirstOrDefault(),
                                            earning = earnings.Select(m=>m.earning).FirstOrDefault(),
                                            AddedDate = (DateTime)note.CreatedDate,
                                            PublishedDate = (DateTime)note.ModifiedDate
                                        }).ToList()

                           //let downloads = (
                           //                 from download in _Context.Downloads
                           //                 where (download.SellerID == memberid && download.isSellerHasAllowedDownload == true)
                           //                 group download by download.NoteID into grp
                           //                 select new DownloadedNotes
                           //                 {
                           //                     totaldownloads = grp.Count()
                           //                 }).ToList()

                           //let downloads = (from note in _Context.SellerNotes
                           //                 where note.SellerID == memberid
                           //                 let inddownload = (from download in _Context.Downloads
                           //                                    where (download.NoteID == note.ID && download.isSellerHasAllowedDownload == true)
                           //                                    group download by download.NoteID into grp
                           //                                    select new notesearning
                           //                                    { down = grp.Count() })
                           //                 select new DownloadedNotes
                           //                 {
                           //                     totaldownloads = inddownload.Select(a => a.down).FirstOrDefault()
                           //                 }).ToList()

                           select new MemberDetails
                           {
                               user = user,
                               up = userprofile,
                               Note = notes,
                               //downloads = downloads,
                               
                           }).FirstOrDefault();

            
            return View(details);
        }


        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Deactivate(int id)
        {
            var result = _Context.Users.Where(m => m.ID == id).FirstOrDefault();
            result.isActive = false;

            var sn = (from note in _Context.SellerNotes where note.SellerID == id select note);
            foreach (var notes in sn)
            {
                notes.Status = 6;
            }

            _Context.SaveChanges();
            return RedirectToAction("AdminMembers", "Admin");
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult AdminNotesUnderReview(string id)
        {
            var seller = _Context.Users.Where(m => m.RoleID == 1).ToList();
            ViewBag.sellername = seller;
            if (id == null)
            {
                var notesunderreview = (from note in _Context.SellerNotes
                                        where (note.Status == 2 || note.Status == 3)
                                        join category in _Context.Categories on note.Category equals category.ID
                                        join status in _Context.NoteStatus on note.Status equals status.ID
                                        join user in _Context.Users on note.SellerID equals user.ID
                                        orderby note.CreatedDate ascending
                                        select new AdminNotesUnderReview
                                        {
                                            user = user,
                                            note = note,
                                            category = category,
                                            status = status
                                        }).ToList();
                return View(notesunderreview);
            }
            else
            {
                var kid = Convert.ToInt32(id);
                var notes = (from note in _Context.SellerNotes
                                        where (note.SellerID == kid && (note.Status == 2 || note.Status == 3))
                                        join category in _Context.Categories on note.Category equals category.ID
                                        join status in _Context.NoteStatus on note.Status equals status.ID
                                        join user in _Context.Users on note.SellerID equals user.ID
                                        orderby note.CreatedDate ascending
                                        select new AdminNotesUnderReview
                                        {
                                            user = user,
                                            note = note,
                                            category = category,
                                            status = status
                                        }).ToList();
                return View(notes);

            }

        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult ApproveNotes(int id)
        {
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var result = _Context.SellerNotes.Where(m => m.ID == id).FirstOrDefault();
            result.Status = 4;
            result.AdminRemarks = null;
            result.ActionedBY = currentuser;
            result.ModifiedBy = currentuser;
            result.PublishedDate = DateTime.Now;
            _Context.SaveChanges();
            return RedirectToAction("AdminNotesUnderReview", "Admin");
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult RejectNotes(AdminNotesUnderReview note)
        {
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var result = _Context.SellerNotes.Where(m => m.ID == note.Noteid).FirstOrDefault();
            result.Status = 5;
            result.AdminRemarks = note.remarks;
            result.ActionedBY = currentuser;
            _Context.SaveChanges();
            return RedirectToAction("AdminNotesUnderReview", "Admin");
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult InReviewNotes(int id)
        {
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var result = _Context.SellerNotes.Where(m => m.ID == id).FirstOrDefault();
            result.Status = 3;
            result.ActionedBY = currentuser;
            _Context.SaveChanges();
            return RedirectToAction("AdminNotesUnderReview", "Admin");
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult ManageSystem()
        {
            return View();
        }

        [Authorize (Roles = "SuperAdmin")]
        [HttpPost]
        public ActionResult ManageSystem(managesystem system)
        {
            var result = _Context.Set<SystemConfiguration>();
            if(system.supportemail != null)
            {
                result.Add(new SystemConfiguration
                {
                       Key = "SupportEmail",
                       Value = system.supportemail,
                       isActive = true
                });
            }

            if (system.supportphoneno != null)
            {
                result.Add(new SystemConfiguration
                {
                    Key = "SupportPhone",
                    Value = system.supportphoneno,
                    isActive = true,
                });
            }

            if (system.email != null)
            {
                result.Add(new SystemConfiguration
                {
                    Key = "Email",
                    Value = system.email,
                    isActive = true
                });
            }

            if (system.fburl != null)
            {
                result.Add(new SystemConfiguration
                {
                    Key = "FacebookURL",
                    Value = system.fburl,
                    isActive = true,
                });
            }

            if (system.twitterurl != null)
            {
                result.Add(new SystemConfiguration
                {
                    Key = "TwitterURL",
                    Value = system.twitterurl,
                    isActive = true
                });
            }

            if (system.linkedinurl != null)
            {
                result.Add(new SystemConfiguration
                {
                    Key = "LinkedinURL",
                    Value = system.linkedinurl,
                    isActive = true
                });
            }

            if (system.defaultnote != null)
            {
                string path = Path.Combine(Server.MapPath("~/Members/"), Server.MapPath("SystemConfig"));

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }


                string filename = Path.GetFileNameWithoutExtension(system.defaultnote.FileName);
                string extension = Path.GetExtension(system.defaultnote.FileName);
                filename = "deafultnotes" + extension;
                string finalpath = Path.Combine(path, filename);
                system.defaultnote.SaveAs(finalpath);

                result.Add(new SystemConfiguration
                {
                    Key = "NotesPicture",
                    Value = "~/Members/SystemConfig/" + filename,
                    isActive = true
                }) ;
            }

            if (system.defaultprofile != null)
            {
                string path = Path.Combine(Server.MapPath("~/Members/"), Server.MapPath("SystemConfig"));

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }


                string filename = Path.GetFileNameWithoutExtension(system.defaultprofile.FileName);
                string extension = Path.GetExtension(system.defaultprofile.FileName);
                filename = "deafultmember" + extension;
                string finalpath = Path.Combine(path, filename);
                system.defaultprofile.SaveAs(finalpath);

                result.Add(new SystemConfiguration
                {
                    Key = "ProfilePicture",
                    Value = "~/Members/SystemConfig/" + filename,
                    isActive = true
                }) ;
            }

            _Context.SaveChanges();

            return View();
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult ManageAdmin()
        {
            var admins = (from user in _Context.Users
                          where (user.RoleID == 2)
                          join userprofile in _Context.UserProfiles on user.ID equals userprofile.UserID
                          orderby user.CreatedDate descending
                          select new ManageAdmin
                          {
                              user = user,
                              userprofile = userprofile
                          });
            
            return View(admins);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult EditAdmin(int id)
        {
            var countrylist = _Context.Countries.Where(m=>m.isActive == true).ToList();
            ViewBag.CountryList = countrylist;
            var editadmin = _Context.Users.Where(m => m.ID == id && m.RoleID == 2).FirstOrDefault();
            var editadminprofile = _Context.UserProfiles.Where(m => m.UserID == id).FirstOrDefault();
            AddAdmin admin = new AddAdmin
            {
                id = id,
                FirstName = editadmin.FirstName,
                LastName = editadmin.LastName,
                Email = editadmin.EmailID,
                Code = editadminprofile.CountryCode,
                Phoneno = editadminprofile.PhoneNo,
                
            };
            return View(admin);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult EditAdmin(AddAdmin user)
        {
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var admin = _Context.Users.Where(m => m.ID == user.id).FirstOrDefault();
            var adminprofile = _Context.UserProfiles.Where(m => m.UserID == user.id).FirstOrDefault();
            admin.FirstName = user.FirstName;
            admin.LastName = user.LastName;
            admin.EmailID = user.Email;
            adminprofile.CountryCode = user.Code;
            adminprofile.PhoneNo = user.Phoneno;
            admin.ModifiedBy = currentuser;
            admin.ModifiedDate = DateTime.Now;
            adminprofile.ModifiedBy = currentuser;
            adminprofile.ModifiedDate = DateTime.Now;
            _Context.SaveChanges();
            return RedirectToAction("ManageAdmin", "Admin");
        }

        [Authorize(Roles = "SuperAdmin")]
        public ActionResult DeleteAdmin(int id)
        {

            var currentuser = Convert.ToInt32(Session["UserId"]);
            var admin = _Context.Users.Where(m => m.ID == id).FirstOrDefault();
            admin.isActive = false;
            admin.ModifiedBy = currentuser;
            admin.ModifiedDate = DateTime.Now;
            _Context.SaveChanges();
            return RedirectToAction("ManageAdmin", "Admin");
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult ManageCategory()
        {
            var categories = from category in _Context.Categories
                             join user in _Context.Users on category.CreatedBy equals user.ID
                             orderby category.CreatedDate descending
                             select new ManageCategory
                             {
                                 category = category,
                                 user = user
                              } ;
            return View(categories);

        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult EditCategory(int id)
        {
            var editcategory = _Context.Categories.Where(m => m.ID == id).FirstOrDefault();
            AddCategory category = new AddCategory
            {
                id = id,
                Name = editcategory.Name,
                Description = editcategory.Description
            };
            return View(category);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult EditCategory(AddCategory category)
        {
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var editcategory = _Context.Categories.Where(m => m.ID == category.id).FirstOrDefault();
            editcategory.Name = category.Name;
            editcategory.Description = category.Description;
            editcategory.ModifiedBy = currentuser;
            editcategory.ModifedDate = DateTime.Now;
            _Context.SaveChanges();
            return RedirectToAction("ManageCategory", "Admin");
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult DeleteCategory(int id)
        {
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var category = _Context.Categories.Where(m => m.ID == id).FirstOrDefault();
            category.isActive = false;
            category.ModifiedBy = currentuser;
            category.ModifedDate = DateTime.Now;
            _Context.SaveChanges();
            return RedirectToAction("ManageCategory", "Admin");
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult ManageCountry()
        {
            var countries = from country in _Context.Countries
                            join user in _Context.Users on country.CreatedBy equals user.ID
                            orderby country.CreatedDate descending
                             select new ManageCountry
                             {
                                 country = country,
                                 user = user
                             };
            
            return View(countries);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult EditCountry(int id)
        {
            var editcountry = _Context.Countries.Where(m => m.ID == id).FirstOrDefault();
            AddCountry country = new AddCountry
            {
                id = id,
                Name = editcountry.Name,
                CountryCode = editcountry.CountryCode
            };
            return View(country);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult EditCountry(AddCountry country)
        {
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var editcountry = _Context.Countries.Where(m => m.ID == country.id).FirstOrDefault();
            editcountry.Name = country.Name;
            editcountry.CountryCode = country.CountryCode;
            editcountry.ModifiedBy = currentuser;
            editcountry.ModifiedDate = DateTime.Now;
            _Context.SaveChanges();
            return RedirectToAction("ManageCountry", "Admin");
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult DeleteCountry(int id)
        {
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var country = _Context.Countries.Where(m => m.ID == id).FirstOrDefault();
            country.isActive = false;
            country.ModifiedBy = currentuser;
            country.ModifiedDate = DateTime.Now;
            _Context.SaveChanges();
            return RedirectToAction("ManageCountry", "Admin");
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult ManageType()
        {
            var types = from type in _Context.Types
                        join user in _Context.Users on type.CreatedBy equals user.ID
                        orderby type.CreatedDate descending
                            select new ManageType
                            {
                                type = type,
                                user = user
                            };
            return View(types);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult EditType(int id)
        {
            var edittype = _Context.Types.Where(m => m.ID == id).FirstOrDefault();
            AddType type = new AddType
            {
                id = id,
                Name = edittype.Name,
                Description = edittype.Description
            };
            return View(type);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult EditType(AddType type)
        {
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var edittype = _Context.Types.Where(m => m.ID == type.id).FirstOrDefault();
            edittype.Name = type.Name;
            edittype.Description = type.Description;
            edittype.ModifiedBy = currentuser;
            edittype.ModifiedDate = DateTime.Now;
            _Context.SaveChanges();
            return RedirectToAction("ManageType", "Admin");
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult DeleteType(int id)
        {
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var type = _Context.Types.Where(m => m.ID == id).FirstOrDefault();
            type.isActive = false;
            type.ModifiedBy = currentuser;
            type.ModifiedDate = DateTime.Now;
            _Context.SaveChanges();
            return RedirectToAction("ManageType", "Admin");
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult AdminProfile()
        {
            var countrylist = _Context.Countries.Where(m => m.isActive == true).ToList();
            ViewBag.CountryList = countrylist;
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var admin = _Context.Users.Where(m => m.ID == currentuser).FirstOrDefault();
            var adminprofile = _Context.UserProfiles.Where(m => m.UserID == currentuser).FirstOrDefault();
            AdminProfile user = new AdminProfile
            {
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Email = admin.EmailID,
                code = adminprofile.CountryCode,
                Phoneno = adminprofile.PhoneNo,
                Secondaryemail = adminprofile.SecondaryEmailAddress
            };
            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult AdminProfile(AdminProfile user)
        {
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var admin = _Context.Users.Where(m => m.ID == currentuser).FirstOrDefault();
            var adminprofile = _Context.UserProfiles.Where(m => m.UserID == currentuser).FirstOrDefault();

            string path = Path.Combine(Server.MapPath("~/Members/"), Session["UserId"].ToString());

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }



            string filename = Path.GetFileNameWithoutExtension(user.ProfilePicture.FileName);
            string extension = Path.GetExtension(user.ProfilePicture.FileName);
            filename = filename + extension;
            string finalpath = Path.Combine(path, filename);
            user.ProfilePicture.SaveAs(finalpath);







            String profilepicture = "~/Members/" + currentuser + "/" + filename;

            admin.FirstName = user.FirstName;
            admin.LastName = user.LastName;
            admin.EmailID = user.Email;
            admin.ModifiedBy = currentuser;
            admin.ModifiedDate = DateTime.Now;

            adminprofile.SecondaryEmailAddress = user.Secondaryemail;
            adminprofile.CountryCode = user.code;
            adminprofile.PhoneNo = user.Phoneno;
            adminprofile.ProfilePicture = profilepicture;
            adminprofile.ModifiedBy = currentuser;
            adminprofile.ModifiedDate = DateTime.Now;

            _Context.SaveChanges();
            return RedirectToAction("AdminProfile","Admin");
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult DeleteReview (int id)
        {
            var result = _Context.SellerNotesReviews.Where(m => m.ID == id).FirstOrDefault();
            _Context.SellerNotesReviews.Remove(result);
            _Context.SaveChanges();
            return RedirectToAction("SearchNotes", "Home");
        }
    }
}