using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace CoreAPI.Models
{
    /// <summary>
    /// Model for QLCL Bao Cao Tham Dinh Cap GCN
    /// </summary>
    public class QLCLDashboard
    {
        /// <summary>
        /// Tổng số cơ sở
        /// </summary>
        public int? so_luong_co_so { get; set; } = 0;

        /// <summary>
        /// Tổng số cơ sở được cấp GCN
        /// </summary>
        public int? so_luong_co_so_dat_chung_nhan { get; set; } = 0;

        /// <summary>
        /// Tổng số cơ sở vi phạm
        /// </summary>
        public int? so_luong_vu_vi_pham { get; set; } = 0;
        
        /// <summary>
        /// Số cơ sở vi phạm
        /// </summary>
        public int? so_dot_kiem_tra { get; set; } = 0;

        /// <summary>
        /// Danh sách loại hình cơ sở
        /// </summary>
        public List<QLCLDashboardLoaiHinhCoSo>? loai_hinh_co_so { get; set; } = null;

        /// <summary>
        /// Danh sách cơ sở có GCN sắp hết hạn
        /// </summary>
        public List<QLCLDashboardGCNSapHetHan>? gcn_sap_het_han { get; set; } = null;

        public List<QLCLDashboardSoDotKiemTraTheoThang>? so_dot_kiem_tra_theo_thang { get; set; } = null;
    }
} 