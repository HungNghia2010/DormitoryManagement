using DormitoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DormitoryManagement.Controllers
{
    public class HomeController : Controller
    {
		private DormitoryManagementEntities _db = new DormitoryManagementEntities();

		public ActionResult Index()
        {
            return View();
        }

		public ActionResult Info()
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

		public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}