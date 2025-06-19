using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreAPI.Models;
using CoreAPI.Models.BaseResponse;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net;

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
            [FromQuery] int? ward = null,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 10)
        {
            var response = new DirectusResponse<QLCLBaoCaoKiemTraHauKiemATTP>();

            try
            {
                // Use raw SQL with function for data
                var sql = @"
                    SELECT * FROM QLCLFunctionBaoCaoKiemTraHauKiemATTP(
                        @FromDate, @ToDate, @Province, @Ward
                    )
                    ORDER BY thang DESC, province, ward
                    OFFSET @Offset ROWS
                    FETCH NEXT @Limit ROWS ONLY";

                var items = await _context.QLCLBaoCaoKiemTraHauKiemATTP
                    .FromSqlRaw(sql,
                        new Microsoft.Data.SqlClient.SqlParameter("@FromDate", fromDate ?? (object)DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@ToDate", toDate ?? (object)DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@Province", province ?? (object)DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@Ward", ward ?? (object)DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@Offset", offset),
                        new Microsoft.Data.SqlClient.SqlParameter("@Limit", limit))
                    .ToListAsync();

                // Get total count using a separate query
                var countSql = @"
                    SELECT COUNT(*) as TotalCount FROM QLCLFunctionBaoCaoKiemTraHauKiemATTP(
                        @FromDate, @ToDate, @Province, @Ward
                    )";

                var totalCount = 0;
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = countSql;
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@FromDate", fromDate ?? (object)DBNull.Value));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@ToDate", toDate ?? (object)DBNull.Value));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Province", province ?? (object)DBNull.Value));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Ward", ward ?? (object)DBNull.Value));

                    if (command.Connection.State != System.Data.ConnectionState.Open)
                        command.Connection.Open();

                    var result = command.ExecuteScalar();
                    totalCount = result != null ? Convert.ToInt32(result) : 0;
                }

                response.Data = items;
                response.Meta = new DirectusMeta
                {
                    total_count = totalCount,
                    filter_count = items.Count,
                    offset = offset,
                    limit = limit,
                    page_count = (int)Math.Ceiling((double)totalCount / limit),
                    sort = new List<string> { "-thang", "province", "ward" },
                    filter = new
                    {
                        fromDate = fromDate?.ToString("yyyy-MM-dd"),
                        toDate = toDate?.ToString("yyyy-MM-dd"),
                        province = province,
                        ward = ward
                    }
                };
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting QLCL Bao Cao Kiem Tra Hau Kiem ATTP data with filter");
                response.Errors.Add(new ErrorResponse
                {
                    Message = "Internal server error",
                    Code = "INTERNAL_ERROR",
                    Reason = ex.Message,
                    Extensions = new ExtensionsResponse
                    {
                        code = "INTERNAL_ERROR",
                        reason = ex.Message
                    }
                });
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return response;
        }

        /// <summary>
        /// Get detailed information about establishments for a specific month, province, and ward
        /// </summary>
        /// <param name="thangNam">Month and year in format 'yyyy-MM'</param>
        /// <param name="province">Province ID (optional)</param>
        /// <param name="ward">Ward ID (optional)</param>
        /// <returns>Detailed establishment information</returns>
        [HttpGet("detail")]
        public async Task<DirectusResponse<QLCLDetailCoSoKiemTraHauKiemATTP>> GetDetail(
            [FromQuery] string thangNam,
            [FromQuery] int? province = null,
            [FromQuery] int? ward = null)
        {
            var response = new DirectusResponse<QLCLDetailCoSoKiemTraHauKiemATTP>();

            try
            {
                // Validate thangNam format
                if (string.IsNullOrEmpty(thangNam) || !System.Text.RegularExpressions.Regex.IsMatch(thangNam, @"^\d{4}-\d{2}$"))
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

                // Use raw SQL with function for detailed data
                var sql = @"
                    SELECT * FROM QLCLFunctionDetailCoSoKiemTraHauKiemATTP(
                        @ThangNam, @Province, @Ward
                    )
                    ORDER BY code DESC
                    ";

                var items = await _context.QLCLDetailCoSoKiemTraHauKiemATTP
                    .FromSqlRaw(sql,
                        new Microsoft.Data.SqlClient.SqlParameter("@ThangNam", thangNam),
                        new Microsoft.Data.SqlClient.SqlParameter("@Province", province ?? (object)DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@Ward", ward ?? (object)DBNull.Value))
                    .ToListAsync();

                // Get total count using a separate query
                var countSql = @"
                    SELECT COUNT(*) as TotalCount FROM QLCLFunctionDetailCoSoKiemTraHauKiemATTP(
                        @ThangNam, @Province, @Ward
                    )";

                var totalCount = 0;
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = countSql;
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@ThangNam", thangNam));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Province", province ?? (object)DBNull.Value));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Ward", ward ?? (object)DBNull.Value));

                    if (command.Connection.State != System.Data.ConnectionState.Open)
                        command.Connection.Open();

                    var result = command.ExecuteScalar();
                    totalCount = result != null ? Convert.ToInt32(result) : 0;
                }

                response.Data = items;
                response.Meta = new DirectusMeta
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
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting QLCL Detail Co So Kiem Tra Hau Kiem ATTP data");
                response.Errors.Add(new ErrorResponse
                {
                    Message = "Internal server error",
                    Code = "INTERNAL_ERROR",
                    Reason = ex.Message,
                    Extensions = new ExtensionsResponse
                    {
                        code = "INTERNAL_ERROR",
                        reason = ex.Message
                    }
                });
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
        /// <param name="ward">Ward ID (optional)</param>
        /// <param name="offset">Pagination offset</param>
        /// <param name="limit">Pagination limit</param>
        /// <returns>Detailed inspection information</returns>
        [HttpGet("chitiet")]
        public async Task<DirectusResponse<QLCLChiTietKiemTraHauKiemATTP>> GetChiTiet(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int? province = null,
            [FromQuery] int? ward = null,
            [FromQuery] string stringSearch  = null,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 10)
        {
            var response = new DirectusResponse<QLCLChiTietKiemTraHauKiemATTP>();

            try
            {
                // Use raw SQL with function for detailed inspection data
                var sql = @"
                    SELECT * FROM QLCLFunctionChiTietKiemTraHauKiemATTP(
                        @FromDate, @ToDate, @Province, @Ward, @StringSearch 
                    )
                    ORDER BY id DESC
                    OFFSET @Offset ROWS
                    FETCH NEXT @Limit ROWS ONLY";

                var items = await _context.QLCLChiTietKiemTraHauKiemATTP
                    .FromSqlRaw(sql,
                        new Microsoft.Data.SqlClient.SqlParameter("@FromDate", fromDate ?? (object)DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@ToDate", toDate ?? (object)DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@Province", province ?? (object)DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@Ward", ward ?? (object)DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@StringSearch", stringSearch ?? (object)DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@Offset", offset),
                        new Microsoft.Data.SqlClient.SqlParameter("@Limit", limit))
                    .ToListAsync();

                // Get total count using a separate query
                var countSql = @"
                    SELECT COUNT(*) as TotalCount FROM QLCLFunctionChiTietKiemTraHauKiemATTP(
                        @FromDate, @ToDate, @Province, @Ward, @StringSearch
                    )";

                var totalCount = 0;
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = countSql;
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@FromDate", fromDate ?? (object)DBNull.Value));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@ToDate", toDate ?? (object)DBNull.Value));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Province", province ?? (object)DBNull.Value));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Ward", ward ?? (object)DBNull.Value));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@StringSearch", stringSearch ?? (object)DBNull.Value));

                    if (command.Connection.State != System.Data.ConnectionState.Open)
                        command.Connection.Open();

                    var result = command.ExecuteScalar();
                    totalCount = result != null ? Convert.ToInt32(result) : 0;
                }

                response.Data = items;
                response.Meta = new DirectusMeta
                {
                    total_count = totalCount,
                    filter_count = items.Count(),
                    offset = offset,
                    limit = limit,
                    page_count = (int)Math.Ceiling((double)totalCount / limit),
                    sort = new List<string> { "-id"},
                    filter = new
                    {
                        fromDate = fromDate?.ToString("yyyy-MM-dd"),
                        toDate = toDate?.ToString("yyyy-MM-dd"),
                        province = province,
                        ward = ward,
                        stringSearch = stringSearch
                    }
                };
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting QLCL Chi Tiet Kiem Tra Hau Kiem ATTP data");
                response.Errors.Add(new ErrorResponse
                {
                    Message = "Internal server error",
                    Code = "INTERNAL_ERROR",
                    Reason = ex.Message,
                    Extensions = new ExtensionsResponse
                    {
                        code = "INTERNAL_ERROR",
                        reason = ex.Message
                    }
                });
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return response;
        }
    }
} 