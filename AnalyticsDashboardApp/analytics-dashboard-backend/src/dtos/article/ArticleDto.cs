using analytics_dashboard.dtos.articledetail;

namespace analytics_dashboard.dtos.article
{
    public class ArticleDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public ArticleDetailDto? ArticleDetail { get; set; }
    }
}