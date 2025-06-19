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
            [FromQuery] int? ward = null,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 10)
        {
            var response = new DirectusResponse<QLCLBaoCaoThamDinhCapGCN>();

            try
            {
                // Use raw SQL with function for data
                var sql = @"
                    SELECT * FROM QLCLFunctionBaoCaoThamDinhCapGCN(
                        @FromDate, @ToDate, @Province, @Ward
                    )
                    ORDER BY thang DESC
                    OFFSET @Offset ROWS
                    FETCH NEXT @Limit ROWS ONLY";

                var items = await _context.QLCLBaoCaoThamDinhCapGCN
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
                    SELECT COUNT(*) as TotalCount FROM QLCLFunctionBaoCaoThamDinhCapGCN(
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
                    sort = new List<string> { "-thang" },
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
                _logger.LogError(ex, "Error getting QLCL Bao Cao Tham Dinh Cap GCN data with filter");
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
        /// Get detailed information about establishments that were not granted certificates
        /// </summary>
        /// <param name="fromDate">Start date (optional)</param>
        /// <param name="toDate">End date (optional)</param>
        /// <param name="province">Province ID (optional)</param>
        /// <param name="ward">Ward ID (optional)</param>
        /// <param name="stringSearch">Search string (optional)</param>
        /// <param name="offset">Pagination offset</param>
        /// <param name="limit">Pagination limit</param>
        /// <returns>Detailed establishment information</returns>
        [HttpGet("coSoKhongCapGCN")]
        public async Task<DirectusResponse<FunctionCoSoKhongDuocCapGCN>> GetCoSoKhongCapGCN(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int? province = null,
            [FromQuery] int? ward = null,
            [FromQuery] string thangNam = null,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 10)
        {
            var response = new DirectusResponse<FunctionCoSoKhongDuocCapGCN>();

            try
            {
                // Use raw SQL with function for detailed data
                var sql = @"
                    SELECT * FROM QLCLFunctionBaoCaoCoSoKhongCapGCN(
                        @FromDate, @ToDate, @Province, @Ward, @ThangNam
                    )
                    ORDER BY id DESC
                    OFFSET @Offset ROWS
                    FETCH NEXT @Limit ROWS ONLY";

                var items = await _context.QLCLCoSoKhongDuocCapGCN
                    .FromSqlRaw(sql,
                        new Microsoft.Data.SqlClient.SqlParameter("@FromDate", fromDate ?? (object)DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@ToDate", toDate ?? (object)DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@Province", province ?? (object)DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@Ward", ward ?? (object)DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@ThangNam", thangNam ?? (object)DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@Offset", offset),
                        new Microsoft.Data.SqlClient.SqlParameter("@Limit", limit))
                    .ToListAsync();

                // Get total count using a separate query
                var countSql = @"
                    SELECT COUNT(*) as TotalCount FROM QLCLFunctionBaoCaoCoSoKhongCapGCN(
                        @FromDate, @ToDate, @Province, @Ward, @ThangNam
                    )";

                var totalCount = 0;
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = countSql;
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@FromDate", fromDate ?? (object)DBNull.Value));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@ToDate", toDate ?? (object)DBNull.Value));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Province", province ?? (object)DBNull.Value));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Ward", ward ?? (object)DBNull.Value));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@ThangNam", thangNam ?? (object)DBNull.Value));

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
                    sort = new List<string> { "-id" },
                    filter = new
                    {
                        fromDate = fromDate?.ToString("yyyy-MM-dd"),
                        toDate = toDate?.ToString("yyyy-MM-dd"),
                        province = province,
                        ward = ward,
                        thangNam = thangNam
                    }
                };
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting QLCL Co So Khong Cap GCN data");
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