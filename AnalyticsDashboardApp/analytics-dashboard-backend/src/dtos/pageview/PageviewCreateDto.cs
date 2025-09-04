namespace analytics_dashboard.dtos.pageview
{
    public class PageviewCreateDto
    {
        public int ArticleId { get; set; }
        public DateTime ViewedAt { get; set; }
        public int DurationSeconds { get; set; }
        public bool IsBounce { get; set; }
    }
}