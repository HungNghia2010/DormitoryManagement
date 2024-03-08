using DormitoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using WebMatrix.WebData;
using System.Web.Helpers;
using DormitoryManagement.Areas.Admin.Data;

namespace DormitoryManagement.Controllers
{
    public class HomeController : Controller
    {
		private DormitoryManagementEntities _db = new DormitoryManagementEntities();

		[RequireLogin]
		public ActionResult Index()
        {
            return View();
        }

		[RequireLogin]
		public ActionResult Info()
		{
			// Lấy tên người dùng đã đăng nhập
			var username = (string)Session["username"];

			// Lấy thông tin sinh viên có tên đăng nhập tương ứng từ cơ sở dữ liệu
			var student = _db.StudentAccounts.FirstOrDefault(s => s.UserName == username);

			if (student != null)
			{
				// Truyền ID sinh viên vào view
				ViewBag.StudentId = student.StudentID;
			
				// Truyền thông tin sinh viên vào view model
				var studentModel = new DormitoryManagement.Models.StudentAccount
				{
					FullName = student.FullName,
					UserName = username,
					Password = student.Password,
					Email = student.Email,
					PhoneNumber = student.PhoneNumber,
					Address = student.Address,
					Gender = student.Gender,
					ImagePath = student.ImagePath,
					RoomID = student.RoomID,
					BuildingID = student.BuildingID,
					// Các thuộc tính khác của studentModel
				};
				return View(studentModel);
			}

			// Xử lý trường hợp không tìm thấy thông tin sinh viên
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult UpdateInfo(StudentAccount studentModel)
		{
			if (ModelState.IsValid)
			{
				// Get the logged-in username
				var username = (string)Session["username"];

				// Get the student from the database based on the username
				var student = _db.StudentAccounts.FirstOrDefault(s => s.UserName == username);

				if (student != null)
				{
					// Update the student's information with the new values
					student.FullName = studentModel.FullName;
					student.UserName = studentModel.UserName;
					student.Email = studentModel.Email;
					student.PhoneNumber = studentModel.PhoneNumber;
					student.Address = studentModel.Address;
					student.Gender = studentModel.Gender;
					student.RoomID = studentModel.RoomID;
					student.BuildingID = studentModel.BuildingID;

					// Save the changes to the database
					_db.SaveChanges();

					// Redirect to the updated info page or any other desired page
					return RedirectToAction("Info");
				}
			}

			// Handle validation errors or any other errors that occurred during the update
			return View(studentModel);
		}

		// GET: Home
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
				var data = _db.StudentAccounts.Where(s => s.UserName == username && s.Password.Equals(f_password)).ToList();
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


		public ActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		public ActionResult ForgotPassword(string email)
		{
			//Verify Email ID
			//Generate Reset password link 
			//Send Email 
			string message = "";
			bool status = false;

			
			var account = _db.StudentAccounts.Where(a => a.Email == email).FirstOrDefault();
			if (account != null)
			{
				//Send email for reset password
				string resetCode = Guid.NewGuid().ToString();
				SendVerificationLinkEmail(account.Email, resetCode, "ResetPassword");
				account.ResetPasswordCode = resetCode;
				//This line I have added here to avoid confirm password not match issue , as we had added a confirm password property 
				//in our model class in part 1
				_db.Configuration.ValidateOnSaveEnabled = false;
				_db.SaveChanges();
				message = "Reset password link has been sent to your email id.";
			}
			else
			{
				message = "Account not found";
			}
			
			ViewBag.Message = message;
			return View();
		}
		public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "VerifyAccount")
		{
			var verifyUrl = "/Home/" + emailFor + "/" + activationCode;
			var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

			var fromEmail = new MailAddress("acnast9@gmail.com", "Dotnet Awesome");
			var toEmail = new MailAddress(emailID);
			var fromEmailPassword = "hqbbmcynpkwhcfnq"; // Replace with actual password

			string subject = "";
			string body = "";
			if (emailFor == "VerifyAccount")
			{
				subject = "Your account is successfully created!";
				body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
					" successfully created. Please click on the below link to verify your account" +
					" <br/><br/><a href='" + link + "'>" + link + "</a> ";

			}
			else if (emailFor == "ResetPassword")
			{
				subject = "Reset Password";
				body = "Hi,<br/>br/>We got request for reset your account password. Please click on the below link to reset your password" +
					"<br/><br/><a href=" + link + ">Reset Password link</a>";
			}


			var smtp = new SmtpClient
			{
				Host = "smtp.gmail.com",
				Port = 587,
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
			};

			using (var message = new MailMessage(fromEmail, toEmail)
			{
				Subject = subject,
				Body = body,
				IsBodyHtml = true
			})
				smtp.Send(message);
		}

		public ActionResult ResetPassword(string id)
		{
			//Verify the reset password link
			//Find account associated with this link
			//redirect to reset password page
			if (string.IsNullOrWhiteSpace(id))
			{
				return HttpNotFound();
			}

			var user = _db.StudentAccounts.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
			if (user != null)
			{
				ResetPassword model = new ResetPassword();
				model.ResetCode = id;
				return View(model);
			}
			else
			{
				return HttpNotFound();
			}
		
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ResetPassword(ResetPassword model)
		{
			var message = "";
			if (ModelState.IsValid)
			{
				
				var user = _db.StudentAccounts.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
				if (user != null)
				{
					user.Password = GetMD5(model.NewPassword);
					user.ResetPasswordCode = "";
					_db.Configuration.ValidateOnSaveEnabled = false;
					_db.SaveChanges();
					message = "New password updated successfully";
				}
				
			}
			else
			{
				message = "Something invalid";
			}
			ViewBag.Message = message;
			return View(model);
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