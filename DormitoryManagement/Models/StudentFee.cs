//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DormitoryManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class StudentFee
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int PaymentId { get; set; }
        public int RoomId { get; set; }
        public string TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string Description { get; set; }
    
        public virtual FeePayment FeePayment { get; set; }
        public virtual Room Room { get; set; }
        public virtual StudentAccount StudentAccount { get; set; }
    }
}
