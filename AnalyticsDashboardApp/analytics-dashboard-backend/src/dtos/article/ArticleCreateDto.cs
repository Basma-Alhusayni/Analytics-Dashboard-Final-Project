using analytics_dashboard.dtos.articledetail;

namespace analytics_dashboard.dtos.article
{
    public class ArticleCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public ArticleDetailCreateDto? ArticleDetail { get; set; }
    }
}