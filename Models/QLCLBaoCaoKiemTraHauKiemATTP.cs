using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreAPI.Models
{
    /// <summary>
    /// Model for QLCL Bao Cao Kiem Tra Hau Kiem ATTP
    /// </summary>
    public class QLCLBaoCaoKiemTraHauKiemATTP
    {
        public string thang { get; set; } = string.Empty;
        public int province { get; set; }
        
        /// <summary>
        /// Quận/huyện
        /// </summary>
        public int ward { get; set; }
        
        /// <summary>
        /// Tổng số đợt kiểm tra
        /// </summary>
        public int tong_dot_kiem_tra { get; set; }
        
        /// <summary>
        /// Tổng số cơ sở kiểm tra
        /// </summary>
        public int tong_co_so_kiem_tra { get; set; }
        
        /// <summary>
        /// Số cơ sở vi phạm
        /// </summary>
        public int so_vi_pham { get; set; }
        
        /// <summary>
        /// Số cơ sở chấp hành
        /// </summary>
        public int so_chap_hanh { get; set; }
        
        /// <summary>
        /// Số cơ sở đạt
        /// </summary>
        public int so_dat { get; set; }
        
        /// <summary>
        /// Số cơ sở không đạt
        /// </summary>
        public int so_khong_dat { get; set; }
    }

    public class QLCLDetailCoSoKiemTraHauKiemATTP
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string dia_chi { get; set; }
        public string dien_thoai { get; set; }
        public string dai_dien { get; set; }
    }

    public class QLCLChiTietKiemTraHauKiemATTP
    {
        public int id { get; set; }
        public string san_pham { get; set; }
        public int? so_luong_mau { get; set; } = 0;
        public string chi_tieu { get; set; }
        public int? so_mau_khong_dat { get; set; } = 0;
        public string chi_tieu_vi_pham { get; set; }
        public string muc_phat_hien { get; set; }
    }
} 