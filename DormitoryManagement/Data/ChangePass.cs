using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DormitoryManagement.Data
{
	public partial class ChangePass
	{
		public string Password { get; set; }
		public string NewPassword { get; set; }

		public string ConfirmPassword { get; set; }
	}
}