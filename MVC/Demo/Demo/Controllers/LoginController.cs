using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Demo.Models;
using System.Web.Security;

namespace Demo.Controllers
{
    public class LoginController : Controller
    {
        private NotesEntities _Context;

        public LoginController()
        {
            _Context = new NotesEntities();
        }
        
        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel user)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Login");
            }


            var result1 = _Context.Users.Where(m => m.EmailID == user.EmailID).FirstOrDefault();

            if (result1 != null)
            {
                var obj = _Context.Users.Where(m => m.EmailID == result1.EmailID && m.Password == user.Password).FirstOrDefault();
                if (obj != null)
                {
                    if (obj.isActive == true)
                    {
                        if (obj.isEmailVerified == true)
                        {
                            var result = _Context.UserProfiles.Where(m => m.UserID == obj.ID).FirstOrDefault();
                            Session["UserId"] = obj.ID;

                            FormsAuthentication.SetAuthCookie(obj.ID.ToString(), true);
                            if (result == null)
                            {
                                if (obj.RoleID == 1)
                                {
                                    return RedirectToAction("Myprofile", "User");
                                }
                                else
                                {
                                    return RedirectToAction("Dashboard", "Admin");
                                }
                            }
                            else
                            {
                                if (result.ProfilePicture == null)
                                {
                                    Session["UserProfile"] = "~/Members/SystemConfig/defaultmember.png";
                                }
                                else
                                {
                                    Session["UserProfile"] = result.ProfilePicture;
                                }
                                if (obj.RoleID == 1)
                                {
                                    return RedirectToAction("SearchNotes", "Home");
                                }

                                else
                                {
                                    return RedirectToAction("Dashboard", "Admin");
                                }
                            }
                            /* return RedirectToAction("Index", "Home");*/
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Your Account is Deactivated";
                        return RedirectToAction("Index", "Login");
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Message = "The Password you entered is incorrect";
                    return View();
                }
            }
            else
            {
                ViewBag.Message = "Email doesn't exist";
                return View();
            }
        }
    }
}