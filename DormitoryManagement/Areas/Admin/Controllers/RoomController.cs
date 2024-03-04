using DormitoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DormitoryManagement.Areas.Admin.Controllers
{
    public class RoomController : Controller
    {
        private DormitoryManagementEntities _db = new DormitoryManagementEntities();
        // GET: Admin/Room
        [RequireLogin]
        public ActionResult Index()
        {
            var Mydata = TempData["error"];
            var mydata = TempData["success"];
            if (Mydata != null || mydata !=null)
            {
                ViewBag.error = Mydata;
                ViewBag.success = mydata;
            }

            var data = _db.LoaiPhongs.ToList();
            return View(data);
        }

        // GET: Admin/Room/Add
        [RequireLogin]
        public ActionResult AddRoomType()
        {
            return View();
        }

        // Post: Admin/Room/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRoomType(LoaiPhong loaiPhong)
        {
            if (ModelState.IsValid)
            {
                var data = _db.LoaiPhongs.Where(s => s.TenLoaiPhong.Equals(loaiPhong.TenLoaiPhong)).ToList();
                if(data.Count > 0)
                {
                    ViewData["error"] = "Tên loại phòng bị trùng khớp";
                    return View();
                }
                else
                {
                    _db.LoaiPhongs.Add(loaiPhong);
                    _db.SaveChanges();
                    TempData["success"] = "Thêm loại phòng thành công";
                    return RedirectToAction("Index", "Room", new { area = "Admin" });
                }
            }
            return View();
        }

        [RequireLogin]
        public ActionResult Delete(int id)
        {
            // Lấy dữ liệu cần xóa từ cơ sở dữ liệu
            var data = _db.LoaiPhongs.Find(id);
            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {
                var check = _db.Rooms.Where(s => s.MaLoaiPhong == data.MaLoaiPhong).ToList();
                if(check.Count > 0)
                {
                    TempData["error"] = "Không thể xóa loại phòng này";
                    return RedirectToAction("Index", "Room", new { area = "Admin" });
                }
                else
                {
                    var name = data.TenLoaiPhong;
                    _db.LoaiPhongs.Remove(data);
                    _db.SaveChanges();
                    TempData["success"] = "Xóa loại phòng " + name + " thành công";
                    return RedirectToAction("Index", "Room", new { area = "Admin" });
                }
                
            }

        }

        // GET: Admin/Room/Add
        [RequireLogin]
        public ActionResult EditRoomType(int id)
        {
            var data = _db.LoaiPhongs.Find(id);
            return View(data);
        }

        // POST: Admin/Room/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRoomType(LoaiPhong loaiphong)
        {
            if (ModelState.IsValid)
            {
                var datacheck = _db.LoaiPhongs.Where(s => s.TenLoaiPhong.Equals(loaiphong.TenLoaiPhong) && s.MaLoaiPhong != loaiphong.MaLoaiPhong).ToList();
                if(datacheck.Count() > 0)
                {
                    ViewData["error"] = "Tên loại phòng bị trùng khớp";
                    return View();
                }
                else
                {
                    var data = _db.LoaiPhongs.Find(loaiphong.MaLoaiPhong);
                    data.TenLoaiPhong = loaiphong.TenLoaiPhong;
                    data.GiaTien = loaiphong.GiaTien;
                    _db.SaveChanges();
                    ViewData["success"] = "Cập nhật loại phòng thành công";
                    return View();
                }
            
            }
            return View();
        }

    }
}