using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Demo.Models;
using Demo.Utility;

namespace Demo.Controllers
{
    public class UserController : Controller
    {
        private NotesEntities _Context;

        public UserController()
        {
            _Context = new NotesEntities();
        }

        // GET: User

        [Authorize]
        public ActionResult SellYourNotes()
        {
            var userid = Convert.ToInt32(Session["UserId"]);
            var result = (from Notes in _Context.SellerNotes
                         join category in _Context.Categories on Notes.Category equals category.ID
                         where Notes.SellerID == userid
                         let dashboard = (from Download in _Context.Downloads where Download.SellerID == userid  group Download by Download.SellerID into grp 
                                            select new Dashboard
                                            {
                                                Notessold = grp.Count(),
                                                TotalMoney = (decimal)grp.Sum(m=>m.PurchasedPrice)
                                            })

                        let download = (from Download in _Context.Downloads where Download.DownloaderID == userid group Download by Download.DownloaderID into grp
                                        select new MyDownloads
                                        {
                                            Downloads = grp.Count(),
                                        })

                        let rejected = (from Note in _Context.SellerNotes where (Note.SellerID == userid && Note.Status == 1)
                                        group Note by Note.SellerID into grp
                                        select new MyRejected
                                        {
                                            RejectedNotes = grp.Count()
                                        })

                        let buyerreq = (from Download in _Context.Downloads where (Download.SellerID == userid && Download.isSellerHasAllowedDownload == false)
                                        group Download by Download.SellerID into grp
                                        select new BuyerReq
                                        {
                                            Buyerrequests = grp.Count()
                                        })

                         select new SellYourNotes
                         {
                             Note = Notes,
                             category = category,
                             Notessold = dashboard.Select(a=>a.Notessold).FirstOrDefault(),
                             TotalMoney = dashboard.Select(a=>a.TotalMoney).FirstOrDefault(),
                             Downloads = download.Select(a=>a.Downloads).FirstOrDefault(),
                             RejectedNotes = rejected.Select(a=>a.RejectedNotes).FirstOrDefault(),
                             Buyerrequests = buyerreq.Select(a=>a.Buyerrequests).FirstOrDefault(),
                             
                         }).ToList();
                         
                        
            return View(result);
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddNotes()
        {
            var categoryList = _Context.Categories.ToList();
            ViewBag.CategoryList = categoryList;

            var typeList = _Context.Types.ToList();
            ViewBag.TypeList = typeList;

            var countryList = _Context.Countries.ToList();
            ViewBag.CountryList = countryList;
            return View();
        }


        [Authorize]
        [HttpPost]
        public ActionResult Save(AddNoteDetails note)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Login");
            }

            var result = _Context.Set<SellerNote>();

            result.Add(new SellerNote
            {
                SellerID = Convert.ToInt32(Session["UserId"]),
                Title = note.Title,
                CreatedDate = DateTime.Now,
                Status = 1,
                Category = Convert.ToInt32(note.Category),
                //DisplayPicture = note.NotesPicture,
                NoteType = Convert.ToInt32(note.Type),
                NumberofPages = Convert.ToInt32(note.Pages),
                Description = note.Description,
                UniversityName = note.InstitutionName,
                Country = Convert.ToInt32(note.Country),
                IsPaid = Convert.ToBoolean(note.Ispaid),
                SellingPrice = Convert.ToDecimal(note.SellingPrice),
                //NotesPreview = note.NotesPreview,
                isActive = true,
            });
            _Context.SaveChanges();

            

            String member = "~/Members/";
            String Userid = Session["Userid"].ToString() + "/";
            String Noteid = (from D in _Context.SellerNotes orderby D.ID descending select D.ID).FirstOrDefault().ToString() + "/";
            String Name = note.Notespdf;
            string Image = note.NotesPicture;
            
            String Imagepath = member + Userid + Noteid + Image;
            String PDFpath = member + Userid + Noteid + Name;

            var lastid = (from S in _Context.SellerNotes orderby S.ID descending select S.ID).FirstOrDefault();
            var result1 = _Context.SellerNotes.Where(m => m.ID == lastid).FirstOrDefault();
            
                result1.DisplayPicture = Imagepath;
                result1.NotesPreview = PDFpath;

            var result2 = _Context.Set<SellerNotesAttachement>();
            result2.Add(new SellerNotesAttachement
            {
                NoteID = _Context.SellerNotes.Max(m => m.ID),
                FileName = Name,
                FilePath = PDFpath,
                CreatedDate = DateTime.Now,
                isActive = true
            }) ;

            _Context.SaveChanges();
            _Context.Dispose();

