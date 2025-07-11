using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreAPI.Models
{
    /// <summary>
    /// Model for QLCL Bao Cao Bien Dong Gia
    /// </summary>
    public class QLCLBaoCaoBienDongGia
    {
        public DateTime? ngay_ghi_nhan { get; set; }
        public int? loai { get; set; }
        public string? ten_san_pham { get; set; }
        public string? nha_cung_cap { get; set; }
        public string? dia_diem { get; set; }
        public string? don_vi_tinh { get; set; }
        public double? gia_mua_vao { get; set; }
        public double? gia_ban_ra { get; set; }
        public double? bien_dong { get; set; }
        
    }
} 