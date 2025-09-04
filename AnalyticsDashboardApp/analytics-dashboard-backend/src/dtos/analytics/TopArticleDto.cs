namespace analytics_dashboard.dtos.analytics
{
    public class TopArticleDto
    {
        public int ArticleId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Views { get; set; }
        public double AverageTimeOnPage { get; set; }
        public double BounceRate { get; set; }
    }
}