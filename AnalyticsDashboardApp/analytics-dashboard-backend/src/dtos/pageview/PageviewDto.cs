namespace analytics_dashboard.dtos.pageview
{
    public class PageviewDto
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public DateTime ViewedAt { get; set; }
        public int DurationSeconds { get; set; }
        public bool IsBounce { get; set; }
        public string ArticleTitle { get; set; } = string.Empty;
    }
}