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
            var room = _db.Rooms.Find(data.ID);
            var building = _db.Buildings.Find(room.BuildingID);

            
            ViewBag.building = building.Name;
            ViewBag.room = room.Name;
            return View(data);
        }
    }
}