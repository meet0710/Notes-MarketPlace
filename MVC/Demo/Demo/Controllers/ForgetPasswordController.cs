using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Demo.Models;
using Demo.Utility;

namespace Demo.Controllers
{
    public class ForgetPasswordController : Controller
    {
        private NotesEntities _Context;

        public ForgetPasswordController()
        {
            _Context = new NotesEntities();
        }

        [HttpGet]
        // GET: ForgetPassword
        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgetPassword(ForgetPasswordModel  user)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ForgetPassword", "ForgetPassword");
            }

            Random r = new Random();
            var changedPassword = r.Next();

            var result = _Context.Users.Where(x => x.EmailID == user.EmailID).FirstOrDefault();
            result.Password = changedPassword.ToString();

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Mail_Template/ForgetPassword.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{Changedpwd}",changedPassword.ToString());

            try
            {
                bool IsSendEmail = SendEmail.EmailSend(user.EmailID, "New Temporary Password has been created for you", body, true);
                _Context.SaveChanges();
                _Context.Dispose();
            }

            catch (Exception e)
            {
                throw e;
            }

            return RedirectToAction("Index","Login");

            
        }

    }
}