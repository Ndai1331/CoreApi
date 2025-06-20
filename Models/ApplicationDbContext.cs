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

        /// <summary>
        /// QLCL Bao Cao Tham Dinh Cap GCN
        /// </summary>
        public DbSet<QLCLBaoCaoThamDinhCapGCN> QLCLBaoCaoThamDinhCapGCN { get; set; }

        /// <summary>
        /// QLCL Co So Khong Duoc Cap GCN
        /// </summary>
        public DbSet<FunctionCoSoKhongDuocCapGCN> QLCLCoSoKhongDuocCapGCN { get; set; }

        /// <summary>
        /// QLCL Dashboard
        /// </summary>
        public DbSet<QLCLDashboard> QLCLDashboard { get; set; }
        public DbSet<QLCLDashboardLoaiHinhCoSo> QLCLDashboardLoaiHinhCoSo { get; set; }

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

            // Configure QLCLBaoCaoThamDinhCapGCN view
            modelBuilder.Entity<QLCLBaoCaoThamDinhCapGCN>()
                .ToView("QLCLViewBaoCaoThamDinhCapGCN")
                .HasKey(e => e.thang);

            // Configure QLCLCoSoKhongDuocCapGCN view
            modelBuilder.Entity<FunctionCoSoKhongDuocCapGCN>()
                .ToView("QLCLViewCoSoKhongDuocCapGCN")
                .HasKey(e => e.id);

            // Configure QLCLDashboard view
            modelBuilder.Entity<QLCLDashboard>()
                .ToView("QLCLViewDashboard")
                .HasKey(e => e.so_luong_co_so);

            // Configure QLCLDashboardLoaiHinhCoSo view
            modelBuilder.Entity<QLCLDashboardLoaiHinhCoSo>()
                .ToView("QLCLViewDashboardLoaiHinhCoSo")
                .HasKey(e => e.code);
        }
    }
} 