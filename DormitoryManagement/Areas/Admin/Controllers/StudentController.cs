using DormitoryManagement.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using System.Web.Services.Description;
namespace DormitoryManagement.Areas.Admin.Controllers
{
	public class StudentController : Controller
	{

		private DormitoryManagementEntities _db = new DormitoryManagementEntities();

		// GET: Admin/Student
		[RequireLogin]
		public ActionResult Index()
		{
			var room = _db.Rooms.ToList();
			var data = _db.StudentAccounts.ToList();

			Dictionary<int,string> buildingRoomDict = new Dictionary<int, string>();
			foreach (var m in room)
			{

				if (!buildingRoomDict.ContainsKey(m.RoomID))
				{
					// Nếu chưa, thêm một cặp khóa/giá trị mới vào từ điển
					buildingRoomDict[m.RoomID] = m.Building.Name;
				}
				else
				{
					buildingRoomDict[m.RoomID] += ", " + m.Building.Name;
				}
			}

			ViewBag.myDict = buildingRoomDict;
			ViewBag.room = room;


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
				if (!IsValidEmail(student.Email))
				{
					ViewData["error"] = "Email không hợp lệ";
					return View();
				}
				

				else
				{
					// Hash the password using MD5
					student.Password = GetMD5(student.Password);
					string fileName = Path.GetFileNameWithoutExtension(student.ImageFile.FileName);
					string extension = Path.GetExtension(student.ImageFile.FileName);
					fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
					student.ImagePath = "/Resources/Images/" + fileName;
					fileName = Path.Combine(Server.MapPath("/Resources/Images/"), fileName);
					student.ImageFile.SaveAs(fileName);

					_db.StudentAccounts.Add(student);
					_db.SaveChanges();
					ViewData["success"] = "Thêm sinh viên thành công";
					return View();
				}
			}
			return View();
		}

		private bool IsValidEmail(string email)
		{
			// Regex pattern for email validation
			string pattern = @"^[\w\.-]+@[\w\.-]+\.\w+$";
			Regex regex = new Regex(pattern);
			return regex.IsMatch(email);
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
			else if (data.IsLocked == 1)
			{
				TempData["error"] = "Cannot delete a locked account.";
				return Redirect("https://localhost:44342/Admin/Student");
			}
			else
			{
				var s = data.StudentID;
				var m = data.UserName;
				data.IsLocked = 0;
				_db.StudentAccounts.Remove(data);
				_db.SaveChanges();
				TempData["success"] = "Xóa sinh viên " + m + " thành công";
				return RedirectToAction("Index", "Student", new { area = "Admin", studentID = s });
			}

		}

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

		// GET: Admin/Student/Update/id
		[RequireLogin]
		public ActionResult UpdateStudent(int id)
		{
			var student = _db.StudentAccounts.Find(id);
			if (student == null)
			{
				return HttpNotFound();
			}
			else if (student.IsLocked == 1)
			{
				TempData["error"] = "Cannot update a locked account.";
				return RedirectToAction("Index");
			}
			return View(student);
		}


		// POST: Admin/Student/Update/4
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult UpdateStudent(StudentAccount updatedStudent, HttpPostedFileBase imageFile)
		{
			if (ModelState.IsValid)
			{

				var usernameExists = _db.StudentAccounts.Any(s => s.UserName == updatedStudent.UserName && s.StudentID != updatedStudent.StudentID);
				if (usernameExists)
				{
					ViewData["error"] = "Tên tài khoản bị trùng khớp";
					return View();
				}
				else if (!IsValidEmail(updatedStudent.Email))
				{
					ViewData["error"] = "Email không hợp lệ";
					return View();
				}
				else
				{
					var existingStudent = _db.StudentAccounts.Find(updatedStudent.StudentID);
					// Hash the password using MD5
					
					// Update the properties of the existing student with the values from the updated student
					existingStudent.UserName = updatedStudent.UserName;
					existingStudent.FullName = updatedStudent.FullName;
					existingStudent.Email = updatedStudent.Email;
					existingStudent.Address = updatedStudent.Address;
					existingStudent.PhoneNumber = updatedStudent.PhoneNumber;
					existingStudent.Gender = updatedStudent.Gender;
					existingStudent.RoomID = updatedStudent.RoomID;
					existingStudent.BuildingID = updatedStudent.BuildingID;

					// Check if a new image file was uploaded
					if (imageFile != null && imageFile.ContentLength > 0)
					{
						// Delete the existing image file, if any
						if (!string.IsNullOrEmpty(existingStudent.ImagePath))
						{
							var path = Server.MapPath(existingStudent.ImagePath);
							if (System.IO.File.Exists(path))
							{
								System.IO.File.Delete(path);
							}
						}

						// Save the new image file
						var imageName = Path.GetFileNameWithoutExtension(imageFile.FileName);
						var extension = Path.GetExtension(imageFile.FileName);
						var newImageName = imageName + "_" + Guid.NewGuid().ToString("N") + extension;
						var imagePath = "/Resources/Images/" + newImageName;
						var imageServerPath = Server.MapPath(imagePath);
						imageFile.SaveAs(imageServerPath);

						// Update the student's image path
						existingStudent.ImagePath = imagePath;
					}

					// Save the changes to the database
					_db.SaveChanges();

					ViewData["success"] = "Cập nhật sinh viên thành công";
					return View(existingStudent);
				}
				
			}

			return View(updatedStudent);
		}

		[RequireLogin]
		public ActionResult UnlockAccount(int id)
		{
			var student = _db.StudentAccounts.Find(id);
			if (student == null)
			{
				return HttpNotFound();
			}
			else if (student.IsLocked == 0)
			{
				TempData["error"] = "The account is already unlocked.";
				return RedirectToAction("Index");
			}

			student.IsLocked = 0;
			_db.SaveChanges();

			TempData["success"] = "Unlocked the account successfully.";
			return RedirectToAction("Index");
		}

	}
}