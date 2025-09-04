using analytics_dashboard.dtos.articledetail;

namespace analytics_dashboard.dtos.article
{
    public class ArticleUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public ArticleDetailUpdateDto? ArticleDetail { get; set; }
    }
}