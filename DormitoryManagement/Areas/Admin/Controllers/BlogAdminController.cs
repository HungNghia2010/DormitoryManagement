using DormitoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DormitoryManagement.Areas.Admin.Controllers
{
    public class BlogAdminController : Controller
    {
        private DormitoryManagementEntities _db = new DormitoryManagementEntities();

        // GET: Admin/Blog
        [RequireLogin]
        public ActionResult Index()
        {
            var Mydata = TempData["success"];
            if (Mydata != null)
            {
                ViewBag.success = Mydata;
            }

            var testdata = TempData["error"];
            if (testdata != null)
            {
                ViewBag.error = testdata;
            }

            var data = _db.Blogs.ToList();

            return View(data);
        }

        [RequireLogin]
        public ActionResult AddBlog()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBlog(Blog blog)
        {
            if (ModelState.IsValid)
            {
                DateTime utcNow = DateTime.UtcNow;
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);
                DateTime vietnamToday = vietnamNow.Date;

                var createtime = vietnamToday.ToString("dd/MM/yyyy");
                blog.CreateDate = createtime;
                _db.Blogs.Add(blog);
                _db.SaveChanges();
                TempData["success"] = "Tạo Blog thành công";
                return RedirectToAction("Index", "BlogAdmin", new { area = "Admin"});

            }

            return View();
        }

        [RequireLogin]
        public ActionResult DeleteBlog(int id)
        {
            var data = _db.Blogs.Find(id);
            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {
                var s = _db.Blogs.Find(id);
                _db.Blogs.Remove(s);
                _db.SaveChanges();
                TempData["success"] = "Xóa thành công";
                return RedirectToAction("Index", "BlogAdmin", new { area = "Admin" });
            }
        }

        [RequireLogin]
        public ActionResult EditBlog(int id)
        {
            var data = _db.Blogs.Find(id);
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBlog(Blog blog)
        {
            if (ModelState.IsValid)
            {
                var data = _db.Blogs.Find(blog.ID);
                data.Title = blog.Title;
                data.Content = blog.Content;
                _db.SaveChanges();
                ViewBag.success = "Cập nhật thành công";
                return View();
            }
            
            return View();
        }

       

    }
}