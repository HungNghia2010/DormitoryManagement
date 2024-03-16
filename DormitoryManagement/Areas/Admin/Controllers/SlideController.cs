using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DormitoryManagement.Areas.Admin.Controllers
{
    public class SlideController : Controller
    {
        // GET: Admin/Slide
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddSlide()
        {
            return View();
        }

    }
}