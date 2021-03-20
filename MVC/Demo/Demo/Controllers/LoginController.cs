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

            
            var obj = _Context.Users.Where(m => m.EmailID == user.EmailID && m.Password == user.Password).FirstOrDefault();

            if (obj != null)
            {
                if (obj.isActive == true)
                {
                    if (obj.isEmailVerified == true)
                    {
                        var result = _Context.UserProfiles.Where(m => m.UserID == obj.ID).FirstOrDefault();
                        Session["UserId"] = obj.ID;
                        
                        FormsAuthentication.SetAuthCookie(obj.ID.ToString(), false);
                        if (result == null)
                        {
                             return RedirectToAction("Myprofile", "User");
                        }
                        else
                        {
                            Session["UserProfile"] = result.ProfilePicture;
                            if (obj.RoleID == 1)
                            {
                                return RedirectToAction("Index", "Home");
                            }

                            else
                            {
                                return RedirectToAction("Index", "Admin");
                            }
                            
                            
                            
                            
                        }
                        /* return RedirectToAction("Index", "Home");*/
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Message = "Email or Password is Incorrect";
                return View();
            }
        }
    }
}