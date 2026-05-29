namespace E_LaptopShop.Application.DTOs
{
    public class ChartDataPoint
    {
        public string Label { get; set; } = null!;
        public decimal Value { get; set; }
        public int Count { get; set; }
        /// <summary>ISO date — FE dùng để tooltip chính xác.</summary>
        public string? DateKey { get; set; }
    }

    public class ChartSeriesDto
    {
        public string Name { get; set; } = null!;
        public List<ChartDataPoint> Data { get; set; } = new();
    }

    public class MultiSeriesChartDto
    {
        public List<string> Labels { get; set; } = new();
        public List<ChartSeriesDto> Series { get; set; } = new();
        /// <summary>Metadata để FE hiển thị comparison badge.</summary>
        public ComparisonMeta? Comparison { get; set; }
    }

    public class ComparisonMeta
    {
        public decimal Current { get; set; }
        public decimal Previous { get; set; }
        public double GrowthPercent =>
            Previous == 0 ? 0
            : Math.Round((double)(Current - Previous) / (double)Previous * 100, 1);
    }

    /// <summary>Query params cho tất cả chart endpoints.</summary>
    public class ChartQuery
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        /// <summary>day | week | month | auto (default)</summary>
        public string GroupBy { get; set; } = "auto";
        public int Limit { get; set; } = 10;

        // Resolve defaults
        public DateTime EffectiveFrom => From ?? DateTime.UtcNow.AddDays(-30).Date;
        public DateTime EffectiveTo   => To?.AddDays(1).Date ?? DateTime.UtcNow.Date.AddDays(1);

        /// <summary>Auto-select granularity based on date range.</summary>
        public string EffectiveGroupBy
        {
            get
            {
                if (GroupBy != "auto") return GroupBy;
                var days = (EffectiveTo - EffectiveFrom).TotalDays;
                return days switch
                {
                    <= 31  => "day",
                    <= 90  => "week",
                    <= 366 => "month",
                    _      => "month",
                };
            }
        }
    }
}
