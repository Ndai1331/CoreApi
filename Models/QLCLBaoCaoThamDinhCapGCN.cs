using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreAPI.Models
{
    /// <summary>
    /// Model for QLCL Bao Cao Tham Dinh Cap GCN
    /// </summary>
    public class QLCLBaoCaoThamDinhCapGCN
    {
        /// <summary>
        /// Tháng báo cáo (yyyy-MM format)
        /// </summary>
        public string thang { get; set; } = string.Empty;
        

        /// <summary>
        /// Tổng số đợt kiểm tra
        /// </summary>
        public int tong_co_so_tham_dinh { get; set; } = 0;
        
        /// <summary>
        /// Tổng số cơ sở kiểm tra
        /// </summary>
        public int so_dat { get; set; } = 0;
        
        /// <summary>
        /// Số cơ sở vi phạm
        /// </summary>
        public int so_khong_dat { get; set; } = 0;
       
        /// <summary>
        /// Số cơ sở đạt
        /// </summary>
        public int so_co_so_duoc_cap_gcn { get; set; } = 0;
        
        /// <summary>
        /// Tỷ lệ cơ sở được cấp GCN (percentage format - 0.00 to 100.00)
        /// </summary>
        public decimal ty_le_co_so_duoc_cap_gcn { get; set; } = 0;
    }

    /// <summary>
    /// Model for establishments that were not granted certificates
    /// </summary>
    public class FunctionCoSoKhongDuocCapGCN
    {
        /// <summary>
        /// Establishment ID
        /// </summary>
        public int id { get; set; }
        
        /// <summary>
        /// Establishment code
        /// </summary>
        public string code { get; set; } = string.Empty;
        
        /// <summary>
        /// Establishment name
        /// </summary>
        public string name { get; set; } = string.Empty;
        
        /// <summary>
        /// Establishment address
        /// </summary>
        public string dia_chi { get; set; } = string.Empty;
        
        /// <summary>
        /// Establishment phone number
        /// </summary>
        public string dien_thoai { get; set; } = string.Empty;
        
        /// <summary>
        /// Establishment representative
        /// </summary>
        public string dai_dien { get; set; } = string.Empty;
    }
} 