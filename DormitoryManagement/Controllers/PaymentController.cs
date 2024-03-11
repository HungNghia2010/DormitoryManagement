using DormitoryManagement.Areas.Admin.Data;
using DormitoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DormitoryManagement.Controllers
{
    public class PaymentController : Controller
    {
        private DormitoryManagementEntities _db = new DormitoryManagementEntities();

        // GET: Payment
        public ActionResult Index()
        {
            var idUser = Convert.ToInt32(Session["idUser"]);




            var data = (from sf in _db.StudentFees
                        join r in _db.Rooms on sf.RoomId equals r.RoomID
                        join b in _db.Buildings on r.BuildingID equals b.BuildingID
                        join sa in _db.StudentAccounts on sf.StudentId equals sa.StudentID
                        join fp in _db.FeePayments on sf.PaymentId equals fp.PaymentID
                        where sa.StudentID == idUser
                        orderby sf.PaymentId descending
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
                            DueDate = fp.DueDate,
                            ExpiryDate = fp.ExpiryDate,
                            ID = sf.Id
                        }).ToList();

            return View(data);
        }
    }
}