using CoreAPI.Models;
using CoreAPI.Models.BaseResponse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CoreAPI.Controllers
{
    /// <summary>
    /// Controller for TT Dashboard
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TTDashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TTDashboardController> _logger;
        private static readonly List<string> _defaultPieChartColors = new List<string>
        {
            "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF", "#FF9F40",
            "#C9CBCF", "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF",
            "#FF9F40", "#C9CBCF", "#B2FF66", "#66FFB2", "#66B2FF", "#B266FF",
            "#FF66B2", "#FFB266", "#A2EB36", "#6384FF", "#CE56FF", "#C0C04B",
            "#66FF99", "#FF6666", "#66FF66", "#6666FF", "#FF66FF", "#66FFFF",
            "#FFFF66"
        };

        public TTDashboardController(ApplicationDbContext context, ILogger<TTDashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<DirectusResponse<TTDashBoardModel>> GetWithFilter(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var response = new DirectusResponse<TTDashBoardModel>();

            try
            {
                // Get main dashboard data
                var dashboardData = await GetDashboardData();

                // Get additional data
                var thietHaiDoThienTai = await GetThietHaiDoThienTaiDichBenhData();
                var thietHaiDoDichBenh = await GetCoSoBiDichBenhData();

                // Combine data
                foreach (var item in dashboardData)
                {
                    item.SanLuongThietHaiDoThienTaiDichBenh = new PieChartDataModel();
                    item.DienTichThietHaiDoThienTaiDichBenh = new PieChartDataModel();

                    if (thietHaiDoThienTai != null && thietHaiDoThienTai.Any())
                    {
                        foreach (var thietHai in thietHaiDoThienTai)
                        {
                            if (thietHai.TongDienTich > 0)
                            {
                                item.DienTichThietHaiDoThienTaiDichBenh.Series.Add((float)thietHai.TongDienTich);
                                item.DienTichThietHaiDoThienTaiDichBenh.Labels.Add(thietHai.LoaiHinhThienTai ?? "Không xác định");
                            }
                            if (thietHai.TongSanLuong > 0)
                            {
                                item.SanLuongThietHaiDoThienTaiDichBenh.Series.Add((float)thietHai.TongSanLuong);
                                item.SanLuongThietHaiDoThienTaiDichBenh.Labels.Add(thietHai.LoaiHinhThienTai ?? "Không xác định");
                            }
                        }
                    }

                    if (thietHaiDoDichBenh != null && thietHaiDoDichBenh.Any())
                    {
                        foreach (var thietHai in thietHaiDoDichBenh)
                        {
                            if (thietHai.TongDienTich > 0)
                            {
                                item.DienTichThietHaiDoThienTaiDichBenh.Series.Add((float)thietHai.TongDienTich);
                                item.DienTichThietHaiDoThienTaiDichBenh.Labels.Add(thietHai.ViSinhVatGayHai ?? "Không xác định");
                            }
                            if (thietHai.TongSanLuong > 0)
                            {
                                item.SanLuongThietHaiDoThienTaiDichBenh.Series.Add((float)thietHai.TongSanLuong);
                                item.SanLuongThietHaiDoThienTaiDichBenh.Labels.Add(thietHai.ViSinhVatGayHai ?? "Không xác định");
                            }
                        }
                    }

                    item.DienTichThietHaiDoThienTaiDichBenh.Colors = _defaultPieChartColors.Take(item.DienTichThietHaiDoThienTaiDichBenh.Labels.Count).ToList();
                    item.SanLuongThietHaiDoThienTaiDichBenh.Colors = _defaultPieChartColors.Take(item.SanLuongThietHaiDoThienTaiDichBenh.Labels.Count).ToList();
                }

                response.Data = dashboardData;
                response.Meta = CreateMetaData(dashboardData.Count, fromDate, toDate);
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting TT Dashboard data with filter");
                response.Errors.Add(CreateErrorResponse(ex));
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return response;
        }


        private async Task<List<TTDashBoardModel>> GetDashboardData()
        {
            var sql = "SELECT * FROM TTFunctionCountDashboard()";
            return await ExecuteQueryAsync(sql, mapper: reader => new TTDashBoardModel
            {
                CoSoSanXuatPhanBonCount = reader.GetInt32(0),
                CoSoDuDieuKienBuonBanPhanBonCount = reader.GetInt32(1),
                CoSoSanXuatThuocBVTVCount = reader.GetInt32(2),
                CoSoKinhDoanhThuocBVTVCount = reader.GetInt32(3),
                ViPhamSanXuatKinhDoanhThuocBVTVCount = reader.GetInt32(4)
            });
        }

        private async Task<List<TTDashBoardLoaiHinhThienTaiModel>> GetThietHaiDoThienTaiDichBenhData()
        {
            var sql = "SELECT * FROM TTFunctionDashboardDuLieuThietHaiThienTai()";
            return await ExecuteQueryAsync(sql, mapper: reader => new TTDashBoardLoaiHinhThienTaiModel
            {
                LoaiHinhThienTai = reader.GetString(0),
                TongDienTich = reader.GetDecimal(1),
                TongSanLuong = reader.GetDecimal(2)
            });
        }

        private async Task<List<TTDashBoardViSinhVatGayHaiModel>> GetCoSoBiDichBenhData()
        {
            var sql = "SELECT * FROM TTFunctionDashboardCoSoBiDichBenh()";
            return await ExecuteQueryAsync(sql, mapper: reader => new TTDashBoardViSinhVatGayHaiModel
            {
                ViSinhVatGayHai = reader.GetString(0),
                TongDienTich = reader.GetDecimal(1),
                TongSanLuong = reader.GetDecimal(2)
            });
        }

        private async Task<List<T>> ExecuteQueryAsync<T>(string sql, Func<IDataReader, T> mapper)
        {
            var result = new List<T>();
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;

            if (command.Connection.State != ConnectionState.Open)
            {
                await command.Connection.OpenAsync();
            }

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(mapper(reader));
            }

            return result;
        }

        private DirectusMeta CreateMetaData(int totalCount, DateTime? fromDate, DateTime? toDate)
        {
            return new DirectusMeta
            {
                total_count = totalCount,
                filter_count = totalCount,
                offset = 0,
                limit = 10,
                page_count = 1,
                sort = new List<string> { "-thang" },
                filter = new
                {
                    fromDate = fromDate?.ToString("yyyy-MM-dd"),
                    toDate = toDate?.ToString("yyyy-MM-dd")
                }
            };
        }

        private ErrorResponse CreateErrorResponse(Exception ex)
        {
            return new ErrorResponse
            {
                Message = "Internal server error",
                Code = "INTERNAL_ERROR",
                Reason = ex.Message,
                Extensions = new ExtensionsResponse
                {
                    code = "INTERNAL_ERROR",
                    reason = ex.Message
                }
            };
        }
    }
}