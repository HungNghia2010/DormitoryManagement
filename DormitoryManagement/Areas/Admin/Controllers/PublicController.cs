using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DormitoryManagement.Models;

namespace DormitoryManagement.Areas.Admin.Controllers
{
    public class PublicController : Controller
    {
        private DormitoryManagementEntities _db = new DormitoryManagementEntities();

        // GET: Admin/Public/Logout
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Login");
        }

        // GET: Admin/Public
        [RequireLogout]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Admin/Public
        [HttpPost]
        [RequireLogout]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                var f_password = GetMD5(password);
                var data = _db.AdminAccounts.Where(s => s.Username.Equals(username)).ToList();
                if (data.Count() > 0)
                {
                    if (data.FirstOrDefault().IsLocked == 1)
                    {
                        ViewData["error"] = "Your account is enabled";
                        return View();
                    }
                    else if (data.FirstOrDefault().Password.Equals(f_password))
                    {
                        //add session
                        Session["username"] = data.FirstOrDefault().Username;
                        Session["idUser"] = data.FirstOrDefault().AdminID;
                        data.FirstOrDefault().LoginAttempts = 0;
                        _db.SaveChanges();

                        return RedirectToRoute(new { Controller = "Homes", action = "Index" });
                    }
                    else
                    {
                        data.FirstOrDefault().LoginAttempts += 1;
                        if(data.FirstOrDefault().LoginAttempts == 5)
                        {
                            data.FirstOrDefault().IsLocked = 1;
                        }
                        _db.SaveChanges();

                        ViewData["error"] = "Your username or password not correct";
                        return View();
                    }

                }
                else
                {
                    ViewData["error"] = "Your email or password not correct";
                    return View();
                }
            }
            return View();
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



    }
}