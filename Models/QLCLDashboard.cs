using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace CoreAPI.Models
{
    /// <summary>
    /// Model for QLCL Bao Cao Tham Dinh Cap GCN
    /// </summary>
    [Table("QLCLViewDashboard")]
    public class QLCLDashboard
    {
        /// <summary>
        /// Tổng số cơ sở
        /// </summary>
        [Column("so_luong_co_so")]
        public int so_luong_co_so { get; set; } = 0;

        /// <summary>
        /// Tổng số cơ sở được cấp GCN
        /// </summary>
        [Column("so_luong_co_so_dat_chung_nhan")]
        public int so_luong_co_so_dat_chung_nhan { get; set; } = 0;

        /// <summary>
        /// Tổng số cơ sở vi phạm
        /// </summary>
        [Column("so_luong_vu_vi_pham")]
        public int so_luong_vu_vi_pham { get; set; } = 0;
        
        /// <summary>
        /// Số cơ sở vi phạm
        /// </summary>
        [Column("so_dot_kiem_tra")]
        public int so_dot_kiem_tra { get; set; } = 0;

        public List<QLCLDashboardLoaiHinhCoSo>? loai_hinh_co_so { get; set; } = null;
    }
} 