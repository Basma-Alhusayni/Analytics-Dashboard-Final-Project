namespace analytics_dashboard.dtos.analytics
{
    public class AnalyticsSummaryDto
    {
        public int TotalViews { get; set; }
        public double AverageTimeOnPage { get; set; }
        public double BounceRate { get; set; }
    }
}