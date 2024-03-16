using DormitoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DormitoryManagement.Areas.Admin.Controllers
{
    public class BlogController : Controller
    {
        private DormitoryManagementEntities _db = new DormitoryManagementEntities();

        // GET: Admin/Blog
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddBlog()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBlog(Room room)
        {
            return View();
        }
    }
}