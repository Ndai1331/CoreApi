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

        /// <summary>
        /// QLCL Detail Co So Kiem Tra Hau Kiem ATTP
        /// </summary>
        public DbSet<QLCLDetailCoSoKiemTraHauKiemATTP> QLCLDetailCoSoKiemTraHauKiemATTP { get; set; }

        /// <summary>
        /// QLCL Chi Tiet Kiem Tra Hau Kiem ATTP
        /// </summary>
        public DbSet<QLCLChiTietKiemTraHauKiemATTP> QLCLChiTietKiemTraHauKiemATTP { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure QLCLBaoCaoKiemTraHauKiemATTP view with composite key
            modelBuilder.Entity<QLCLBaoCaoKiemTraHauKiemATTP>()
                .ToView("QLCLViewBaoCaoKiemTraHauKiemATTP")
                .HasKey(e => new { e.thang });

            // Configure QLCLDetailCoSoKiemTraHauKiemATTP view
            modelBuilder.Entity<QLCLDetailCoSoKiemTraHauKiemATTP>()
                .ToView("QLCLViewDetailCoSoKiemTraHauKiemATTP")
                .HasKey(e => e.id);

            // Configure QLCLChiTietKiemTraHauKiemATTP view
            modelBuilder.Entity<QLCLChiTietKiemTraHauKiemATTP>()
                .ToView("QLCLViewChiTietKiemTraHauKiemATTP")
                .HasKey(e => e.id);

        }
    }
} 