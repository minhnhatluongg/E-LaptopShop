namespace E_LaptopShop.Application.DTOs
{
    // ─── Summary cards ────────────────────────────────────────────────────────
    public class DashboardSummaryDto
    {
        public int TotalProducts { get; set; }
        public int TotalBrands { get; set; }
        public int TotalCategories { get; set; }
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public int OrdersThisMonth { get; set; }
        public int NewUsersThisMonth { get; set; }

        // MoM growth (%) — positive = tăng, negative = giảm
        public double RevenueGrowthPercent { get; set; }
        public double OrderGrowthPercent { get; set; }
    }

    // ─── Monthly / quarterly breakdown ────────────────────────────────────────
    public class MonthlyStatDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthLabel => $"T{Month}/{Year}";
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
        public int NewUsers { get; set; }
        public int NewProducts { get; set; }
    }

    public class QuarterlyStatDto
    {
        public int Year { get; set; }
        public int Quarter { get; set; }
        public string QuarterLabel => $"Q{Quarter}/{Year}";
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
        public int NewUsers { get; set; }
    }

    // ─── Order status distribution (donut chart) ──────────────────────────────
    public class OrderStatusStatDto
    {
        public string Status { get; set; } = null!;
        public string StatusLabel { get; set; } = null!;
        public int Count { get; set; }
        public decimal Revenue { get; set; }
    }

    // ─── Top products ─────────────────────────────────────────────────────────
    public class TopProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? Slug { get; set; }
        public string? BrandName { get; set; }
        public string? ImageUrl { get; set; }
        public int OrderCount { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }

    // ─── Combined overview (1 call cho tất cả) ────────────────────────────────
    public class DashboardOverviewDto
    {
        public DashboardSummaryDto Summary { get; set; } = new();
        public List<MonthlyStatDto> Monthly12 { get; set; } = new();      // 12 tháng gần nhất
        public List<QuarterlyStatDto> Quarterly4 { get; set; } = new();   // 4 quý gần nhất
        public List<OrderStatusStatDto> OrderStatus { get; set; } = new();
        public List<TopProductDto> TopProducts { get; set; } = new();
    }
}
