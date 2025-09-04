namespace analytics_dashboard.models
{
    public class ArticleDetail
    {
        public int ArticleId { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string HeroImageUrl { get; set; } = string.Empty;
        public int ReadingTimeSeconds { get; set; }
        
        public Article? Article { get; set; }
    }
}