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
            if (Mydata != null)
            {
                ViewBag.success = Mydata;
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
                    ViewData["success"] = "Thêm loại phòng thành công";
                    return View();
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
                var check = _db.Rooms.Where(m => m.RoomType.Equals(data.TenLoaiPhong)).ToList();
                if(check != null)
                {
                    TempData["error"] = "Không thể xóa loại phòng này";
                    return RedirectToAction("Index", "Room", new { area = "Admin" });
                }
                else
                {
                    _db.LoaiPhongs.Remove(data);
                    _db.SaveChanges();
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

    }
}