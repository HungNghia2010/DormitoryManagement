using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DormitoryManagement.Areas.Admin.Controllers
{
    public class RoomController : Controller
    {
        // GET: Admin/Room
        public ActionResult Index()
        {
            return View();
        }
    }
}