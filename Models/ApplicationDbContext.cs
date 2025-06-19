using Microsoft.EntityFrameworkCore;

namespace CoreAPI.Models
{
    /// <summary>
    /// Application Database Context for DRCARE_CORE database
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// QLCL Bao Cao Kiem Tra Hau Kiem ATTP View
        /// </summary>
        public DbSet<QLCLBaoCaoKiemTraHauKiemATTP> QLCLBaoCaoKiemTraHauKiemATTP { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure QLCLBaoCaoKiemTraHauKiemATTP view with composite key
            modelBuilder.Entity<QLCLBaoCaoKiemTraHauKiemATTP>()
                .ToView("QLCLViewBaoCaoKiemTraHauKiemATTP")
                .HasKey(e => new { e.thang, e.province, e.ward });

        }
    }
} 