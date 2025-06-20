using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreAPI.Models
{
    /// <summary>
    /// Model for establishments with certificates expiring soon
    /// </summary>
    public class QLCLDashboardGCNSapHetHan
    {
        /// <summary>
        /// Establishment ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Establishment name
        /// </summary>
        public string name { get; set; } = string.Empty;

        /// <summary>
        /// Certificate number
        /// </summary>
        public string so_giay_chung_nhan { get; set; } = string.Empty;

        /// <summary>
        /// Certificate expiry date
        /// </summary>
        public DateTime ngay_het_hieu_luc { get; set; }

        /// <summary>
        /// Days remaining until expiry
        /// </summary>
        public int so_ngay_con_lai { get; set; }
    }
} 