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

        /// <summary>
        /// Get QLCL Bao Cao Kiem Tra Hau Kiem ATTP data using function with parameters
        /// </summary>
        /// <param name="fromDate">Filter from date (yyyy-MM-dd)</param>
        /// <param name="toDate">Filter to date (yyyy-MM-dd)</param>
        /// <param name="province">Filter by province ID</param>
        /// <param name="ward">Filter by ward ID</param>
        /// <param name="offset">Offset for pagination (default: 0)</param>
        /// <param name="limit">Limit for pagination (default: 10)</param>
        /// <returns>Filtered data using function</returns>
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
    }
} 