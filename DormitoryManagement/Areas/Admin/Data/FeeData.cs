using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DormitoryManagement.Areas.Admin.Data
{
    public class FeeData
    {
        public string RoomName { get; set; }
        public string BuildingName { get; set; }
        public string Descript { get; set; }
        public string PaymentStatus { get; set; }
        public int PaymentId { get; set; }
        public string TotalAmount { get; set; }
        public string FullName { get; set; }
        public int StudentID { get; set; }
        public string MonthYear { get; set; }
        public int ID { get; set; }
        public string DueDate { get; set; }
        public string ExpiryDate { get; set; }
    }
}