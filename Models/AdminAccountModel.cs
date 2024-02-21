using Models.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    class AdminAccountModel
    {
        private DormitoryManagementDbContext context = null;
        public AdminAccountModel()
        {
            context = new DormitoryManagementDbContext();
        }
        public bool Login(string username, string password)
        {

        }
    }
}
