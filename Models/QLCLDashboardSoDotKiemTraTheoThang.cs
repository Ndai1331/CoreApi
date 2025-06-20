using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreAPI.Models
{
    /// <summary>
    /// Model for QLCL Dashboard Loai Hinh Co So
    /// </summary>
    [Table("QLCLViewDashboardSoDotKiemTraTheoThang")]
    public class QLCLDashboardSoDotKiemTraTheoThang
    {
        /// <summary>
        /// Loại hình cơ sở code
        /// </summary>
        public int t1 { get; set; } = 0;

        public int t2 { get; set; } = 0;

        public int t3 { get; set; } = 0;

        public int t4 { get; set; } = 0;

        public int t5 { get; set; } = 0;

        public int t6 { get; set; } = 0;

        public int t7 { get; set; } = 0;

        public int t8 { get; set; } = 0;

        public int t9 { get; set; } = 0;

        public int t10 { get; set; } = 0;

        public int t11 { get; set; } = 0;

        public int t12 { get; set; } = 0;
    }
} 