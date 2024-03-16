using DormitoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DormitoryManagement.Controllers
{
    public class BlogController : Controller
    {
        private DormitoryManagementEntities _db = new DormitoryManagementEntities();

        // GET: Blog
        [RequireLogin]
        public ActionResult Index()
        {
            var data = _db.Blogs.ToList();
            return View(data);
        }

        [RequireLogin]
        public ActionResult Detail(int id)
        {
            var data = _db.Blogs.Find(id);
            ViewBag.Tit = data.Title;
            ViewBag.Content = data.Content;
            ViewBag.CreateDate = data.CreateDate;
            return View();
        }
    }
}