using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DormitoryManagement.Controllers
{
    public class BlogController : Controller
    {
        // GET: Blog
        [RequireLogin]
        public ActionResult Index()
        {
            return View();
        }

        [RequireLogin]
        public ActionResult Detail()
        {
            return View();
        }
    }
}