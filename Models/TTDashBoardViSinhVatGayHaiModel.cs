namespace CoreAPI.Models
{
    /// <summary>
    /// Represents a model for tracking harmful microorganisms and their impact on agricultural areas.
    /// </summary>
    /// <remarks>This model is used to store information about harmful microorganisms, including their name,
    /// the total affected area, and the total yield loss. It is typically used in agricultural dashboards to assess and
    /// manage the impact of these microorganisms.</remarks>
    public class TTDashBoardViSinhVatGayHaiModel
    {
        /// <summary>
        /// Tên vi sinh vật gây hại
        /// </summary>
        public string? ViSinhVatGayHai { get; set; }
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
