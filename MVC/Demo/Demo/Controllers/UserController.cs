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
                          orderby Notes.CreatedDate descending
                          where Notes.SellerID == userid
                          let dashboard = (from Download in _Context.Downloads where (Download.SellerID == userid && Download.isSellerHasAllowedDownload == true)  group Download by Download.SellerID into grp 
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

                        let rejected = (from Note in _Context.SellerNotes where (Note.SellerID == userid && Note.Status == 5)
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

        public ActionResult DeleteNotes(int noteid)
        {
            var result1 = _Context.SellerNotesAttachements.Where(m => m.NoteID == noteid).FirstOrDefault();
            var result2 = _Context.SellerNotes.Where(m => m.ID == noteid).FirstOrDefault();
            _Context.SellerNotesAttachements.Remove(result1);
            _Context.SaveChanges();
            _Context.SellerNotes.Remove(result2);
            _Context.SaveChanges();
            return RedirectToAction("SellYourNotes", "User");
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

        [HttpGet]
        public ActionResult EditNotes(int noteid)
        {
            var categoryList = _Context.Categories.ToList();
            ViewBag.CategoryList = categoryList;

            var typeList = _Context.Types.ToList();
            ViewBag.TypeList = typeList;

            var countryList = _Context.Countries.ToList();
            ViewBag.CountryList = countryList;

            var note = _Context.SellerNotes.Where(m => m.ID == noteid).FirstOrDefault();
            var noteattachment = _Context.SellerNotesAttachements.Where(m => m.NoteID == noteid).FirstOrDefault();



            AddNoteDetails notes = new AddNoteDetails
            {
                Title = note.Title,
                Category = note.Category.ToString(),
                NoteType = note.NoteType.ToString(),
                Pages = note.NumberofPages.ToString(),
                Description = note.Description,
                Country = note.Country.ToString(),
                InstitutionName = note.UniversityName,
                CourseName = note.Course,
                CourseCode = note.CourseCode,
                ProfessorName = note.Professor,
                SellingPrice = note.SellingPrice.ToString(),
                noteid = noteid.ToString(),
                Ispaid = note.IsPaid.ToString()
            };


            return View(notes);
        }

        [HttpPost]
        public ActionResult EditNotesSave(AddNoteDetails note)
        {
            var noteid = Convert.ToInt32(note.noteid);
            var result = _Context.SellerNotes.Where(m => m.ID == noteid).FirstOrDefault();
            var result1 = _Context.SellerNotesAttachements.Where(m => m.NoteID == noteid).FirstOrDefault();

            String member = "~/Members/";
            String Userid = Session["Userid"].ToString() + "/";
            String Noteid = noteid + "/";
            String Name = Path.GetFileNameWithoutExtension(note.Notespdf.FileName) + Path.GetExtension(note.Notespdf.FileName);
            string Image = Path.GetFileNameWithoutExtension(note.NotesPicture.FileName) + Path.GetExtension(note.NotesPicture.FileName);
            string Previewpdfpath = Path.GetFileNameWithoutExtension(note.NotesPreview.FileName) + Path.GetExtension(note.NotesPreview.FileName);

            string path = Path.Combine(Server.MapPath("~/Members/" + Session["UserId"].ToString()), Noteid);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (note.NotesPicture != null)
            {
                string filename = Path.GetFileNameWithoutExtension(note.NotesPicture.FileName);
                string extension = Path.GetExtension(note.NotesPicture.FileName);
                filename = filename + extension;
                string finalpath = Path.Combine(path, filename);
                note.NotesPicture.SaveAs(finalpath);
            }

            if (note.Notespdf != null)
            {
                string filename = Path.GetFileNameWithoutExtension(note.Notespdf.FileName);
                string extension = Path.GetExtension(note.Notespdf.FileName);
                filename = filename + extension;
                string finalpath = Path.Combine(path, filename);
                note.Notespdf.SaveAs(finalpath);
            }

            if (note.NotesPreview != null)
            {
                string filename = Path.GetFileNameWithoutExtension(note.NotesPreview.FileName);
                string extension = Path.GetExtension(note.NotesPreview.FileName);
                filename = filename + extension;
                string finalpath = Path.Combine(path, filename);
                note.NotesPreview.SaveAs(finalpath);
            }



            String Imagepath = member + Userid + Noteid + Image;
            String PreviewPDFpath = member + Userid + Noteid + Previewpdfpath;
            String PDFPath = member + Userid + Noteid + Name;

            if (note!=null)
            {
                result.Title = note.Title;
                result.Category = Convert.ToInt32(note.Category);
                result.NoteType = Convert.ToInt32(note.NoteType);
                result.NumberofPages = Convert.ToInt32(note.Pages);
                result.Description = note.Description;
                result.UniversityName = note.InstitutionName;
                result.Country = Convert.ToInt32(note.Country);
                result.Course = note.CourseName;
                result.CourseCode = note.CourseCode;
                result.Professor = note.ProfessorName;
                result.IsPaid = Convert.ToBoolean(note.Ispaid);
                result.SellingPrice = Convert.ToDecimal(note.SellingPrice);
                result.NotesPreview = PreviewPDFpath;
                result.DisplayPicture = Imagepath;
                result1.FilePath = PDFPath;
                result1.FileName = Path.GetFileNameWithoutExtension(note.NotesPreview.FileName) + Path.GetExtension(note.Notespdf.FileName);
                result.ModifiedDate = DateTime.Now;
            }
            _Context.SaveChanges();
            return RedirectToAction("SellYourNotes", "User");
        }

        [HttpPost]
        public ActionResult EditNotesPublish(AddNoteDetails note)
        {
            var noteid = Convert.ToInt32(note.noteid);
            var result = _Context.SellerNotes.Where(m => m.ID == noteid).FirstOrDefault();
            var result1 = _Context.SellerNotesAttachements.Where(m => m.NoteID == noteid).FirstOrDefault();

            String member = "~/Members/";
            String Userid = Session["Userid"].ToString() + "/";
            String Noteid = noteid + "/";
            String Name = Path.GetFileNameWithoutExtension(note.Notespdf.FileName) + Path.GetExtension(note.Notespdf.FileName);
            string Image = Path.GetFileNameWithoutExtension(note.NotesPicture.FileName) + Path.GetExtension(note.NotesPicture.FileName);
            string Previewpdfpath = Path.GetFileNameWithoutExtension(note.NotesPreview.FileName) + Path.GetExtension(note.NotesPreview.FileName);

            string path = Path.Combine(Server.MapPath("~/Members/" + Session["UserId"].ToString()), Noteid);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (note.NotesPicture != null)
            {
                string filename = Path.GetFileNameWithoutExtension(note.NotesPicture.FileName);
                string extension = Path.GetExtension(note.NotesPicture.FileName);
                filename = filename + extension;
                string finalpath = Path.Combine(path, filename);
                note.NotesPicture.SaveAs(finalpath);
            }

            if (note.Notespdf != null)
            {
                string filename = Path.GetFileNameWithoutExtension(note.Notespdf.FileName);
                string extension = Path.GetExtension(note.Notespdf.FileName);
                filename = filename + extension;
                string finalpath = Path.Combine(path, filename);
                note.Notespdf.SaveAs(finalpath);
            }

            if (note.NotesPreview != null)
            {
                string filename = Path.GetFileNameWithoutExtension(note.NotesPreview.FileName);
                string extension = Path.GetExtension(note.NotesPreview.FileName);
                filename = filename + extension;
                string finalpath = Path.Combine(path, filename);
                note.NotesPreview.SaveAs(finalpath);
            }



            String Imagepath = member + Userid + Noteid + Image;
            String PreviewPDFpath = member + Userid + Noteid + Previewpdfpath;
            String PDFPath = member + Userid + Noteid + Name;

            if (note != null)
            {
                result.Title = note.Title;
                result.Category = Convert.ToInt32(note.Category);
                result.NoteType = Convert.ToInt32(note.NoteType);
                result.NumberofPages = Convert.ToInt32(note.Pages);
                result.Description = note.Description;
                result.Status = 2;
                result.UniversityName = note.InstitutionName;
                result.Country = Convert.ToInt32(note.Country);
                result.Course = note.CourseName;
                result.CourseCode = note.CourseCode;
                result.Professor = note.ProfessorName;
                result.IsPaid = Convert.ToBoolean(note.Ispaid);
                result.SellingPrice = Convert.ToDecimal(note.SellingPrice);
                result.NotesPreview = PreviewPDFpath;
                result.DisplayPicture = Imagepath;
                result1.FilePath = PDFPath;
                result1.FileName = Path.GetFileNameWithoutExtension(note.NotesPreview.FileName) + Path.GetExtension(note.Notespdf.FileName);
                result.ModifiedDate = DateTime.Now;
            }
            _Context.SaveChanges();

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

            return RedirectToAction("SellYourNotes", "User");
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
                NoteType = Convert.ToInt32(note.NoteType),
                NumberofPages = Convert.ToInt32(note.Pages),
                Description = note.Description,
                UniversityName = note.InstitutionName,
                Country = Convert.ToInt32(note.Country),
                Course = note.CourseName,
                CourseCode = note.CourseCode,
                Professor = note.ProfessorName,
                IsPaid = Convert.ToBoolean(note.Ispaid),
                SellingPrice = Convert.ToDecimal(note.SellingPrice),
                //NotesPreview = note.NotesPreview,
                isActive = true,
            });
            _Context.SaveChanges();



            String member = "~/Members/";
            String Userid = Session["Userid"].ToString() + "/";
            String Noteid = (from D in _Context.SellerNotes orderby D.ID descending select D.ID).FirstOrDefault().ToString() + "/";
            String Name = Path.GetFileNameWithoutExtension(note.Notespdf.FileName) + Path.GetExtension(note.Notespdf.FileName);
            string Image = Path.GetFileNameWithoutExtension(note.NotesPicture.FileName) + Path.GetExtension(note.NotesPicture.FileName);
            string Previewpdfpath = Path.GetFileNameWithoutExtension(note.NotesPreview.FileName) + Path.GetExtension(note.NotesPreview.FileName);


            string path = Path.Combine(Server.MapPath("~/Members/" + Session["UserId"].ToString()), Noteid);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (note.NotesPicture != null)
            {
                string filename = Path.GetFileNameWithoutExtension(note.NotesPicture.FileName);
                string extension = Path.GetExtension(note.NotesPicture.FileName);
                filename = filename + extension;
                string finalpath = Path.Combine(path, filename);
                note.NotesPicture.SaveAs(finalpath);
            }

            if (note.Notespdf != null)
            {
                string filename = Path.GetFileNameWithoutExtension(note.Notespdf.FileName);
                string extension = Path.GetExtension(note.Notespdf.FileName);
                filename = filename + extension;
                string finalpath = Path.Combine(path, filename);
                note.Notespdf.SaveAs(finalpath);
            }

            if (note.NotesPreview != null)
            {
                string filename = Path.GetFileNameWithoutExtension(note.NotesPreview.FileName);
                string extension = Path.GetExtension(note.NotesPreview.FileName);
                filename = filename + extension;
                string finalpath = Path.Combine(path, filename);
                note.NotesPreview.SaveAs(finalpath);
            }



            String Imagepath = member + Userid + Noteid + Image;
            String PreviewPDFpath = member + Userid + Noteid + Previewpdfpath;
            String PDFPath = member + Userid + Noteid + Name;

            var lastid = (from S in _Context.SellerNotes orderby S.ID descending select S.ID).FirstOrDefault();
            var result1 = _Context.SellerNotes.Where(m => m.ID == lastid).FirstOrDefault();

            result1.DisplayPicture = Imagepath;
            result1.NotesPreview = PreviewPDFpath;

            var result2 = _Context.Set<SellerNotesAttachement>();
            result2.Add(new SellerNotesAttachement
            {
                NoteID = _Context.SellerNotes.Max(m => m.ID),
                FileName = Name,
                FilePath = PDFPath,
                CreatedDate = DateTime.Now,
                isActive = true
            });

            _Context.SaveChanges();
            _Context.Dispose();

            return RedirectToAction("AddNotes", "User");

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
                Status = 2,
                Category = Convert.ToInt32(note.Category),
                //DisplayPicture = note.NotesPicture,
                NoteType = Convert.ToInt32(note.NoteType),
                NumberofPages = Convert.ToInt32(note.Pages),
                Description = note.Description,
                UniversityName = note.InstitutionName,
                Country = Convert.ToInt32(note.Country),
                Course = note.CourseName,
                CourseCode = note.CourseCode,
                Professor = note.ProfessorName,
                IsPaid = Convert.ToBoolean(note.Ispaid),
                SellingPrice = Convert.ToDecimal(note.SellingPrice),
                //NotesPreview = note.NotesPreview,
                isActive = true,
            });
            _Context.SaveChanges();



            String member = "~/Members/";
            String Userid = Session["Userid"].ToString() + "/";
            String Noteid = (from D in _Context.SellerNotes orderby D.ID descending select D.ID).FirstOrDefault().ToString() + "/";

            String Name = Path.GetFileNameWithoutExtension(note.Notespdf.FileName) + Path.GetExtension(note.Notespdf.FileName);
            string Image = Path.GetFileNameWithoutExtension(note.NotesPicture.FileName) + Path.GetExtension(note.NotesPicture.FileName);
            string Previewpdfpath = Path.GetFileNameWithoutExtension(note.NotesPreview.FileName) + Path.GetExtension(note.NotesPreview.FileName);


            string path = Path.Combine(Server.MapPath("~/Members/" + Session["UserId"].ToString()), Noteid);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (note.NotesPicture != null)
            {
                string filename = Path.GetFileNameWithoutExtension(note.NotesPicture.FileName);
                string extension = Path.GetExtension(note.NotesPicture.FileName);
                filename = filename + extension;
                string finalpath = Path.Combine(path, filename);
                note.NotesPicture.SaveAs(finalpath);
            }

            if (note.Notespdf != null)
            {
                string filename = Path.GetFileNameWithoutExtension(note.Notespdf.FileName);
                string extension = Path.GetExtension(note.Notespdf.FileName);
                filename = filename + extension;
                string finalpath = Path.Combine(path, filename);
                note.Notespdf.SaveAs(finalpath);
            }

            if (note.NotesPreview != null)
            {
                string filename = Path.GetFileNameWithoutExtension(note.NotesPreview.FileName);
                string extension = Path.GetExtension(note.NotesPreview.FileName);
                filename = filename + extension;
                string finalpath = Path.Combine(path, filename);
                note.NotesPreview.SaveAs(finalpath);
            }



            String Imagepath = member + Userid + Noteid + Image;
            String PreviewPDFpath = member + Userid + Noteid + Previewpdfpath;
            String PDFPath = member + Userid + Noteid + Name;

            var lastid = (from S in _Context.SellerNotes orderby S.ID descending select S.ID).FirstOrDefault();
            var result1 = _Context.SellerNotes.Where(m => m.ID == lastid).FirstOrDefault();

            result1.DisplayPicture = Imagepath;
            result1.NotesPreview = PreviewPDFpath;

            var result2 = _Context.Set<SellerNotesAttachement>();
            result2.Add(new SellerNotesAttachement
            {
                NoteID = _Context.SellerNotes.Max(m => m.ID),
                FileName = Name,
                FilePath = PDFPath,
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

            var currentuser = (int)Session["UserId"];
            var user = _Context.Users.Where(m => m.ID == currentuser).FirstOrDefault();
            var userprofile = _Context.UserProfiles.Where(m => m.UserID == currentuser).FirstOrDefault();
            if (userprofile != null)
            {
                MyProfile profile = new MyProfile
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailID = user.EmailID,
                    DOB = (DateTime)userprofile.DOB,
                    CountryCode = userprofile.CountryCode,
                    PhoneNo = userprofile.PhoneNo,
                    AddLine1 = userprofile.AddressLine1,
                    AddLine2 = userprofile.AddressLine2,
                    City = userprofile.City,
                    State = userprofile.State,
                    Zipcode = userprofile.ZipCode,
                    Country = userprofile.Country,
                    University = userprofile.University,
                    College = userprofile.College,
                };
                return View(profile);
            }

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
                var currentuser = (int)Session["UserId"];
                var userprofile = _Context.UserProfiles.Where(m => m.UserID == currentuser).FirstOrDefault();
                var user1 = _Context.Users.Where(m => m.ID == currentuser).FirstOrDefault();


                if (userprofile == null)
                {
                    var result1 = _Context.Set<UserProfile>();
                    var result3 = Convert.ToInt32(Session["UserId"]);

                    string path = Path.Combine(Server.MapPath("~/Members/"), Session["UserId"].ToString());

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }



                    string filename = Path.GetFileNameWithoutExtension(user.ProfilePicture.FileName);
                    string extension = Path.GetExtension(user.ProfilePicture.FileName);
                    filename = "DP_" + DateTime.Now.ToString("ddMMyyyy") + extension;
                    string finalpath = Path.Combine(path, filename);
                    user.ProfilePicture.SaveAs(finalpath);







                    String profilepicture = "~/Members/" + result3 + "/" + filename;
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

                }

                else

                {
                    //string filename = Path.GetFileNameWithoutExtension(user.ProfilePicture.FileName);
                    //return Content(filename);
                    string path = Path.Combine(Server.MapPath("~/Members/"), Session["UserId"].ToString());

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }



                    string filename = Path.GetFileNameWithoutExtension(user.ProfilePicture.FileName);
                    string extension = Path.GetExtension(user.ProfilePicture.FileName);
                    filename = "DP_" + DateTime.Now.ToString("ddMMyyyy") + extension;
                    string finalpath = Path.Combine(path, filename);
                    user.ProfilePicture.SaveAs(finalpath);


                    String profilepicture = "~/Members/" + currentuser + "/" + filename;

                    user1.FirstName = user.FirstName;
                    user1.LastName = user.LastName;
                    user1.EmailID = user.EmailID;
                    userprofile.DOB = user.DOB;
                    userprofile.Gender = Convert.ToInt32(user.Gender);
                    userprofile.CountryCode = user.CountryCode;
                    userprofile.PhoneNo = user.PhoneNo;
                    userprofile.ProfilePicture = profilepicture;
                    userprofile.AddressLine1 = user.AddLine1;
                    userprofile.AddressLine2 = user.AddLine2;
                    userprofile.City = user.City;
                    userprofile.State = user.State;
                    userprofile.ZipCode = user.Zipcode;
                    userprofile.Country = user.Country;
                    userprofile.University = user.University;
                    userprofile.College = user.College;

                    _Context.SaveChanges();
                    _Context.Dispose();
                }
            }
                return RedirectToAction("Myprofile", "User");
            }

        [Authorize]
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePassword pwd)
        {
            if (ModelState.IsValid)
            {
                var currentuser = (int)Session["UserId"];
                var result = _Context.Users.Where(m => m.ID == currentuser && m.Password == pwd.oldpwd).FirstOrDefault();
                if (result != null)
                {
                    result.Password = pwd.newpwd;
                    _Context.SaveChanges();
                }
                else
                {
                    ViewBag.message = "Incorrect Old Password";
                }
            }

            return View();
        }

        /*My BuyerRequests*/
        [Authorize]
        public ActionResult BuyerRequests()
        {
            List<Download> downloads = _Context.Downloads.ToList();
            List<SellerNote> notes = _Context.SellerNotes.ToList();
            List<User> user = _Context.Users.ToList();
            List<UserProfile> userprofile = _Context.UserProfiles.ToList();
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var buyerreq = from d in downloads join n in notes on d.NoteID equals n.ID join u in user on d.DownloaderID equals u.ID join profile in userprofile on d.DownloaderID equals profile.UserID  where (d.SellerID == currentuser && d.isSellerHasAllowedDownload == false) orderby d.CreatedDate descending select new Buyerrequest { download = d, note = n, user = u, userprofile = profile };
            return View(buyerreq);
        }


        [Authorize]
        public ActionResult MySoldNotes()
        {
            List<Download> downloads = _Context.Downloads.ToList();
            List<SellerNote> notes = _Context.SellerNotes.ToList();
            List<User> user = _Context.Users.ToList();
            List<UserProfile> userprofile = _Context.UserProfiles.ToList();
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var mysold = from d in downloads join n in notes on d.NoteID equals n.ID join u in user on d.DownloaderID equals u.ID join profile in userprofile on d.DownloaderID equals profile.UserID where (d.SellerID == currentuser && d.isSellerHasAllowedDownload == true) orderby d.CreatedDate descending select new Buyerrequest { download = d, note = n, user = u, userprofile = profile };
            return View(mysold);
        }

        [Authorize]
        public ActionResult MyRejectedNotes()
        {
            List<SellerNote> notes = _Context.SellerNotes.ToList();
            List<Category> category = _Context.Categories.ToList();
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var rejected = from n in notes
                           join c in category on n.Category equals c.ID
                           where (n.SellerID == currentuser && n.Status == 5)
                           select new MyRejectedNotes
                           {
                               note = n,
                               category = c
                           };
            
            return View(rejected);
        }

        [Authorize]
        [HttpGet]
        public ActionResult CloneNotes(int noteid)
        {
            var categoryList = _Context.Categories.ToList();
            ViewBag.CategoryList = categoryList;

            var typeList = _Context.Types.ToList();
            ViewBag.TypeList = typeList;

            var countryList = _Context.Countries.ToList();
            ViewBag.CountryList = countryList;

            var note = _Context.SellerNotes.Where(m => m.ID == noteid).FirstOrDefault();
            var noteattachment = _Context.SellerNotesAttachements.Where(m => m.NoteID == noteid).FirstOrDefault();

            
            
                AddNoteDetails notes = new AddNoteDetails
                {
                    Title = note.Title,
                    Category = note.Category.ToString(),
                    NoteType = note.NoteType.ToString(),
                    Pages = note.NumberofPages.ToString(),
                    Description = note.Description,
                    Country = note.Country.ToString(),
                    InstitutionName = note.UniversityName,
                    CourseName = note.Course,
                    CourseCode = note.CourseCode,
                    ProfessorName = note.Professor,
                    SellingPrice = note.SellingPrice.ToString(),
                };
            

            return View(notes);
        }

        /*My Downloaded Notes*/
        [Authorize] 
        public ActionResult MyDownloads()
        {
            List<Download> downloads = _Context.Downloads.ToList();
            List<SellerNote> notes = _Context.SellerNotes.ToList();
            List<User> user = _Context.Users.ToList();
            List<UserProfile> userprofile = _Context.UserProfiles.ToList();
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var mydownloads = from d in downloads join n in notes on d.NoteID equals n.ID 
                              join u in user on d.SellerID equals u.ID 
                              join profile in userprofile on d.SellerID equals profile.UserID 
                              where (d.DownloaderID == currentuser && d.isSellerHasAllowedDownload == true)
                              orderby d.AttachmentDownloadedDate descending
                              select new Buyerrequest 
                              { download = d, note = n, user = u, userprofile = profile };
            return View(mydownloads);
        }


        /*Allow Download From Buyer Request*/
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(MyDownloadReviews ratingsandreviews)
        {
            //if(!ModelState.IsValid)
            //{
            //    return View("SellYourNotes", "User");
            //}

            //var result = _Context.Set<SellerNotesReview>();
            //var currentuser = Convert.ToInt32(Session["UserId"]);
            //result.Add(new SellerNotesReview
            //{
            //    NoteID = ratingsandreviews.NoteID,
            //    ReviewedByID = currentuser,
            //    AgainstDownloadsID = ratingsandreviews.AgainstDownloadsID,
            //    Ratings = ratingsandreviews.ratings,
            //    Comments = ratingsandreviews.reviews,
            //    isActive = true
            //});
            //_Context.SaveChanges(); 
            //return RedirectToAction("MyDownloads","User");
            return Content(ratingsandreviews.NoteID.ToString());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSpamReport(MyDownloadSpamReport report)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction("MyDownloads", "User");
            }

            var result = _Context.Set<SellerNotesReportedIssue>();
            var currentuser = Convert.ToInt32(Session["UserId"]);
            result.Add(new SellerNotesReportedIssue
            {
                NoteID = report.NoteID,
                ReportedByID = currentuser,
                AgainstDownloadID = report.AgainstDownloadID,
                Remarks = report.remarks,
            });
            _Context.SaveChanges();


            var membername = _Context.Users.Where(m => m.ID == currentuser).FirstOrDefault();
            var sellerid = _Context.Downloads.Where(m => m.NoteID == report.NoteID).FirstOrDefault();
            var sellername = _Context.Users.Where(m => m.ID == sellerid.SellerID).FirstOrDefault();
            var admin = _Context.Users.Where(m => m.RoleID == 3).FirstOrDefault();
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Mail_Template/SpamReport.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{MemberName}",membername.FirstName + membername.LastName );
            body = body.Replace("{SellerName}",sellername.FirstName + sellername.LastName );
            body = body.Replace("{NoteTitle}", sellerid.NoteTitle);

            try
            {
                bool IsSendEmail = SendEmail.EmailSend(admin.EmailID, membername.FirstName+ " " + membername.LastName + " Reported an issue for " + sellerid.NoteTitle, body, true);
            }

            catch (Exception e)
            {
                throw e;
            }
            return RedirectToAction("MyDownloads", "User");
        }

        /*Download Notes from My Downlods*/
        public FileResult DownloadNotes(int noteid)
        {
            var currentuser = Convert.ToInt32(Session["UserId"]);
            var myfile = _Context.SellerNotesAttachements.Where(m => m.NoteID == noteid).FirstOrDefault();
            var result = _Context.Downloads.Where(m => m.NoteID == noteid && m.DownloaderID == currentuser).FirstOrDefault();
            result.isSellerHasAllowedDownload = true;
            result.AttachmentDownloadedDate = DateTime.Now.ToString("dd MMM yyyy, HH:mm:ss");
            _Context.SaveChanges();
            return File(myfile.FilePath, "text/plain", myfile.FileName);
        }

        public FileResult DownloadSoldNotes(int noteid)
        {
            var myfile = _Context.SellerNotesAttachements.Where(m => m.NoteID == noteid).FirstOrDefault();
            return File(myfile.FilePath, "text/plain", myfile.FileName);
        }


        /*Downlod Notes from Note Details Page*/
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
                AttachmentPath = attachmentpath,
                CreatedDate = DateTime.Now.ToString("dd MMM yyyy, HH:mm:ss")

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