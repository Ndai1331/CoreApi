using CoreAPI.Models;
using CoreAPI.Models.BaseResponse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Threading.Tasks;

namespace CoreAPI.Controllers
{
    /// <summary>
    /// Controller for QLCL Dashboard
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class QLCLDashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<QLCLDashboardController> _logger;

        public QLCLDashboardController(ApplicationDbContext context, ILogger<QLCLDashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<DirectusResponse<QLCLDashboard>> GetWithFilter(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int? province = null,
            [FromQuery] string wards = null)
        {
            var response = new DirectusResponse<QLCLDashboard>();

            try
            {
                // Get main dashboard data
                var dashboardData = await GetDashboardData(fromDate, toDate, province, wards);

                // Get additional data
                var loaiHinhCoSoItems = await GetLoaiHinhCoSoData(fromDate, toDate, province, wards);
                var gcnSapHetHanItems = await GetGCNSapHetHanData(province, wards);
                var soDotKiemTraTheoThangItems = await GetSoDotKiemTraTheoThangData(province, wards);

                // Combine data
                foreach (var item in dashboardData)
                {
                    item.loai_hinh_co_so = loaiHinhCoSoItems;
                    item.gcn_sap_het_han = gcnSapHetHanItems;
                    item.so_dot_kiem_tra_theo_thang = soDotKiemTraTheoThangItems;
                }

                response.Data = dashboardData;
                response.Meta = CreateMetaData(dashboardData.Count, fromDate, toDate);
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting QLCL Dashboard data with filter");
                response.Errors.Add(CreateErrorResponse(ex));
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return response;
        }

        /// <summary>
        /// Get detailed information about establishments that were not granted certificates
        /// </summary>
        /// <param name="fromDate">Start date (optional)</param>
        /// <param name="toDate">End date (optional)</param>
        /// <param name="province">Province ID (optional)</param>
        /// <param name="wards">Ward ID (optional)</param>
        /// <param name="thangNam">Search string (optional)</param>
        /// <param name="offset">Pagination offset</param>
        /// <param name="limit">Pagination limit</param>
        /// <returns>Detailed establishment information</returns>
        [HttpGet("coSoKhongCapGCN")]
        public async Task<DirectusResponse<FunctionCoSoKhongDuocCapGCN>> GetCoSoKhongCapGCN(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int? province = null,
            [FromQuery] string wards = null,
            [FromQuery] string thangNam = null,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 10)
        {
            var response = new DirectusResponse<FunctionCoSoKhongDuocCapGCN>();

            try
            {
                var items = await GetCoSoKhongCapGCNData(fromDate, toDate, province, wards, thangNam, offset, limit);
                var totalCount = await GetCoSoKhongCapGCNCount(fromDate, toDate, province, wards, thangNam);

                response.Data = items;
                response.Meta = CreateMetaDataWithPagination(totalCount, items.Count, offset, limit, fromDate, toDate, province, wards, thangNam);
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting QLCL Co So Khong Cap GCN data");
                response.Errors.Add(CreateErrorResponse(ex));
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return response;
        }

        #region Private Methods

        private async Task<List<QLCLDashboard>> GetDashboardData(DateTime? fromDate, DateTime? toDate, int? province, string wards)
        {
            var sql = "SELECT * FROM FunctionDashboardQLCL(@FromDate, @ToDate, @Province, @Ward)";
            return await ExecuteQueryAsync<QLCLDashboard>(sql, new Dictionary<string, object>
            {
                ["@FromDate"] = fromDate ?? (object)DBNull.Value,
                ["@ToDate"] = toDate ?? (object)DBNull.Value,
                ["@Province"] = province ?? (object)DBNull.Value,
                ["@Ward"] = wards ?? (object)DBNull.Value
            }, reader => new QLCLDashboard
            {
                so_luong_co_so = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                so_luong_co_so_dat_chung_nhan = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                so_luong_vu_vi_pham = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                so_dot_kiem_tra = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
            });
        }

        private async Task<List<QLCLDashboardLoaiHinhCoSo>> GetLoaiHinhCoSoData(DateTime? fromDate, DateTime? toDate, int? province, string wards)
        {
            var sql = "SELECT * FROM FunctionDashboardLoaiHinhCoSoQLCL(@FromDate, @ToDate, @Province, @Ward)";
            return await ExecuteQueryAsync<QLCLDashboardLoaiHinhCoSo>(sql, new Dictionary<string, object>
            {
                ["@FromDate"] = fromDate ?? (object)DBNull.Value,
                ["@ToDate"] = toDate ?? (object)DBNull.Value,
                ["@Province"] = province ?? (object)DBNull.Value,
                ["@Ward"] = wards ?? (object)DBNull.Value
            }, reader => new QLCLDashboardLoaiHinhCoSo
            {
                code = reader.GetString(0),
                name = reader.GetString(1),
                so_luong_co_so = reader.GetInt32(2)
            });
        }

        private async Task<List<QLCLDashboardGCNSapHetHan>> GetGCNSapHetHanData(int? province, string wards)
        {
            var sql = "SELECT * FROM FunctionDashboardGCNSapHetHan( @Province, @Ward )";
            return await ExecuteQueryAsync<QLCLDashboardGCNSapHetHan>(sql, new Dictionary<string, object>()
            {
                ["@Province"] = province ?? (object)DBNull.Value,
                ["@Ward"] = wards ?? (object)DBNull.Value
            }, reader => new QLCLDashboardGCNSapHetHan
            {
                id = reader.GetInt32(0),
                name = reader.GetString(1),
                so_giay_chung_nhan = reader.GetString(2),
                ngay_het_hieu_luc = reader.GetDateTime(3),
                so_ngay_con_lai = reader.GetInt32(4)
            });
        }

        private async Task<List<QLCLDashboardSoDotKiemTraTheoThang>> GetSoDotKiemTraTheoThangData(int? province, string wards)
        {
            var sql = "SELECT * FROM FunctionSoDotKiemTraTheoThang(@Year, @Province, @Ward)";
            return await ExecuteQueryAsync<QLCLDashboardSoDotKiemTraTheoThang>(sql, new Dictionary<string, object>
            {
                ["@Province"] = province ?? (object)DBNull.Value,
                ["@Ward"] = wards ?? (object)DBNull.Value,
                ["@Year"] = DateTime.Now.Year
            }, reader => new QLCLDashboardSoDotKiemTraTheoThang
            {
                t1 = reader.GetInt32(0),
                t2 = reader.GetInt32(1),
                t3 = reader.GetInt32(2),
                t4 = reader.GetInt32(3),
                t5 = reader.GetInt32(4),
                t6 = reader.GetInt32(5),
                t7 = reader.GetInt32(6),
                t8 = reader.GetInt32(7),
                t9 = reader.GetInt32(8),
                t10 = reader.GetInt32(9),
                t11 = reader.GetInt32(10),
                t12 = reader.GetInt32(11)
            });
        }

        private async Task<List<FunctionCoSoKhongDuocCapGCN>> GetCoSoKhongCapGCNData(DateTime? fromDate, DateTime? toDate, int? province, string wards, string thangNam, int offset, int limit)
        {
            var sql = @"
                SELECT * FROM QLCLFunctionBaoCaoCoSoKhongCapGCN(@FromDate, @ToDate, @Province, @Ward, @ThangNam)
                ORDER BY id DESC
                OFFSET @Offset ROWS
                FETCH NEXT @Limit ROWS ONLY";

            return await ExecuteQueryAsync<FunctionCoSoKhongDuocCapGCN>(sql, new Dictionary<string, object>
            {
                ["@FromDate"] = fromDate ?? (object)DBNull.Value,
                ["@ToDate"] = toDate ?? (object)DBNull.Value,
                ["@Province"] = province ?? (object)DBNull.Value,
                ["@Ward"] = wards ?? (object)DBNull.Value,
                ["@ThangNam"] = thangNam ?? (object)DBNull.Value,
                ["@Offset"] = offset,
                ["@Limit"] = limit
            }, reader => new FunctionCoSoKhongDuocCapGCN
            {
                id = reader.GetInt32(0),
                code = reader.GetString(1),
                name = reader.GetString(2),
                dia_chi = reader.GetString(3),
                dien_thoai = reader.GetString(4),
                dai_dien = reader.GetString(5)
            });
        }

        private async Task<int> GetCoSoKhongCapGCNCount(DateTime? fromDate, DateTime? toDate, int? province, string wards, string thangNam)
        {
            var sql = "SELECT COUNT(*) FROM QLCLFunctionBaoCaoCoSoKhongCapGCN(@FromDate, @ToDate, @Province, @Ward, @ThangNam)";
            return await ExecuteScalarAsync<int>(sql, new Dictionary<string, object>
            {
                ["@FromDate"] = fromDate ?? (object)DBNull.Value,
                ["@ToDate"] = toDate ?? (object)DBNull.Value,
                ["@Province"] = province ?? (object)DBNull.Value,
                ["@Ward"] = wards ?? (object)DBNull.Value,
                ["@ThangNam"] = thangNam ?? (object)DBNull.Value
            });
        }

        private async Task<List<T>> ExecuteQueryAsync<T>(string sql, Dictionary<string, object> parameters, Func<IDataReader, T> mapper)
        {
            var result = new List<T>();
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;

            foreach (var param in parameters)
            {
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(param.Key, param.Value));
            }

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

        private async Task<T> ExecuteScalarAsync<T>(string sql, Dictionary<string, object> parameters)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;

            foreach (var param in parameters)
            {
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(param.Key, param.Value));
            }

            if (command.Connection.State != ConnectionState.Open)
            {
                await command.Connection.OpenAsync();
            }

            var result = await command.ExecuteScalarAsync();
            return result != null ? (T)Convert.ChangeType(result, typeof(T)) : default(T);
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

        private DirectusMeta CreateMetaDataWithPagination(int totalCount, int filterCount, int offset, int limit, DateTime? fromDate, DateTime? toDate, int? province, string wards, string thangNam)
        {
            return new DirectusMeta
            {
                total_count = totalCount,
                filter_count = filterCount,
                offset = offset,
                limit = limit,
                page_count = (int)Math.Ceiling((double)totalCount / limit),
                sort = new List<string> { "-id" },
                filter = new
                {
                    fromDate = fromDate?.ToString("yyyy-MM-dd"),
                    toDate = toDate?.ToString("yyyy-MM-dd"),
                    province = province,
                    wards = wards,
                    thangNam = thangNam
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

        #endregion
    }
}