using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DormitoryManagement.Areas.Admin
{
    public class RequireLogoutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var userId = filterContext.HttpContext.Session["idUser"];

            if (userId != null)
            {
                // Nếu session tồn tại, chuyển hướng đến trang chính
                filterContext.Result = new RedirectResult("~/Homes");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}