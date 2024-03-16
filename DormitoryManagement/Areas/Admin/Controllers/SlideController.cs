using DormitoryManagement.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DormitoryManagement.Areas.Admin.Controllers
{
    public class SlideController : Controller
    {
        private DormitoryManagementEntities _db = new DormitoryManagementEntities();
        // GET: Admin/Slide
        [RequireLogin]
        public ActionResult Index()
        {
            var Mydata = TempData["success"];
            var Mydataerror = TempData["error"];
            
            if(Mydata != null)
            {
                ViewBag.success = Mydata;
            }
            if(Mydataerror != null)
            {
                ViewBag.error = Mydataerror;
            }
            
            var data = _db.Slides.OrderBy(m => m.Number).ToList();
            return View(data);
        }

        [RequireLogin]
        public ActionResult AddSlide()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSlide(Slide slide)
        {
            if (ModelState.IsValid)
            {
                var data = _db.Slides.Where(s => s.Number == slide.Number && s.Number != null).ToList();
                if(slide.Hide == 0 && slide.Number != null)
                {
                    ViewBag.error = "Để ẩn thì thứ tự ảnh phải bằng rỗng";
                    return View();
                }else if(data.Count > 0)
                {
                    ViewBag.error = "Thứ tự ảnh đã bị trùng";
                    return View();
                }

                string fileName = Path.GetFileNameWithoutExtension(slide.ImageFile.FileName);
                string extension = Path.GetExtension(slide.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                slide.ImagePath = "/Resources/Images/" + fileName;
                fileName = Path.Combine(Server.MapPath("/Resources/Images/"), fileName);
                slide.ImageFile.SaveAs(fileName);

                _db.Slides.Add(slide);
                _db.SaveChanges();
                ViewBag.success = "Thêm thành công";
                return View();

            }
            return View();
        }

        [RequireLogin]
        public ActionResult EditSlide(int id)
        {
            var data = _db.Slides.Find(id);
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSlide(Slide slide, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                var data = _db.Slides.Find(slide.ID);
                var datacheck = _db.Slides.Where(s => s.Number == slide.Number && s.Number != null && s.ID != slide.ID).ToList();
                
                if(datacheck.Count > 0)
                {
                    ViewBag.error = "Thứ tự ảnh bị trùng";
                    return View(data);
                }else if (slide.Hide == 0 && slide.Number != null)
                {
                    ViewBag.error = "Để ẩn thì thứ tự ảnh phải bằng rỗng";
                    return View(data);
                }
                else if (slide.ImageFile == null)
                {
                    data.Number = slide.Number;
                    data.Hide = slide.Hide;
                    _db.SaveChanges();
                    ViewBag.success = "Cập nhật thành công";
                    return View(data);
                }
                else if(slide.ImageFile != null && imageFile.ContentLength > 0)
                {
                    if (!string.IsNullOrEmpty(data.ImagePath))
                    {
                        var path = Server.MapPath(data.ImagePath);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }

                    var imageName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                    var extension = Path.GetExtension(imageFile.FileName);
                    var newImageName = imageName + "_" + Guid.NewGuid().ToString("N") + extension;
                    var imagePath = "/Resources/Images/" + newImageName;
                    var imageServerPath = Server.MapPath(imagePath);
                    imageFile.SaveAs(imageServerPath);

                    data.ImagePath = imagePath;
                    data.Number = slide.Number;
                    data.Hide = slide.Hide;
                    _db.SaveChanges();
                    ViewBag.success = "Cập nhật thành công";
                    return View(data);
                }
            }
            
            return View();
        }

        [RequireLogin]
        public ActionResult DeleteSlide(int id)
        {
            var data = _db.Slides.Find(id);
            if (data == null)
            {
                return HttpNotFound();
            }
            _db.Slides.Remove(data);
            _db.SaveChanges();
            TempData["success"] = "Xóa Slide thành công";
            return RedirectToAction("Index", "Slide", new { area = "Admin" });
        }

    }
}