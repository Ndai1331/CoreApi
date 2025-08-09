using CoreAPI.Models;
using CoreAPI.Models.BaseResponse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreAPI.Controllers
{
    /// <summary>
    /// Controller for QLCL Bao Cao Kiem Tra Hau Kiem ATTP
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class QLCLBaoCaoKiemTraHauKiemATTPController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<QLCLBaoCaoKiemTraHauKiemATTPController> _logger;

        public QLCLBaoCaoKiemTraHauKiemATTPController(ApplicationDbContext context, ILogger<QLCLBaoCaoKiemTraHauKiemATTPController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<DirectusResponse<QLCLBaoCaoKiemTraHauKiemATTP>> GetWithFilter(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int? province = null,
            [FromQuery] string wards = null,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 10)
        {
            var response = new DirectusResponse<QLCLBaoCaoKiemTraHauKiemATTP>();

            try
            {
                var items = await GetBaoCaoKiemTraHauKiemATTPData(fromDate, toDate, province, wards, offset, limit);
                var totalCount = await GetBaoCaoKiemTraHauKiemATTPCount(fromDate, toDate, province, wards);

                response.Data = items;
                response.Meta = CreateMetaDataWithPagination(totalCount, items.Count, offset, limit, fromDate, toDate, province, wards);
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting QLCL Bao Cao Kiem Tra Hau Kiem ATTP data with filter");
                response.Errors.Add(CreateErrorResponse(ex));
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return response;
        }

        /// <summary>
        /// Get detailed information about establishments for a specific month, province, and wards
        /// </summary>
        /// <param name="thangNam">Month and year in format 'yyyy-MM'</param>
        /// <param name="province">Province ID (optional)</param>
        /// <param name="wards">Ward ID (optional)</param>
        /// <returns>Detailed establishment information</returns>
        [HttpGet("detail")]
        public async Task<DirectusResponse<QLCLDetailCoSoKiemTraHauKiemATTP>> GetDetail(
            [FromQuery] string thangNam,
            [FromQuery] int? province = null,
            [FromQuery] string wards = null)
        {
            var response = new DirectusResponse<QLCLDetailCoSoKiemTraHauKiemATTP>();

            try
            {
                // Validate thangNam format
                if (string.IsNullOrEmpty(thangNam) || !Regex.IsMatch(thangNam, @"^\d{4}-\d{2}$"))
                {
                    response.Errors.Add(new ErrorResponse
                    {
                        Message = "Invalid thangNam format. Use 'yyyy-MM' format",
                        Code = "INVALID_FORMAT",
                        Reason = "thangNam must be in 'yyyy-MM' format"
                    });
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return response;
                }

                var items = await GetDetailCoSoKiemTraHauKiemATTPData(thangNam, province, wards);
                var totalCount = await GetDetailCoSoKiemTraHauKiemATTPCount(thangNam, province, wards);

                response.Data = items;
                response.Meta = CreateMetaData(totalCount, thangNam, province, wards);
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting QLCL Detail Co So Kiem Tra Hau Kiem ATTP data");
                response.Errors.Add(CreateErrorResponse(ex));
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return response;
        }

        /// <summary>
        /// Get detailed inspection information with filtering capabilities
        /// </summary>
        /// <param name="fromDate">Start date (optional)</param>
        /// <param name="toDate">End date (optional)</param>
        /// <param name="province">Province ID (optional)</param>
        /// <param name="wards">Ward ID (optional)</param>
        /// <param name="stringSearch">filter text (optional)</param>
        /// <param name="offset">Pagination offset</param>
        /// <param name="limit">Pagination limit</param>
        /// <returns>Detailed inspection information</returns>
        [HttpGet("chitiet")]
        public async Task<DirectusResponse<QLCLChiTietKiemTraHauKiemATTP>> GetChiTiet(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int? province = null,
            [FromQuery] string wards = null,
            [FromQuery] string stringSearch = null,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 10)
        {
            var response = new DirectusResponse<QLCLChiTietKiemTraHauKiemATTP>();

            try
            {
                var items = await GetChiTietKiemTraHauKiemATTPData(fromDate, toDate, province, wards, stringSearch, offset, limit);
                var totalCount = await GetChiTietKiemTraHauKiemATTPCount(fromDate, toDate, province, wards, stringSearch);

                response.Data = items;
                response.Meta = CreateMetaDataWithPagination(totalCount, items.Count, offset, limit, fromDate, toDate, province, wards, stringSearch);
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting QLCL Chi Tiet Kiem Tra Hau Kiem ATTP data");
                response.Errors.Add(CreateErrorResponse(ex));
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return response;
        }

        #region Private Methods

        private async Task<List<QLCLBaoCaoKiemTraHauKiemATTP>> GetBaoCaoKiemTraHauKiemATTPData(DateTime? fromDate, DateTime? toDate, int? province, string wards, int offset, int limit)
        {
            var sql = @"
                SELECT * FROM QLCLFunctionBaoCaoKiemTraHauKiemATTP(@FromDate, @ToDate, @Province, @Ward)
                ORDER BY thang DESC, province, ward
                OFFSET @Offset ROWS
                FETCH NEXT @Limit ROWS ONLY";

            return await ExecuteQueryAsync<QLCLBaoCaoKiemTraHauKiemATTP>(sql, new Dictionary<string, object>
            {
                ["@FromDate"] = fromDate ?? (object)DBNull.Value,
                ["@ToDate"] = toDate ?? (object)DBNull.Value,
                ["@Province"] = province ?? (object)DBNull.Value,
                ["@Ward"] = wards ?? (object)DBNull.Value,
                ["@Offset"] = offset,
                ["@Limit"] = limit
            }, reader => new QLCLBaoCaoKiemTraHauKiemATTP
            {
                thang = reader.GetString(0),
                province = reader.GetInt32(1),
                ward = reader.GetInt32(2),
                tong_dot_kiem_tra = reader.GetInt32(3),
                tong_co_so_kiem_tra = reader.GetInt32(4),
                so_vi_pham = reader.GetInt32(5),
                so_chap_hanh = reader.GetInt32(6),
                so_dat = reader.GetInt32(7),
                so_khong_dat = reader.GetInt32(8)
            });
        }

        private async Task<int> GetBaoCaoKiemTraHauKiemATTPCount(DateTime? fromDate, DateTime? toDate, int? province, string wards)
        {
            var sql = "SELECT COUNT(*) FROM QLCLFunctionBaoCaoKiemTraHauKiemATTP(@FromDate, @ToDate, @Province, @Ward)";
            return await ExecuteScalarAsync<int>(sql, new Dictionary<string, object>
            {
                ["@FromDate"] = fromDate ?? (object)DBNull.Value,
                ["@ToDate"] = toDate ?? (object)DBNull.Value,
                ["@Province"] = province ?? (object)DBNull.Value,
                ["@Ward"] = wards ?? (object)DBNull.Value
            });
        }

        private async Task<List<QLCLDetailCoSoKiemTraHauKiemATTP>> GetDetailCoSoKiemTraHauKiemATTPData(string thangNam, int? province, string wards)
        {
            var sql = @"
                SELECT * FROM QLCLFunctionDetailCoSoKiemTraHauKiemATTP(@ThangNam, @Province, @Ward)
                ORDER BY code DESC";

            return await ExecuteQueryAsync<QLCLDetailCoSoKiemTraHauKiemATTP>(sql, new Dictionary<string, object>
            {
                ["@ThangNam"] = thangNam,
                ["@Province"] = province ?? (object)DBNull.Value,
                ["@Ward"] = wards ?? (object)DBNull.Value
            }, reader => new QLCLDetailCoSoKiemTraHauKiemATTP
            {
                id = reader.GetInt32(0),
                code = reader.GetString(1),
                name = reader.GetString(2),
                dia_chi = reader.GetString(3),
                dien_thoai = reader.GetString(4),
                dai_dien = reader.GetString(5)
            });
        }

        private async Task<int> GetDetailCoSoKiemTraHauKiemATTPCount(string thangNam, int? province, string wards)
        {
            var sql = "SELECT COUNT(*) FROM QLCLFunctionDetailCoSoKiemTraHauKiemATTP(@ThangNam, @Province, @Ward)";
            return await ExecuteScalarAsync<int>(sql, new Dictionary<string, object>
            {
                ["@ThangNam"] = thangNam,
                ["@Province"] = province ?? (object)DBNull.Value,
                ["@Ward"] = wards ?? (object)DBNull.Value
            });
        }

        private async Task<List<QLCLChiTietKiemTraHauKiemATTP>> GetChiTietKiemTraHauKiemATTPData(DateTime? fromDate, DateTime? toDate, int? province, string wards, string stringSearch, int offset, int limit)
        {
            var sql = @"
                SELECT * FROM QLCLFunctionChiTietKiemTraHauKiemATTP(@FromDate, @ToDate, @Province, @Ward, @StringSearch)
                ORDER BY id DESC
                OFFSET @Offset ROWS
                FETCH NEXT @Limit ROWS ONLY";

            return await ExecuteQueryAsync<QLCLChiTietKiemTraHauKiemATTP>(sql, new Dictionary<string, object>
            {
                ["@FromDate"] = fromDate ?? (object)DBNull.Value,
                ["@ToDate"] = toDate ?? (object)DBNull.Value,
                ["@Province"] = province ?? (object)DBNull.Value,
                ["@Ward"] = wards ?? (object)DBNull.Value,
                ["@StringSearch"] = stringSearch ?? (object)DBNull.Value,
                ["@Offset"] = offset,
                ["@Limit"] = limit
            }, reader => new QLCLChiTietKiemTraHauKiemATTP
            {
                id = reader.GetInt32(0),
                san_pham = reader.GetString(1),
                so_luong_mau = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                chi_tieu = reader.GetString(3),
                so_mau_khong_dat = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                chi_tieu_vi_pham = reader.IsDBNull(5) ? "" : reader.GetString(5),
                muc_phat_hien = reader.IsDBNull(6) ? "" : reader.GetString(6)
            });
        }

        private async Task<int> GetChiTietKiemTraHauKiemATTPCount(DateTime? fromDate, DateTime? toDate, int? province, string wards, string stringSearch)
        {
            var sql = "SELECT COUNT(*) FROM QLCLFunctionChiTietKiemTraHauKiemATTP(@FromDate, @ToDate, @Province, @Ward, @StringSearch)";
            return await ExecuteScalarAsync<int>(sql, new Dictionary<string, object>
            {
                ["@FromDate"] = fromDate ?? (object)DBNull.Value,
                ["@ToDate"] = toDate ?? (object)DBNull.Value,
                ["@Province"] = province ?? (object)DBNull.Value,
                ["@Ward"] = wards ?? (object)DBNull.Value,
                ["@StringSearch"] = stringSearch ?? (object)DBNull.Value
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

        private DirectusMeta CreateMetaData(int totalCount, string thangNam, int? province, string wards)
        {
            return new DirectusMeta
            {
                total_count = totalCount,
                sort = new List<string> { "code" },
                filter = new
                {
                    thangNam = thangNam,
                    province = province,
                    wards = wards
                }
            };
        }

        private DirectusMeta CreateMetaDataWithPagination(int totalCount, int filterCount, int offset, int limit, DateTime? fromDate, DateTime? toDate, int? province, string wards, string stringSearch = null)
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
                    stringSearch = stringSearch
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