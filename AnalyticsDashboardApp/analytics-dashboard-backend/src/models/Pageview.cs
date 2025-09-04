namespace analytics_dashboard.models
{
    public class Pageview
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public DateTime ViewedAt { get; set; }
        public int DurationSeconds { get; set; }
        public bool IsBounce { get; set; }
        
        public Article? Article { get; set; }
    }
}