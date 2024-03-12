using DormitoryManagement.Areas.Admin.Data;
using DormitoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DormitoryManagement.Areas.Admin.Controllers
{
    public class TuitionFeeController : Controller
    {
        private DormitoryManagementEntities _db = new DormitoryManagementEntities();

        // GET: Admin/TuitionFee
        [RequireLogin]
        public ActionResult Index()
        {
            var data = _db.FeePayments.ToList();

            var Mydata = TempData["success"];
            ViewBag.success = Mydata;

            var Mydataerror = TempData["error"];
            ViewBag.error = Mydataerror;

            return View(data);
        }


        // GET: Admin/TuitionFee/AddTuition
        [RequireLogin]
        public ActionResult AddTuition()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTuition(FeePayment feePayment)
        {
            if (ModelState.IsValid)
            {
                var datacheck = _db.FeePayments.Where(s => s.MonthYear.Equals(feePayment.MonthYear)).ToList();
                if(datacheck.Count > 0)
                {
                    ViewData["error"] = "Tháng ghi đã bị trùng khớp";
                    return View();
                }
                _db.FeePayments.Add(feePayment);
                _db.SaveChanges();

                var students = _db.StudentAccounts.Where(s=>s.RoomID != null).ToList();

                foreach(var student in students)
                {
                    var s = _db.Rooms.Find(student.RoomID);
                    var IDtype = _db.LoaiPhongs.Find(s.MaLoaiPhong);

                    var studentFee = new StudentFee
                    {
                        StudentId = student.StudentID,
                        PaymentId = feePayment.PaymentID,
                        RoomId = student.RoomID ?? 0,
                        PaymentStatus = "Chưa thanh toán",
                        TotalAmount = IDtype.GiaTien
                    };
                    _db.StudentFees.Add(studentFee);
                    _db.SaveChanges();
                }
                
                ViewData["success"] = "Thêm thành công";
                return View();
            }

            return View();
        }

        // GET: Admin/TuitionFee/EditTuition
        [RequireLogin]
        public ActionResult EditTuition(int id)
        {
            var data = _db.FeePayments.Find(id);
            return View(data);
        }

        [RequireLogin]
        public ActionResult DeleteTuitionFee(int id)
        {
            // Lấy dữ liệu cần xóa từ cơ sở dữ liệu
            var data = _db.FeePayments.Find(id);
            var m = data.Description;
            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {
                var check = _db.StudentFees.Where(a => a.PaymentId == id).ToList();
                
                if (check.Count > 0)
                {
                    TempData["error"] = "Không thể xóa phiếu thu này";
                    return RedirectToAction("Index", "TuitionFee", new { area = "Admin"});
                }
                
                _db.FeePayments.Remove(data);
                _db.SaveChanges();
                TempData["success"] = "Xóa phiếu thu " + m + " thành công";
                return RedirectToAction("Index", "TuitionFee", new { area = "Admin" });
            }

        }


        // Post: Admin/TuitionFee/EditTuition
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTuition(FeePayment feePayment)
        {
            if (ModelState.IsValid)
            {
                var datacheck = _db.FeePayments.Where(m => m.PaymentID != feePayment.PaymentID && m.MonthYear.Equals(feePayment.MonthYear)).ToList();
                if (datacheck.Count > 0)
                {
                    ViewData["error"] = "Tháng ghi đã bị trùng khớp";
                    return View();
                }
                var fee = _db.FeePayments.Find(feePayment.PaymentID);
                fee.MonthYear = feePayment.MonthYear;
                fee.Description = feePayment.Description;
                fee.DueDate = feePayment.DueDate;
                fee.ExpiryDate = feePayment.ExpiryDate;
                _db.SaveChanges();
                ViewData["success"] = "Cập nhật thành công thành công";
                return View();
            }
            return View();
        }

        // GET: Admin/TuitionFee/EditTuition
        [RequireLogin]
        public ActionResult ManagementFee()
        {
            var Mydata = TempData["success"];
            ViewBag.success = Mydata;

            var Mydataerror = TempData["error"];
            ViewBag.error = Mydataerror;

            return View(GetFeeData());
        }

        [RequireLogin]
        public ActionResult DeleteStudentFee(int id)
        {
            // Lấy dữ liệu cần xóa từ cơ sở dữ liệu
            var data = _db.StudentFees.Find(id);
            var m = data.Id;
            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {
                var check = _db.StudentFees.Where(a => a.Id == id && a.PaymentStatus.Equals("Chưa thanh toán")).ToList();

                if (check.Count > 0)
                {
                    TempData["error"] = "Không thể xóa hóa đơn này";
                    return RedirectToAction("ManagementFee", "TuitionFee", new { area = "Admin" });
                }

                _db.StudentFees.Remove(data);
                _db.SaveChanges();
                TempData["success"] = "Xóa phiếu thu " + m + " thành công";
                return RedirectToAction("ManagementFee", "TuitionFee", new { area = "Admin" });
            }

        }

        // GET: Admin/TuitionFee/EditTuition
        [RequireLogin]
        public ActionResult EditStudentFee(int id)
        {
            var data = GetFeeDataById(id);
            if (id != null) {
                return View(data);
            }
            return View();
        }

        // Post: Admin/TuitionFee/editstudentfee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStudentFee(FeeData feeData)
        {
            if (ModelState.IsValid)
            {
                var data = _db.StudentFees.Find(feeData.ID);
                data.TotalAmount = feeData.TotalAmount;
                data.Description = feeData.Note;
                data.PaymentStatus = feeData.PaymentStatus;
                ViewData["success"] = "Cập nhật thành công";
                _db.SaveChanges();
                return View();
            }
            return View();
        }

        public List<FeeData> GetFeeData()
        {
            return (from sf in _db.StudentFees
                    join r in _db.Rooms on sf.RoomId equals r.RoomID
                    join b in _db.Buildings on r.BuildingID equals b.BuildingID
                    join sa in _db.StudentAccounts on sf.StudentId equals sa.StudentID
                    join fp in _db.FeePayments on sf.PaymentId equals fp.PaymentID
                    select new FeeData
                    {
                        RoomName = r.Name,
                        BuildingName = b.Name,
                        PaymentStatus = sf.PaymentStatus,
                        Descript = fp.Description,
                        PaymentId = sf.PaymentId,
                        TotalAmount = sf.TotalAmount,
                        FullName = sa.FullName,
                        StudentID = sa.StudentID,
                        MonthYear = fp.MonthYear,
                        ID = sf.Id,
                        Note = sf.Description
                    }).ToList();
        }

        public FeeData GetFeeDataById(int id)
        {
            var data = (from sf in _db.StudentFees
                        join r in _db.Rooms on sf.RoomId equals r.RoomID
                        join b in _db.Buildings on r.BuildingID equals b.BuildingID
                        join sa in _db.StudentAccounts on sf.StudentId equals sa.StudentID
                        join fp in _db.FeePayments on sf.PaymentId equals fp.PaymentID
                        where sf.Id == id
                        select new FeeData
                        {
                            RoomName = r.Name,
                            BuildingName = b.Name,
                            PaymentStatus = sf.PaymentStatus,
                            Descript = fp.Description,
                            PaymentId = sf.PaymentId,
                            TotalAmount = sf.TotalAmount,
                            FullName = sa.FullName,
                            StudentID = sa.StudentID,
                            MonthYear = fp.MonthYear,
                            ID = sf.Id,
                            Note = sf.Description
                        }).ToList();

            return data.FirstOrDefault();
        }
    }
}