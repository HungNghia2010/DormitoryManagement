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
        
        public ActionResult ManagementFee()
        {
            var data = (from sf in _db.StudentFees
                        join r in _db.Rooms on sf.RoomId equals r.RoomID
                        join b in _db.Buildings on r.BuildingID equals b.BuildingID
                        join sa in _db.StudentAccounts on sf.StudentId equals sa.StudentID
                        join fp in _db.FeePayments on sf.PaymentId equals fp.PaymentID
                        select new FeeData
                        {
                            RoomName = r.Name,
                            BuildingName = b.Name,
                            PaymentStatus = sf.PaymentStatus,
                            PaymentId = sf.PaymentId,
                            TotalAmount = sf.TotalAmount,
                            FullName = sa.FullName,
                            StudentID = sa.StudentID,
                            MonthYear = fp.MonthYear,
                            ID = sf.Id
                        }).ToList();
 
            return View(data);
        }

    }
}