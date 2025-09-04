using analytics_dashboard.models;
using analytics_dashboard.dtos.analytics;

namespace analytics_dashboard.interfaces
{
    public interface IArticleService
    {
        Task<List<Article>> GetAllArticles();
        Task<Article?> GetArticleById(int id);
        Task<Article> AddArticle(Article newArticle);
        Task<Article?> UpdateArticle(int id, Article updatedArticle);
        Task<bool> DeleteArticle(int id);
        Task<List<Article>> SearchArticles(string searchTerm, string? category);
        
        Task<List<Article>> GetFilteredArticles(AnalyticsFilterDto filter);
    }
}