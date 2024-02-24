using DormitoryManagement.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DormitoryManagement.Areas.Admin.Controllers
{
	public class StudentController : Controller
	{

		private DormitoryManagementEntities _db = new DormitoryManagementEntities();
		// GET: Admin/Student
		[RequireLogin]
		public ActionResult Index()
		{
			var data = _db.StudentAccounts.ToList();
			return View(data);
		}

		// GET: Admin/Student/Add
		[RequireLogin]
		public ActionResult AddStudent()
		{
			return View();
		}

		// Post: Admin/Student/Add
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AddStudent(StudentAccount student)
		{
			if (ModelState.IsValid)
			{
				var data = _db.StudentAccounts.Where(s => s.UserName.Equals(student.UserName)).ToList();
				var data2 = _db.StudentAccounts.Where(s => s.Email.Equals(student.Email)).ToList();
				if (data.Count > 0)
				{
					ViewData["error"] = "Tên tài khoản bị trùng khớp";
					return View();
				}
				if (data2.Count > 0)
				{
					ViewData["error"] = "Email bị trùng khớp";
					return View();
				}
				else
				{
					// Check if an image file was uploaded
					if (student.ImageFile != null && student.ImageFile.Length > 0)
					{
						// Generate a unique filename for the image
						string fileName = Guid.NewGuid().ToString() + Path.GetExtension(student.ImageFile.FileName);

						// Set the imagePath property to the filename
						student.ImagePath = fileName;

						// Save the image file to a designated folder
						string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
						using (var stream = new FileStream(filePath, FileMode.Create))
						{
							student.ImageFile.CopyTo(stream);
						}
					}

					_db.StudentAccounts.Add(student);
					_db.SaveChanges();
					ViewData["success"] = "Thêm sinh viên thành công";
					return View();
				}
			}
			return View();
		}

		[RequireLogin]
		public ActionResult Delete(int id)
		{
			// Lấy dữ liệu cần xóa từ cơ sở dữ liệu
			var data = _db.StudentAccounts.Find(id);
			if (data == null)
			{
				return HttpNotFound();
			}
			else
			{
				var s = data.StudentID;
				var m = data.UserName;
				_db.StudentAccounts.Remove(data);
				_db.SaveChanges();
				TempData["success"] = "Xóa sinh viên " + m + " thành công";
				return RedirectToAction("Index", "Student", new { area = "Admin", studentID = s });
			}

		}

	}
}