using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Models;
using E_LaptopShop.Controllers.Api_version;
using E_LaptopShop.Helpers;
using E_LaptopShop.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_LaptopShop.Controllers
{
    [Route("api/v1/dashboard")]
    public class DashboardController : ApiV1ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ApplicationDbContext db, ILogger<DashboardController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/v1/dashboard/overview — trả tất cả stats trong 1 call
        // ─────────────────────────────────────────────────────────────────────
        /// <summary>👑 [ADMIN] Dashboard overview: summary + monthly + quarterly + status + top products</summary>
        [HttpGet("overview")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<DashboardOverviewDto>>> GetOverview(
            [FromQuery] int? year = null,
            CancellationToken ct = default)
        {
            try
            {
                var now = DateTime.UtcNow;
                var targetYear = year ?? now.Year;

                var overview = new DashboardOverviewDto
                {
                    Summary      = await BuildSummaryAsync(now, ct),
                    Monthly12    = await BuildMonthly12Async(now, ct),
                    Quarterly4   = await BuildQuarterly4Async(now, ct),
                    OrderStatus  = await BuildOrderStatusAsync(ct),
                    TopProducts  = await BuildTopProductsAsync(10, ct),
                };

                return Ok(ApiResponse<DashboardOverviewDto>.SuccessResponse(overview));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building dashboard overview");
                return StatusCode(500, ApiResponse<DashboardOverviewDto>.ErrorResponse("Dashboard error"));
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Private helpers
        // ─────────────────────────────────────────────────────────────────────

        private async Task<DashboardSummaryDto> BuildSummaryAsync(DateTime now, CancellationToken ct)
        {
            var thisMonthStart = new DateTime(now.Year, now.Month, 1);
            var lastMonthStart = thisMonthStart.AddMonths(-1);

            // Delivered = "Delivered" | "Completed"
            var revenueQuery = _db.Orders
                .Where(o => o.Status == "Delivered" || o.Status == "Completed");

            var thisMonthOrders = await _db.Orders
                .CountAsync(o => o.CreatedAt >= thisMonthStart, ct);
            var lastMonthOrders = await _db.Orders
                .CountAsync(o => o.CreatedAt >= lastMonthStart && o.CreatedAt < thisMonthStart, ct);

            var revenueThisMonth = await revenueQuery
                .Where(o => o.CreatedAt >= thisMonthStart)
                .SumAsync(o => (decimal?)o.TotalAmount ?? 0m, ct);
            var revenueLastMonth = await revenueQuery
                .Where(o => o.CreatedAt >= lastMonthStart && o.CreatedAt < thisMonthStart)
                .SumAsync(o => (decimal?)o.TotalAmount ?? 0m, ct);

            double CalcGrowth(decimal current, decimal previous) =>
                previous == 0 ? 0 : Math.Round((double)(current - previous) / (double)previous * 100, 1);

            return new DashboardSummaryDto
            {
                TotalProducts      = await _db.Products.CountAsync(ct),
                TotalBrands        = await _db.Brands.CountAsync(ct),
                TotalCategories    = await _db.Categories.CountAsync(ct),
                TotalUsers         = await _db.Users.CountAsync(ct),
                TotalOrders        = await _db.Orders.CountAsync(ct),
                PendingOrders      = await _db.Orders.CountAsync(o => o.Status == "Pending", ct),
                DeliveredOrders    = await _db.Orders.CountAsync(
                    o => o.Status == "Delivered" || o.Status == "Completed", ct),
                CancelledOrders    = await _db.Orders.CountAsync(o => o.Status == "Cancelled", ct),
                TotalRevenue       = await revenueQuery.SumAsync(o => (decimal?)o.TotalAmount ?? 0m, ct),
                RevenueThisMonth   = revenueThisMonth,
                OrdersThisMonth    = thisMonthOrders,
                NewUsersThisMonth  = await _db.Users.CountAsync(u => u.CreatedAt >= thisMonthStart, ct),
                RevenueGrowthPercent = CalcGrowth(revenueThisMonth, revenueLastMonth),
                OrderGrowthPercent   = CalcGrowth(thisMonthOrders, lastMonthOrders),
            };
        }

        private async Task<List<MonthlyStatDto>> BuildMonthly12Async(DateTime now, CancellationToken ct)
        {
            var from = new DateTime(now.Year, now.Month, 1).AddMonths(-11);

            var orders = await _db.Orders
                .Where(o => o.CreatedAt >= from
                         && (o.Status == "Delivered" || o.Status == "Completed"))
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new
                {
                    g.Key.Year, g.Key.Month,
                    Count   = g.Count(),
                    Revenue = g.Sum(o => o.TotalAmount),
                })
                .ToListAsync(ct);

            var users = await _db.Users
                .Where(u => u.CreatedAt >= from)
                .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync(ct);

            var products = await _db.Products
                .Where(p => p.CreatedAt >= from)
                .GroupBy(p => new { p.CreatedAt!.Value.Year, p.CreatedAt.Value.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync(ct);

            // Build all 12 months
            return Enumerable.Range(0, 12)
                .Select(i =>
                {
                    var m = from.AddMonths(i);
                    var o = orders.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month);
                    var u = users.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month);
                    var pr = products.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month);
                    return new MonthlyStatDto
                    {
                        Year = m.Year, Month = m.Month,
                        OrderCount  = o?.Count    ?? 0,
                        Revenue     = o?.Revenue  ?? 0m,
                        NewUsers    = u?.Count    ?? 0,
                        NewProducts = pr?.Count   ?? 0,
                    };
                })
                .ToList();
        }

        private async Task<List<QuarterlyStatDto>> BuildQuarterly4Async(DateTime now, CancellationToken ct)
        {
            var from = new DateTime(now.Year, now.Month, 1).AddMonths(-11);

            var orders = await _db.Orders
                .Where(o => o.CreatedAt >= from
                         && (o.Status == "Delivered" || o.Status == "Completed"))
                .ToListAsync(ct);

            var users = await _db.Users
                .Where(u => u.CreatedAt >= from)
                .ToListAsync(ct);

            int CurrentQuarter(DateTime d) => (d.Month - 1) / 3 + 1;

            // Build 4 quarters
            return Enumerable.Range(0, 4)
                .Select(i =>
                {
                    var pivot = now.AddMonths(-i * 3);
                    var q     = CurrentQuarter(pivot);
                    var y     = pivot.Year;
                    var qStart = new DateTime(y, (q - 1) * 3 + 1, 1);
                    var qEnd   = qStart.AddMonths(3);

                    return new QuarterlyStatDto
                    {
                        Year     = y,
                        Quarter  = q,
                        OrderCount = orders.Count(o => o.CreatedAt >= qStart && o.CreatedAt < qEnd),
                        Revenue    = orders.Where(o => o.CreatedAt >= qStart && o.CreatedAt < qEnd)
                                          .Sum(o => o.TotalAmount),
                        NewUsers   = users.Count(u => u.CreatedAt >= qStart && u.CreatedAt < qEnd),
                    };
                })
                .OrderBy(q => q.Year).ThenBy(q => q.Quarter)
                .ToList();
        }

        private async Task<List<OrderStatusStatDto>> BuildOrderStatusAsync(CancellationToken ct)
        {
            var statusLabels = new Dictionary<string, string>
            {
                ["Pending"]    = "Chờ xác nhận",
                ["Confirmed"]  = "Đã xác nhận",
                ["Processing"] = "Đang xử lý",
                ["Shipped"]    = "Đang giao",
                ["Delivered"]  = "Đã giao",
                ["Completed"]  = "Hoàn thành",
                ["Cancelled"]  = "Đã huỷ",
                ["Returned"]   = "Hoàn trả",
                ["Refunded"]   = "Hoàn tiền",
            };

            var raw = await _db.Orders
                .GroupBy(o => o.Status)
                .Select(g => new
                {
                    Status  = g.Key,
                    Count   = g.Count(),
                    Revenue = g.Sum(o => o.TotalAmount),
                })
                .ToListAsync(ct);

            return raw
                .OrderByDescending(r => r.Count)
                .Select(r => new OrderStatusStatDto
                {
                    Status      = r.Status,
                    StatusLabel = statusLabels.TryGetValue(r.Status, out var lbl) ? lbl : r.Status,
                    Count       = r.Count,
                    Revenue     = r.Revenue,
                })
                .ToList();
        }

        private async Task<List<TopProductDto>> BuildTopProductsAsync(int limit, CancellationToken ct)
        {
            var raw = await _db.OrderItems
                .Where(i => i.Order.Status == "Delivered" || i.Order.Status == "Completed")
                .GroupBy(i => i.ProductId)
                .Select(g => new
                {
                    ProductId    = g.Key,
                    OrderCount   = g.Select(i => i.OrderId).Distinct().Count(),
                    QuantitySold = g.Sum(i => i.Quantity),
                    Revenue      = g.Sum(i => (decimal)i.SubTotal),
                })
                .OrderByDescending(r => r.Revenue)
                .Take(limit)
                .ToListAsync(ct);

            if (!raw.Any()) return new List<TopProductDto>();

            var ids = raw.Select(r => r.ProductId).ToList();

            var products = await _db.Products
                .Where(p => ids.Contains(p.Id))
                .Select(p => new
                {
                    p.Id, p.Name, p.Slug,
                    BrandName = p.Brand != null ? p.Brand.Name : null,
                    ImageUrl  = p.ProductImages
                        .Where(pi => pi.IsMain)
                        .Select(pi => pi.ImageUrl)
                        .FirstOrDefault()
                        ?? p.ProductImages.Select(pi => pi.ImageUrl).FirstOrDefault(),
                })
                .ToListAsync(ct);

            return raw.Select(r =>
            {
                var p = products.FirstOrDefault(x => x.Id == r.ProductId);
                return new TopProductDto
                {
                    ProductId    = r.ProductId,
                    ProductName  = p?.Name ?? "Unknown",
                    Slug         = p?.Slug,
                    BrandName    = p?.BrandName,
                    ImageUrl     = p?.ImageUrl,
                    OrderCount   = r.OrderCount,
                    QuantitySold = r.QuantitySold,
                    Revenue      = r.Revenue,
                };
            }).ToList();
        }

        // ─────────────────────────────────────────────────────────────────────
        // FLEXIBLE CHART ENDPOINTS
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>👑 Revenue chart — flexible date range + category + groupBy.</summary>
        [HttpGet("chart/revenue")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<MultiSeriesChartDto>>> RevenueChart(
            [FromQuery] ChartQuery q, CancellationToken ct = default)
        {
            try
            {
                var result = await BuildRevenueChartAsync(q, ct);
                return Ok(ApiResponse<MultiSeriesChartDto>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building revenue chart");
                return StatusCode(500, ApiResponse<MultiSeriesChartDto>.ErrorResponse("Chart error"));
            }
        }

        /// <summary>👑 Orders count chart — flexible date range + category + groupBy.</summary>
        [HttpGet("chart/orders")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<MultiSeriesChartDto>>> OrdersChart(
            [FromQuery] ChartQuery q, CancellationToken ct = default)
        {
            try
            {
                var result = await BuildOrdersChartAsync(q, ct);
                return Ok(ApiResponse<MultiSeriesChartDto>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building orders chart");
                return StatusCode(500, ApiResponse<MultiSeriesChartDto>.ErrorResponse("Chart error"));
            }
        }

        /// <summary>👑 New users chart — flexible date range + groupBy.</summary>
        [HttpGet("chart/users")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<MultiSeriesChartDto>>> UsersChart(
            [FromQuery] ChartQuery q, CancellationToken ct = default)
        {
            try
            {
                var result = await BuildUsersChartAsync(q, ct);
                return Ok(ApiResponse<MultiSeriesChartDto>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building users chart");
                return StatusCode(500, ApiResponse<MultiSeriesChartDto>.ErrorResponse("Chart error"));
            }
        }

        /// <summary>👑 Top products — flexible date range + category.</summary>
        [HttpGet("chart/top-products")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<List<TopProductDto>>>> TopProductsChart(
            [FromQuery] ChartQuery q, CancellationToken ct = default)
        {
            try
            {
                var result = await BuildTopProductsRangeAsync(q, ct);
                return Ok(ApiResponse<List<TopProductDto>>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building top products chart");
                return StatusCode(500, ApiResponse<List<TopProductDto>>.ErrorResponse("Chart error"));
            }
        }

        // ─── Flexible chart builders ──────────────────────────────────────────

        private async Task<MultiSeriesChartDto> BuildRevenueChartAsync(ChartQuery q, CancellationToken ct)
        {
            var from = q.EffectiveFrom;
            var to   = q.EffectiveTo;
            var groupBy = q.EffectiveGroupBy;

            // Comparison period (same length before `from`)
            var span     = (to - from).TotalDays;
            var prevFrom = from.AddDays(-span);

            var baseQuery = _db.Orders
                .Where(o => (o.Status == "Delivered" || o.Status == "Completed")
                         && o.CreatedAt >= from && o.CreatedAt < to);

            if (q.CategoryId.HasValue)
                baseQuery = baseQuery.Where(o =>
                    o.OrderItems.Any(i => i.Product != null && i.Product.CategoryId == q.CategoryId));
            if (q.BrandId.HasValue)
                baseQuery = baseQuery.Where(o =>
                    o.OrderItems.Any(i => i.Product != null && i.Product.BrandId == q.BrandId));

            var orders = await baseQuery
                .Select(o => new { o.CreatedAt, o.TotalAmount })
                .ToListAsync(ct);

            var prevQuery = _db.Orders
                .Where(o => (o.Status == "Delivered" || o.Status == "Completed")
                         && o.CreatedAt >= prevFrom && o.CreatedAt < from);
            if (q.CategoryId.HasValue)
                prevQuery = prevQuery.Where(o =>
                    o.OrderItems.Any(i => i.Product != null && i.Product.CategoryId == q.CategoryId));
            var prevRevenue = await prevQuery.SumAsync(o => (decimal?)o.TotalAmount ?? 0m, ct);

            var grouped = GroupByGranularity(orders.Select(o => (o.CreatedAt, o.TotalAmount)), from, to, groupBy);

            return new MultiSeriesChartDto
            {
                Labels = grouped.Select(g => g.Label).ToList(),
                Series = new List<ChartSeriesDto>
                {
                    new() { Name = "Doanh thu", Data = grouped.Select(g => new ChartDataPoint { Label = g.Label, Value = g.Value, DateKey = g.DateKey }).ToList() },
                },
                Comparison = new ComparisonMeta
                {
                    Current  = orders.Sum(o => o.TotalAmount),
                    Previous = prevRevenue,
                },
            };
        }

        private async Task<MultiSeriesChartDto> BuildOrdersChartAsync(ChartQuery q, CancellationToken ct)
        {
            var from = q.EffectiveFrom;
            var to   = q.EffectiveTo;
            var groupBy = q.EffectiveGroupBy;

            var baseQ = _db.Orders
                .Where(o => o.CreatedAt >= from && o.CreatedAt < to);
            if (q.CategoryId.HasValue)
                baseQ = baseQ.Where(o =>
                    o.OrderItems.Any(i => i.Product != null && i.Product.CategoryId == q.CategoryId));
            if (q.BrandId.HasValue)
                baseQ = baseQ.Where(o =>
                    o.OrderItems.Any(i => i.Product != null && i.Product.BrandId == q.BrandId));

            var orders = await baseQ.Select(o => new { o.CreatedAt, o.Status }).ToListAsync(ct);

            var all      = GroupByGranularity(orders.Select(o => (o.CreatedAt, 1m)), from, to, groupBy);
            var success  = GroupByGranularity(
                orders.Where(o => o.Status is "Delivered" or "Completed").Select(o => (o.CreatedAt, 1m)),
                from, to, groupBy);
            var cancelled = GroupByGranularity(
                orders.Where(o => o.Status == "Cancelled").Select(o => (o.CreatedAt, 1m)),
                from, to, groupBy);

            return new MultiSeriesChartDto
            {
                Labels = all.Select(g => g.Label).ToList(),
                Series = new List<ChartSeriesDto>
                {
                    new() { Name = "Tổng đơn",   Data = all.Select(g      => new ChartDataPoint { Label = g.Label,      Value = g.Value, Count = (int)g.Value }).ToList() },
                    new() { Name = "Hoàn thành", Data = success.Select(g  => new ChartDataPoint { Label = g.Label,  Value = g.Value, Count = (int)g.Value }).ToList() },
                    new() { Name = "Đã huỷ",     Data = cancelled.Select(g => new ChartDataPoint { Label = g.Label, Value = g.Value, Count = (int)g.Value }).ToList() },
                },
                Comparison = new ComparisonMeta
                {
                    Current  = orders.Count,
                    Previous = await _db.Orders.CountAsync(o =>
                        o.CreatedAt >= from.AddDays(-(to - from).TotalDays)
                        && o.CreatedAt < from, ct),
                },
            };
        }

        private async Task<MultiSeriesChartDto> BuildUsersChartAsync(ChartQuery q, CancellationToken ct)
        {
            var from = q.EffectiveFrom;
            var to   = q.EffectiveTo;
            var groupBy = q.EffectiveGroupBy;

            var users = await _db.Users
                .Where(u => u.CreatedAt >= from && u.CreatedAt < to)
                .Select(u => new { u.CreatedAt })
                .ToListAsync(ct);

            var grouped = GroupByGranularity(users.Select(u => (u.CreatedAt, 1m)), from, to, groupBy);

            return new MultiSeriesChartDto
            {
                Labels = grouped.Select(g => g.Label).ToList(),
                Series = new List<ChartSeriesDto>
                {
                    new() { Name = "User mới", Data = grouped.Select(g => new ChartDataPoint { Label = g.Label, Value = g.Value }).ToList() },
                },
                Comparison = new ComparisonMeta
                {
                    Current  = users.Count,
                    Previous = await _db.Users.CountAsync(u =>
                        u.CreatedAt >= from.AddDays(-(to - from).TotalDays) && u.CreatedAt < from, ct),
                },
            };
        }

        private async Task<List<TopProductDto>> BuildTopProductsRangeAsync(ChartQuery q, CancellationToken ct)
        {
            var from = q.EffectiveFrom;
            var to   = q.EffectiveTo;

            var query = _db.OrderItems
                .Where(i => i.Order.CreatedAt >= from && i.Order.CreatedAt < to
                         && (i.Order.Status == "Delivered" || i.Order.Status == "Completed"));

            if (q.CategoryId.HasValue)
                query = query.Where(i => i.Product != null && i.Product.CategoryId == q.CategoryId);
            if (q.BrandId.HasValue)
                query = query.Where(i => i.Product != null && i.Product.BrandId == q.BrandId);

            var raw = await query
                .GroupBy(i => i.ProductId)
                .Select(g => new
                {
                    ProductId    = g.Key,
                    OrderCount   = g.Select(i => i.OrderId).Distinct().Count(),
                    QuantitySold = g.Sum(i => i.Quantity),
                    Revenue      = g.Sum(i => (decimal)i.SubTotal),
                })
                .OrderByDescending(r => r.Revenue)
                .Take(q.Limit)
                .ToListAsync(ct);

            if (!raw.Any()) return new List<TopProductDto>();

            var ids = raw.Select(r => r.ProductId).ToList();
            var products = await _db.Products
                .Where(p => ids.Contains(p.Id))
                .Select(p => new
                {
                    p.Id, p.Name, p.Slug,
                    BrandName = p.Brand != null ? p.Brand.Name : null,
                    ImageUrl  = p.ProductImages.Where(pi => pi.IsMain)
                                              .Select(pi => pi.ImageUrl).FirstOrDefault()
                               ?? p.ProductImages.Select(pi => pi.ImageUrl).FirstOrDefault(),
                })
                .ToListAsync(ct);

            return raw.Select(r =>
            {
                var p = products.FirstOrDefault(x => x.Id == r.ProductId);
                return new TopProductDto
                {
                    ProductId = r.ProductId, ProductName = p?.Name ?? "Unknown",
                    Slug = p?.Slug, BrandName = p?.BrandName, ImageUrl = p?.ImageUrl,
                    OrderCount = r.OrderCount, QuantitySold = r.QuantitySold, Revenue = r.Revenue,
                };
            }).ToList();
        }

        // ─── Grouping helper ──────────────────────────────────────────────────

        private static List<(string Label, decimal Value, string DateKey)> GroupByGranularity(
            IEnumerable<(DateTime Date, decimal Value)> source,
            DateTime from, DateTime to, string groupBy)
        {
            var dict = source
                .GroupBy(x => groupBy switch
                {
                    "day"  => x.Date.Date.ToString("yyyy-MM-dd"),
                    "week" => IsoWeekKey(x.Date),
                    _      => $"{x.Date.Year:D4}-{x.Date.Month:D2}",
                })
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Value));

            var slots = new List<(string Label, decimal Value, string DateKey)>();
            var cursor = from.Date;

            while (cursor < to.Date)
            {
                string key = groupBy switch
                {
                    "day"  => cursor.ToString("yyyy-MM-dd"),
                    "week" => IsoWeekKey(cursor),
                    _      => $"{cursor.Year:D4}-{cursor.Month:D2}",
                };

                string label = groupBy switch
                {
                    "day"  => cursor.ToString("dd/MM"),
                    "week" => $"T{IsoWeekNumber(cursor)} {cursor.Year}",
                    _      => $"T{cursor.Month}/{cursor.Year % 100:D2}",
                };

                slots.Add((label, dict.TryGetValue(key, out var v) ? v : 0m, key));

                cursor = groupBy switch
                {
                    "day"  => cursor.AddDays(1),
                    "week" => cursor.AddDays(7),
                    _      => cursor.AddMonths(1),
                };
            }

            // Deduplicate week labels (cursor can land on same week twice near boundaries)
            return groupBy == "week"
                ? slots.GroupBy(s => s.DateKey).Select(g => g.First()).ToList()
                : slots;
        }

        private static string IsoWeekKey(DateTime d)
        {
            var week = IsoWeekNumber(d);
            return $"{d.Year}-W{week:D2}";
        }

        private static int IsoWeekNumber(DateTime d)
        {
            return System.Globalization.ISOWeek.GetWeekOfYear(d);
        }
    }
}
