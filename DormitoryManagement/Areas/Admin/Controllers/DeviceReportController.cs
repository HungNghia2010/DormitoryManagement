using DormitoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DormitoryManagement.Areas.Admin.Controllers
{
    public class DeviceReportController : Controller
    {
        private DormitoryManagementEntities _db = new DormitoryManagementEntities();
        // GET: Admin/DeviceReport
        public ActionResult Index()
        {
            if (TempData["success"] != null)
            {
                ViewBag.success = TempData["success"];
            }

            if (TempData["error"] != null)
            {
                ViewBag.error = TempData["error"];
            }

            var data = _db.DeviceReports.ToList();

            var room = _db.Rooms.ToList();


            Dictionary<int, string> buildingRoomDict = new Dictionary<int, string>();
            foreach (var m in room)
            {

                if (!buildingRoomDict.ContainsKey(m.RoomID))
                {
                    // Nếu chưa, thêm một cặp khóa/giá trị mới vào từ điển
                    buildingRoomDict[m.RoomID] = m.Building.Name;
                }
                else
                {
                    buildingRoomDict[m.RoomID] += ", " + m.Building.Name;
                }
            }

            ViewBag.myDict = buildingRoomDict;

            ViewBag.room = room;

            return View(data);
        }

        public ActionResult UpdateDeviceReport(int id)
        {

            var data = _db.DeviceReports.Find(id);
            var room = _db.Rooms.Find(data.RoomId);
            var building = _db.Buildings.Find(room.BuildingID);


            ViewBag.building = building.Name;
            ViewBag.room = room.Name;
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateDeviceReport(DeviceReport deviceReport)
        {
            if (ModelState.IsValid)
            {
                var check = _db.DeviceReports.Find(deviceReport.ID);
                var room = _db.Rooms.Find(check.RoomId);
                var building = _db.Buildings.Find(room.BuildingID);
                ViewBag.building = building.Name;
                ViewBag.room = room.Name;

                if (deviceReport.ReportStatus == null)
                {
                    ViewBag.error = "Hãy chọn trạng thái";
                    return View();
                }
                var data = _db.DeviceReports.Find(deviceReport.ID);
                data.ReportStatus = deviceReport.ReportStatus;
                _db.SaveChanges();
                ViewBag.success = "Cập nhật thành công";
            }
            return View();
        }

        public ActionResult DeleteDevice(int id)
        {
            // Lấy dữ liệu cần xóa từ cơ sở dữ liệu
            var data = _db.DeviceReports.Find(id);
            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (data.ReportStatus != "Đã hoàn thành")
                {
                    TempData["error"] = "Không thể xóa";
                    return RedirectToAction("Index", "DeviceReport", new { area = "Admin" });
                }
                else
                {
                    _db.DeviceReports.Remove(data);
                    _db.SaveChanges();
                    TempData["success"] = "Xóa thành công";
                    return RedirectToAction("Index", "DeviceReport", new { area = "Admin" });
                }

            }
        }
    }
}