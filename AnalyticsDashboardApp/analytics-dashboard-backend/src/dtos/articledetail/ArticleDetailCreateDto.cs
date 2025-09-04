namespace analytics_dashboard.dtos.articledetail
{
    public class ArticleDetailCreateDto
    {
        public string Summary { get; set; } = string.Empty;
        public string HeroImageUrl { get; set; } = string.Empty;
        public int ReadingTimeSeconds { get; set; }
    }
}