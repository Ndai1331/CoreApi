using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPI.Models;
using CoreAPI.Models.BaseResponse;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Data;

namespace CoreAPI.Controllers
{
    /// <summary>
    /// Controller for QLCL Bao Cao Kiem Tra Hau Kiem ATTP
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class QLCLBaoCaoBienDongGiaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<QLCLBaoCaoBienDongGiaController> _logger;

        public QLCLBaoCaoBienDongGiaController(ApplicationDbContext context, ILogger<QLCLBaoCaoBienDongGiaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<DirectusResponse<QLCLBaoCaoBienDongGia>> GetWithFilter(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int? province = null,
            [FromQuery] int? ward = null,
            [FromQuery] int? loai = null,
            [FromQuery] string? tenSanPham = null,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 10)
        {
            var response = new DirectusResponse<QLCLBaoCaoBienDongGia>();

            try
            {
                var items = await GetBaoCaoBienDongGiaData(fromDate, toDate, province, ward, loai, tenSanPham, offset, limit);
                var totalCount = await GetBaoCaoBienDongGiaCount(fromDate, toDate, province, ward, loai, tenSanPham);

                response.Data = items;
                response.Meta = CreateMetaDataWithPagination(totalCount, items.Count, offset, limit, fromDate, toDate, province, ward, loai, tenSanPham);
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting QLCL Bao Cao Bien Dong Gia data with filter");
                response.Errors.Add(CreateErrorResponse(ex));
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return response;
        }

        #region Private Methods

        private async Task<List<QLCLBaoCaoBienDongGia>> GetBaoCaoBienDongGiaData(DateTime? fromDate, DateTime? toDate, int? province, int? ward, int? loai, string? tenSanPham, int offset, int limit)
        {
            var sql = @"
                SELECT * FROM CreateFunctionDashboardBienDongGia(
                @FromDate, @ToDate, @Province, @Ward, @Loai, @TenSanPham)
                ORDER BY ngay_ghi_nhan DESC
                OFFSET @Offset ROWS
                FETCH NEXT @Limit ROWS ONLY";

            return await ExecuteQueryAsync<QLCLBaoCaoBienDongGia>(sql, new Dictionary<string, object>
            {
                ["@FromDate"] = fromDate ?? (object)DBNull.Value,
                ["@ToDate"] = toDate ?? (object)DBNull.Value,
                ["@Province"] = province ?? (object)DBNull.Value,
                ["@Ward"] = ward ?? (object)DBNull.Value,
                ["@Loai"] = loai ?? (object)DBNull.Value,
                ["@TenSanPham"] = tenSanPham ?? (object)DBNull.Value,
                ["@Offset"] = offset,
                ["@Limit"] = limit
            }, reader => new QLCLBaoCaoBienDongGia
            {
                ngay_ghi_nhan = reader.GetDateTime(0),
                loai = reader.GetInt32(1),
                ten_san_pham = reader.GetString(2),
                nha_cung_cap = reader.GetString(3),
                dia_diem = reader.GetString(4),
                don_vi_tinh = reader.GetString(5),
                gia_mua_vao = reader.GetDouble(6),
                gia_ban_ra = reader.GetDouble(7),
                bien_dong = reader.GetDouble(8)
            });
        }

        private async Task<int> GetBaoCaoBienDongGiaCount(DateTime? fromDate, DateTime? toDate, int? province, int? ward, int? loai, string? tenSanPham)
        {
            var sql = "SELECT COUNT(*) FROM CreateFunctionDashboardBienDongGia(@FromDate, @ToDate, @Province, @Ward, @Loai, @TenSanPham)";
            return await ExecuteScalarAsync<int>(sql, new Dictionary<string, object>
            {
                ["@FromDate"] = fromDate ?? (object)DBNull.Value,
                ["@ToDate"] = toDate ?? (object)DBNull.Value,
                ["@Province"] = province ?? (object)DBNull.Value,
                ["@Ward"] = ward ?? (object)DBNull.Value,
                ["@Loai"] = loai ?? (object)DBNull.Value,
                ["@TenSanPham"] = tenSanPham ?? (object)DBNull.Value
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
                command.Connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
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
                command.Connection.Open();

            var result = command.ExecuteScalar();
            return result != null ? (T)Convert.ChangeType(result, typeof(T)) : default(T);
        }

        private DirectusMeta CreateMetaData(int totalCount, string thangNam, int? province, int? ward)
        {
            return new DirectusMeta
            {
                total_count = totalCount,
                sort = new List<string> { "code" },
                filter = new
                {
                    thangNam = thangNam,
                    province = province,
                    ward = ward
                }
            };
        }

        private DirectusMeta CreateMetaDataWithPagination(int totalCount, int filterCount, int offset, int limit, DateTime? fromDate, DateTime? toDate, 
        int? province, int? ward, int? loai, string? tenSanPham)
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
                    ward = ward,
                    loai = loai,
                    tenSanPham = tenSanPham
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