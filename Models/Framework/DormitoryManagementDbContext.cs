using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Models.Framework
{
    public partial class DormitoryManagementDbContext : DbContext
    {
        public DormitoryManagementDbContext()
            : base("name=DormitoryManagementDbContext")
        {
        }

        public virtual DbSet<AdminAccount> AdminAccounts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminAccount>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<AdminAccount>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<AdminAccount>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<AdminAccount>()
                .Property(e => e.FullName)
                .IsUnicode(false);
        }
    }
}
