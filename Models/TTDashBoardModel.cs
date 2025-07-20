using System.Collections.Generic;

namespace CoreAPI.Models
{
    /// <summary>
    /// Represents the dashboard model for tracking various agricultural production and business metrics.
    /// </summary>
    /// <remarks>This model includes counts of production and business facilities for fertilizers and plant
    /// protection products, as well as data on violations and damage caused by natural disasters and
    /// diseases.</remarks>
    public class TTDashBoardModel
    {
        /// <summary>
        /// Số lượng cơ sở sản xuất phân bón
        /// </summary>
        public decimal CoSoSanXuatPhanBonCount { get; set; } = 0;
        /// <summary>
        /// Số lượng cơ sở kinh doanh phân bón
        /// </summary>
        public decimal CoSoDuDieuKienBuonBanPhanBonCount { get; set; } = 0;
        /// <summary>
        /// Số lượng cơ sở sản xuất thuốc BVTV
        /// </summary>
        public decimal CoSoSanXuatThuocBVTVCount { get; set; } = 0;
        /// <summary>
        /// Số lượng cơ sở kinh doanh thuốc BVTV
        /// </summary>
        public decimal CoSoKinhDoanhThuocBVTVCount { get; set; } = 0;
        /// <summary>
        /// Số lượng cơ sở vi phạm sản xuất kinh doanh thuốc BVTV
        /// </summary>
        public decimal ViPhamSanXuatKinhDoanhThuocBVTVCount { get; set; } = 0;
        /// <summary>
        /// Dữ liệu biểu đồ tròn về diện tich thiệt hại do thiên tai dịch bệnh
        /// </summary>
        public PieChartDataModel DienTichThietHaiDoThienTaiDichBenh { get; set; } = new PieChartDataModel();
        /// <summary>
        /// Dữ liệu biểu đồ tròn về sản lượng thiệt hại do thiên tai dịch bệnh
        /// </summary>
        public PieChartDataModel SanLuongThietHaiDoThienTaiDichBenh { get; set; } = new PieChartDataModel();
    }

    /// <summary>
    /// Represents the data model for a pie chart, including series values, labels, and colors.
    /// </summary>
    /// <remarks>This model is used to store the data necessary for rendering a pie chart. Each list should
    /// have corresponding elements, where each index represents a segment of the pie chart. Ensure that the number of
    /// elements in <see cref="Series"/>, <see cref="Labels"/>, and <see cref="Colors"/> are equal to maintain
    /// consistency in the chart representation.</remarks>
    public class PieChartDataModel
    {
        public List<float> Series { get; set; } = new List<float>();
        public List<string> Labels { get; set; } = new List<string>();
        public List<string> Colors { get; set; } = new List<string>();
    }
}
