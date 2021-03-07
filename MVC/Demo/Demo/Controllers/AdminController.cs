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
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Index()
        {
            return View();
        }
    }
}