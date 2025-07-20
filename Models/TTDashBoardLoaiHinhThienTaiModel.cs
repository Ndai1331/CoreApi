namespace CoreAPI.Models
{
    /// <summary>
    /// Represents a model for dashboard data related to different types of natural disasters.
    /// </summary>
    public class TTDashBoardLoaiHinhThienTaiModel
    {
        /// <summary>
        /// Tên loại hình thiên tai
        /// </summary>
        public string? LoaiHinhThienTai { get; set; }
        /// <summary>
        /// Tổng diện tích bị thiệt hại
        /// </summary>
        public decimal TongDienTich { get; set; } = 0;
        /// <summary>
        /// Tổng sản lượng bị thiệt hại
        /// </summary>
        public decimal TongSanLuong { get; set; } = 0;
    }
}
