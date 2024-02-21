using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DormitoryManagement.Areas.Admin.Data
{
    public partial class Changepass
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }
}