            return RedirectToAction("AddNotes","User");
        }


        [Authorize]
        [HttpPost]
        public ActionResult Publish(AddNoteDetails note)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Login");
            }

            var result = _Context.Set<SellerNote>();

            result.Add(new SellerNote
            {
                SellerID = Convert.ToInt32(Session["UserId"]),
                Title = note.Title,
                CreatedDate = DateTime.Now,
                Status = 1003,
                Category = Convert.ToInt32(note.Category),
                //DisplayPicture = note.NotesPicture,
                NoteType = Convert.ToInt32(note.Type),
                NumberofPages = Convert.ToInt32(note.Pages),
                Description = note.Description,
                UniversityName = note.InstitutionName,
                Country = Convert.ToInt32(note.Country),
                IsPaid = Convert.ToBoolean(note.Ispaid),
                SellingPrice = Convert.ToDecimal(note.SellingPrice),
                //NotesPreview = note.NotesPreview,
                isActive = true,
            });
            _Context.SaveChanges();



            String member = "~/Members/";
            String Userid = Session["Userid"].ToString() + "/";
            String Noteid = (from D in _Context.SellerNotes orderby D.ID descending select D.ID).FirstOrDefault().ToString() + "/";
            String Name = note.Notespdf;
            string Image = note.NotesPicture;

            String Imagepath = member + Userid + Noteid + Image;
            String PDFpath = member + Userid + Noteid + Name;

            var lastid = (from S in _Context.SellerNotes orderby S.ID descending select S.ID).FirstOrDefault();
            var result1 = _Context.SellerNotes.Where(m => m.ID == lastid).FirstOrDefault();

            result1.DisplayPicture = Imagepath;
            result1.NotesPreview = PDFpath;

            var result2 = _Context.Set<SellerNotesAttachement>();
            result2.Add(new SellerNotesAttachement
            {
                NoteID = _Context.SellerNotes.Max(m => m.ID),
                FileName = Name,
                FilePath = PDFpath,
                CreatedDate = DateTime.Now,
                isActive = true
            });

            _Context.SaveChanges();
            //_Context.Dispose();

            var currentuser = Convert.ToInt32(Session["UserId"]);
            var emailto = _Context.Users.Where(m => m.RoleID == 3).FirstOrDefault();
            var emailfrom = _Context.Users.Where(m => m.ID == currentuser).FirstOrDefault();

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Mail_Template/PublishNotes.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{SellerName}", emailfrom.FirstName);
            body = body.Replace("{NoteTitle}", note.Title);

            try
            {
                bool IsSendEmail = SendEmail.EmailSend(emailto.EmailID, emailfrom.FirstName + " sent his Notes for Review", body, true);
            }

            catch (Exception e)
            {
                throw e;
            }

            _Context.Dispose();
            return RedirectToAction("AddNotes", "User");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Myprofile()
        {
            var countryList = _Context.Countries.ToList();

            ViewBag.CountryList = countryList;

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Myprofile(MyProfile user)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Login");
            }

            else
            {
                var result1 = _Context.Set<UserProfile>();
                var result3 = Convert.ToInt32(Session["UserId"]);
                String profilepicture = "~/Members/" + result3 + "/" + user.ProfilePicture;
                result1.Add(new UserProfile
                {
                    UserID = Convert.ToInt32(Session["UserId"]),
                    DOB = user.DOB,
                    Gender = Convert.ToInt32(user.Gender),
                    CountryCode = user.CountryCode,
                    PhoneNo = user.PhoneNo,
                    ProfilePicture = profilepicture,
                    AddressLine1 = user.AddLine1,
                    AddressLine2 = user.AddLine2,
                    City = user.City,
                    State = user.State,
                    ZipCode = user.Zipcode,
                    Country = user.Country,
                    University = user.University,
                    College = user.College,
                    CreatedDate = DateTime.Now,
                });

                var result2 = _Context.Users.Where(m => m.ID == result3).FirstOrDefault();
                result2.FirstName = user.FirstName;
                result2.LastName = user.LastName;
                if (result2.EmailID != user.EmailID)
                {
                    result2.isEmailVerified = false;
                    result2.EmailID = user.EmailID;
                    string body = string.Empty;
                    using (StreamReader reader = new StreamReader(Server.MapPath("~/Mail_Template/EmailConfirmation.html")))
                    {
                        body = reader.ReadToEnd();
                    }

                    var callbackUrl = Url.Action("ConfirmEmail", "Registration", new
                    {
                        userId = result2.EmailID
                        ,
                        pass = result2.Password
                    }, protocol: Request.Url.Scheme);

                    body = body.Replace("{Username}", result2.FirstName);
                    body = body.Replace("{ConfirmationLink}", callbackUrl);

                    try
                    {
                        bool IsSendEmail = SendEmail.EmailSend(user.EmailID, "Notes Marketplace - Email Verification", body, true);
                    }

                    catch (Exception e)
                    {
                        throw e;
                    }
                }

                _Context.SaveChanges();
                _Context.Dispose();
                
                return RedirectToAction("Myprofile","User");
            }
        }

        [Authorize]
        public ActionResult BuyerRequests()
        {
            List<Download> downloads = _Context.Downloads.ToList();
            List<SellerNote> notes = _Context.SellerNotes.ToList();
            List<User> user = _Context.Users.ToList();
            List<UserProfile> userprofile = _Context.UserProfiles.ToList();
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var buyerreq = from d in downloads join n in notes on d.NoteID equals n.ID join u in user on d.DownloaderID equals u.ID join profile in userprofile on d.DownloaderID equals profile.UserID  where (d.SellerID == currentuser && d.isSellerHasAllowedDownload == false) select new Buyerrequest { download = d, note = n, user = u, userprofile = profile };
            return View(buyerreq);
        }

        [Authorize]
        public ActionResult AllowDownload(int id)
        {
            var result =  _Context.Downloads.Where(m=>m.ID==id).FirstOrDefault();
            var attachmentpath = _Context.SellerNotesAttachements.Where(m => m.NoteID == result.NoteID).FirstOrDefault();
            result.isSellerHasAllowedDownload = true;
            result.isPaid = true;
            result.AttachmentPath = attachmentpath.FilePath;


            var currentuser = Convert.ToInt32(Session["UserId"]);
            var emailto = _Context.Users.Where(m => m.ID == result.DownloaderID).FirstOrDefault();
            var emailfrom = _Context.Users.Where(m => m.ID == currentuser).FirstOrDefault();

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Mail_Template/AllowDownload.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{SellerName}", emailfrom.FirstName);
            body = body.Replace("{BuyerName}", emailto.FirstName);

            try
            {
                bool IsSendEmail = SendEmail.EmailSend(emailto.EmailID, emailfrom.FirstName + " Allows you to Download a Note", body, true);
            }

            catch (Exception e)
            {
                throw e;
            }
            _Context.SaveChanges();



            return RedirectToAction("BuyerRequests","User");
        }

        [Authorize]
        public ActionResult Download(int noteid)
        {
            var download = _Context.Set<Download>();
            var result = _Context.SellerNotes.Where(m => m.ID == noteid).FirstOrDefault();
            var result1 = _Context.SellerNotesAttachements.Where(m => m.ID == noteid).FirstOrDefault();
            var category = _Context.Categories.Where(m => m.ID == result.Category).FirstOrDefault();
            bool isSellerHasAllowedDownload, IsAttachmentDownloaded;
            string attachmentpath;
            if (result.IsPaid == false)
            {
                isSellerHasAllowedDownload = true;
                IsAttachmentDownloaded = true;
                attachmentpath = result1.FilePath;
            }
            else
            {
                isSellerHasAllowedDownload = false;
                IsAttachmentDownloaded = false;
                attachmentpath = null;
            }

            download.Add(new Download
            {
                NoteID = noteid,
                SellerID = result.SellerID,
                DownloaderID = Convert.ToInt32(Session["UserId"]),
                isSellerHasAllowedDownload = isSellerHasAllowedDownload,
                IsAttachmentDownloaded = IsAttachmentDownloaded,
                isPaid = result.IsPaid,
                PurchasedPrice = Convert.ToInt32(result.SellingPrice),
                NoteTitle = result.Title,
                NoteCategory = category.Name,
                AttachmentPath = attachmentpath
            }) ;
            _Context.SaveChanges();

            if (result.IsPaid == true)
            {
                var currentuser = Convert.ToInt32(Session["UserId"]);
                var emailto = _Context.Users.Where(m => m.ID == result.SellerID).FirstOrDefault();
                var emailfrom = _Context.Users.Where(m => m.ID == currentuser).FirstOrDefault();

                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/Mail_Template/BuyerPaidNotes.html")))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{SellerName}", emailto.FirstName);
                body = body.Replace("{BuyerName}", emailfrom.FirstName);

                try
                {
                    bool IsSendEmail = SendEmail.EmailSend(emailto.EmailID, emailfrom.FirstName + " Wants to Purchase Your notes", body, true);
                }

                catch (Exception e)
                {
                    throw e;
                }
            }
            return RedirectToAction("NoteDetails","Home",new { id = noteid});
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Login");

        }
    
    
    }

}