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
    /// Controller for QLCL Bao Cao Tham Dinh Cap GCN
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class QLCLBaoCaoThamDinhCapGCNController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<QLCLBaoCaoThamDinhCapGCNController> _logger;

        public QLCLBaoCaoThamDinhCapGCNController(ApplicationDbContext context, ILogger<QLCLBaoCaoThamDinhCapGCNController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<DirectusResponse<QLCLBaoCaoThamDinhCapGCN>> GetWithFilter(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int? province = null,
            [FromQuery] string wards = null,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 10)
        {
            var response = new DirectusResponse<QLCLBaoCaoThamDinhCapGCN>();

            try
            {
                var items = await GetBaoCaoThamDinhCapGCNData(fromDate, toDate, province, wards, offset, limit);
                var totalCount = await GetBaoCaoThamDinhCapGCNCount(fromDate, toDate, province, wards);

                response.Data = items;
                response.Meta = CreateMetaDataWithPagination(totalCount, items.Count, offset, limit, fromDate, toDate, province, wards);
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting QLCL Bao Cao Tham Dinh Cap GCN data with filter");
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
        /// <param name="thangNam">month string (optional)</param>
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
        private async Task<List<QLCLBaoCaoThamDinhCapGCN>> GetBaoCaoThamDinhCapGCNData(DateTime? fromDate, DateTime? toDate, int? province, string wards, int offset, int limit)
        {
            var sql = @"
                SELECT * FROM QLCLFunctionBaoCaoThamDinhCapGCN(@FromDate, @ToDate, @Province, @Ward)
                ORDER BY thang DESC
                OFFSET @Offset ROWS
                FETCH NEXT @Limit ROWS ONLY";

            return await ExecuteQueryAsync<QLCLBaoCaoThamDinhCapGCN>(sql, new Dictionary<string, object>
            {
                ["@FromDate"] = fromDate ?? (object)DBNull.Value,
                ["@ToDate"] = toDate ?? (object)DBNull.Value,
                ["@Province"] = province ?? (object)DBNull.Value,
                ["@Ward"] = wards ?? (object)DBNull.Value,
                ["@Offset"] = offset,
                ["@Limit"] = limit
            }, reader => new QLCLBaoCaoThamDinhCapGCN
            {
                thang = reader.GetString(0),
                tong_co_so_tham_dinh = reader.GetInt32(1),
                so_dat = reader.GetInt32(2),
                so_khong_dat = reader.GetInt32(3),
                so_co_so_duoc_cap_gcn = reader.GetInt32(4),
                ty_le_co_so_duoc_cap_gcn = reader.GetDecimal(5)
            });
        }

        private async Task<int> GetBaoCaoThamDinhCapGCNCount(DateTime? fromDate, DateTime? toDate, int? province, string wards)
        {
            var sql = "SELECT COUNT(*) FROM QLCLFunctionBaoCaoThamDinhCapGCN(@FromDate, @ToDate, @Province, @Ward)";
            return await ExecuteScalarAsync<int>(sql, new Dictionary<string, object>
            {
                ["@FromDate"] = fromDate ?? (object)DBNull.Value,
                ["@ToDate"] = toDate ?? (object)DBNull.Value,
                ["@Province"] = province ?? (object)DBNull.Value,
                ["@Ward"] = wards ?? (object)DBNull.Value
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

        private DirectusMeta CreateMetaDataWithPagination(int totalCount, int filterCount, int offset, int limit, DateTime? fromDate, DateTime? toDate, int? province, string wards, string thangNam = null)
        {
            return new DirectusMeta
            {
                total_count = totalCount,
                filter_count = filterCount,
                offset = offset,
                limit = limit,
                page_count = (int)Math.Ceiling((double)totalCount / limit),
                sort = new List<string> { "-thang" },
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