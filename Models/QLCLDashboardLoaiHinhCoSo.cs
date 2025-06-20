using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreAPI.Models
{
    /// <summary>
    /// Model for QLCL Dashboard Loai Hinh Co So
    /// </summary>
    [Table("QLCLViewDashboardLoaiHinhCoSo")]
    public class QLCLDashboardLoaiHinhCoSo
    {
        /// <summary>
        /// Loại hình cơ sở code
        /// </summary>
        [Column("code")]
        public string code { get; set; } = string.Empty;

        /// <summary>
        /// Loại hình cơ sở name
        /// </summary>
        [Column("name")]
        public string name { get; set; } = string.Empty;

        /// <summary>
        /// Số lượng cơ sở
        /// </summary>
        [Column("so_luong_co_so")]
        public int so_luong_co_so { get; set; } = 0;
    }
} 