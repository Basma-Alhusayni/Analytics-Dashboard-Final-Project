namespace analytics_dashboard.models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        
        public ArticleDetail? ArticleDetail { get; set; }
        public List<Pageview> Pageviews { get; set; } = new List<Pageview>();
    }
}