using DormitoryManagement.Areas.Admin.Data;
using DormitoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace DormitoryManagement.Areas.Admin.Controllers
{
    public class HomesController : Controller
    {
        private DormitoryManagementEntities _db = new DormitoryManagementEntities();

        // GET: Admin/Homes
        [RequireLogin]
        public ActionResult Index()
        {

            var buildings = _db.Buildings.ToList();
            return View(buildings);
        }

        // GET: Admin/Homes
        [RequireLogin]
        public ActionResult ChangePass()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePass(string password, string newpassword, string ConfirmPassword)
        {
            if (ModelState.IsValid)
            {
                var f_password = GetMD5(password);
                var username = Session["username"];
                var data = _db.AdminAccounts.Where(s => s.Username == username && s.Password.Equals(f_password)).ToList();
                if (data.Count > 0)
                {
                    if (password.Equals(newpassword))
                    {
                        ViewData["error"] = "Mật khẩu mới không được trùng với mật khẩu cũ";
                        return View();
                    }
                    else if (newpassword.Equals(ConfirmPassword))
                    {
                        if (CalculatePasswordStrength(newpassword) > 50)
                        {
                            var pf = GetMD5(newpassword);
                            data.FirstOrDefault().Password = pf;
                            _db.SaveChanges();
                            ViewData["success"] = "Thành công";
                            return View();
                        }
                        else
                        {
                            ViewData["error"] = "Mật khẩu mới không bảo mật";
                            return View();
                        }
                    }
                    else
                    {
                        ViewData["error"] = "Mật khẩu mới không trùng khớp";
                        return View();
                    }
                }
                else
                {
                    ViewData["error"] = "Mật khẩu hiện tại không đúng";
                    return View();
                }
            }
            return View();
        }

        private int CalculatePasswordStrength(string password)
        {
            // Điểm dựa trên chiều dài mật khẩu
            int lengthPoints = Math.Min(password.Length * 6, 50); // Ví dụ: 5 điểm cho mỗi ký tự

            int charTypePoints = 0;
            if (Regex.IsMatch(password, "[a-z]")) charTypePoints++;
            if (Regex.IsMatch(password, "[A-Z]")) charTypePoints++;
            if (Regex.IsMatch(password, "\\d")) charTypePoints++;
            if (Regex.IsMatch(password, "[!@#\\$%\\^&\\*\\(\\)_\\+\\-=\\[\\]\\{\\};:'\",<>/\\?\\\\|`~]")) charTypePoints++;

            int strength = lengthPoints + (charTypePoints * 20); // Điểm tối đa là 100

            return strength;
        }

        //create a string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }

        // GET: Admin/Homes/Floor
        [RequireLogin]
        public ActionResult Floor()
        {
            var Mydata = TempData["success"];
            ViewBag.success = Mydata;

            var builingId = Request.QueryString["buildingId"];
            var ID = Convert.ToInt32(builingId);
            var data = _db.Rooms.Where(s => s.BuildingID == ID).ToList();
            var building = _db.Buildings.Find(ID);


            ViewData["name"] = building.Name;
            ViewData["id"] = building.BuildingID;

            return View(data);
        }

        // GET: Admin/Homes/AdddApartment
        [RequireLogin]
        public ActionResult AddApartment()
        {
            return View();
        }

        // POST: Admin/Homes/AddApartment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddApartment(Building building)
        {
            if (ModelState.IsValid)
            {
                var Data = _db.Buildings.Where(s => s.Name == building.Name).ToList();
                if(Data.Count > 0)
                {
                    ViewData["error"] = "Tên tòa nhà bị trùng khớp";
                    return View();
                }
                else
                {
                    _db.Buildings.Add(building);
                    _db.SaveChanges();
                    ViewData["success"] = "Thêm tòa nhà thành công";
                    return View();
                }
            }
            return View();
        }

        // GET: Admin/Homes/EditApartment
        [RequireLogin]
        public ActionResult EditApartment(int id)
        {
            var data = _db.Buildings.Find(id);
            return View(data);
        }

        // Post: Admin/Homes/EditApartment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditApartment(Building building)
        {
            if (ModelState.IsValid)
            {
                var Data = _db.Buildings.Where(s => s.Name == building.Name && s.BuildingID != building.BuildingID).ToList();
                if (Data.Count > 0)
                {
                    ViewData["error"] = "Tên tòa nhà bị trùng khớp";
                    return View();
                }
                else
                {
                    var buil = _db.Buildings.Find(building.BuildingID);
                    buil.Name = building.Name;
                    buil.Descrip = building.Descrip;
                    _db.SaveChanges();
                    ViewData["success"] = "Cập nhật tòa nhà thành công";
                    return View();
                }
            }
            return View();
        }

        [RequireLogin]
        public ActionResult Delete(int id)
        {
            // Lấy dữ liệu cần xóa từ cơ sở dữ liệu
            var data = _db.Buildings.Find(id);
            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {
                _db.Buildings.Remove(data);
                _db.SaveChanges();
                return RedirectToAction("Index", "Homes", new { area = "Admin" });
            }

        }

        [RequireLogin]
        public ActionResult AddRoom(int buildingId)
        {
            var roomtype = _db.LoaiPhongs.Select(x => new SelectListItem { Value = x.TenLoaiPhong, Text = x.TenLoaiPhong });
            var genders = new List<SelectListItem>
            {
                new SelectListItem { Value = "Male", Text = "Male" },
                new SelectListItem { Value = "Female", Text = "Female" }
            };
            ViewBag.Genders = genders;
            ViewBag.RoomTypes = roomtype;
            ViewBag.id = buildingId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRoom(Room room)
        {
            if (ModelState.IsValid)
            {
                var roomtype = _db.LoaiPhongs.Select(x => new SelectListItem { Value = x.TenLoaiPhong, Text = x.TenLoaiPhong });
                var genders = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Male", Text = "Male" },
                    new SelectListItem { Value = "Female", Text = "Female" }
                };
                ViewBag.Genders = genders;
                ViewBag.RoomTypes = roomtype;

                var data = _db.Rooms.Where(m => m.Name.Equals(room.Name) && m.BuildingID == room.BuildingID).ToList();
                if(data.Count > 0)
                {
                    ViewData["error"] = "Tên phòng bị trùng khớp";
                    return View();
                }
                else
                {
                    room.Occupancy = 0;
                    room.Status = "Còn trống";

                    _db.Rooms.Add(room);
                    _db.SaveChanges();
                    TempData["success"] = "Thêm phòng thành công";
                    return RedirectToAction("Floor", "Homes", new { area = "Admin", buildingId = room.BuildingID });
                }
            }
            
            return View();
        }

        [RequireLogin]
        public ActionResult DeleteRoom(int id)
        {
            // Lấy dữ liệu cần xóa từ cơ sở dữ liệu
            var data = _db.Rooms.Find(id);
            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {
                var s = data.BuildingID;
                var m = data.Name;
                _db.Rooms.Remove(data);
                _db.SaveChanges();
                TempData["success"] = "Xóa phòng " + m + " thành công";
                return RedirectToAction("Floor", "Homes", new { area = "Admin", buildingId = s });
            }

        }

        public ActionResult EditRoom(int id)
        {
            var roomtype = _db.LoaiPhongs.Select(x => new SelectListItem { Value = x.TenLoaiPhong, Text = x.TenLoaiPhong });
            var genders = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Male", Text = "Male" },
                    new SelectListItem { Value = "Female", Text = "Female" }
                };
            ViewBag.Genders = genders;
            ViewBag.RoomTypes = roomtype;

            var student = _db.StudentAccounts.Where(s => s.RoomID == id).ToList();
            ViewBag.Students = student;

            var data = _db.Rooms.Find(id);


            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRoom(Room room, FormCollection form)
        {
            var roomtype = _db.LoaiPhongs.Select(x => new SelectListItem { Value = x.TenLoaiPhong, Text = x.TenLoaiPhong });
            var genders = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Male", Text = "Male" },
                    new SelectListItem { Value = "Female", Text = "Female" }
                };
            ViewBag.Genders = genders;
            ViewBag.RoomTypes = roomtype;

            if (ModelState.IsValid)
            {
                var selectedValues = form["studentName"];
                if (selectedValues != null)
                {
                    string[] studentIdArray = selectedValues.Split(',');
                    int[] intStudentIdArray = Array.ConvertAll(studentIdArray, int.Parse);
                    int s = intStudentIdArray[0];

                    var data = _db.StudentAccounts.Find(s);
                    data.RoomID = room.RoomID;
                    _db.SaveChanges();
                }

                var student = _db.StudentAccounts.Where(s => s.RoomID == room.RoomID).ToList();
                ViewBag.Students = student;

                var roomdata = _db.Rooms.Where(s => s.Name == room.Name && s.RoomID != room.RoomID).ToList();
                if(roomdata.Count() > 0)
                {
                    ViewData["error"] = "Tên tòa nhà bị trùng khớp";
                    return View();
                }else if (student.Count > room.MaxCapacity)
                {
                    ViewData["error"] = "Số giường phải lớn hơn hoặc bằng số sinh viên có trong phòng";
                    return View();
                }else if (selectedValues.Length > room.MaxCapacity)
                {
                    ViewData["error"] = "Số sinh viên không thể lớn hơn số giường";
                    return View();
                }
                else
                {
                    var roomedit = _db.Rooms.Find(room.RoomID);
                    roomedit.Name = room.Name;
                    roomedit.RoomType = room.RoomType;
                    roomedit.MaxCapacity = room.MaxCapacity;
                    roomedit.Gender = room.Gender;
                    room.Descript = room.Descript;
                    room.Occupancy = selectedValues.Length;
                    ViewData["success"] = "Cập nhật tòa nhà thành công";
                    _db.SaveChanges();
                    return View();
                }

            }
            else
            {
                var student = _db.StudentAccounts.Where(s => s.RoomID == room.RoomID).ToList();
                ViewBag.Students = student;
                return View();
            }
        }

        public ActionResult GetStudents()
        {
            var students = _db.StudentAccounts.Where(s => s.RoomID == null).Select(s => new { id = s.StudentID, name = s.FullName }).ToList();
            return Json(students, JsonRequestBehavior.AllowGet);
        }
    }

}