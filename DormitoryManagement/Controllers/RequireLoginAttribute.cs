using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DormitoryManagement.Controllers
{
   public class RequireLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var userId = filterContext.HttpContext.Session["idUser"];

            if (userId == null)
            {
                // Kiểm tra nếu không phải trang đăng nhập hoặc đăng ký, thì mới chuyển hướng
                if (filterContext.ActionDescriptor.ActionName != "Login")
                {
                    filterContext.Result = new RedirectResult("~/Access/Login"); // Chuyển hướng đến trang đăng nhập
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}