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
    
    public partial class FeePayment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FeePayment()
        {
            this.StudentFees = new HashSet<StudentFee>();
        }
    
        public int PaymentID { get; set; }
        public string Description { get; set; }
        public string MonthYear { get; set; }
        public string DueDate { get; set; }
        public string ExpiryDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentFee> StudentFees { get; set; }
    }
}
