import React, { useState, useEffect } from "react";
import { analyticsService, articleService } from "../services/api";
import AddArticleForm from "./AddArticleForm";
import EditArticleForm from "./EditArticleForm";
import DailyViewsChart from "./DailyViewsChart";
import type {
  AnalyticsFilterDto,
  AnalyticsSummaryDto,
  DailyViewsDto,
  TopArticleDto,
  PageviewDto,
  ArticleDto,
} from "../types";
import "./Dashboard.css";

interface ApiError extends Error {
  response?: {
    data?: {
      error?: string;
    };
  };
}

const Dashboard: React.FC = () => {
  const [analyticsSummary, setAnalyticsSummary] =
    useState<AnalyticsSummaryDto | null>(null);
  const [dailyViews, setDailyViews] = useState<DailyViewsDto[]>([]);
  const [topArticles, setTopArticles] = useState<TopArticleDto[]>([]);
  const [recentPageviews, setRecentPageviews] = useState<PageviewDto[]>([]);
  const [articles, setArticles] = useState<ArticleDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState("dashboard");
  const [showAddForm, setShowAddForm] = useState(false);
  const [editingArticle, setEditingArticle] = useState<ArticleDto | null>(null);

  const currentYear = new Date().getFullYear();
  const defaultStartDate = new Date(Date.UTC(currentYear, 0, 1));
  defaultStartDate.setUTCHours(0, 0, 0, 0);

  const defaultEndDate = new Date();
  defaultEndDate.setUTCHours(23, 59, 59, 999);

  const [filter, setFilter] = useState<AnalyticsFilterDto>({
    startDate: defaultStartDate.toISOString().split("T")[0],
    endDate: defaultEndDate.toISOString().split("T")[0],
    category: "",
  });

  const loadData = React.useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      console.log(`Loading data for tab: ${activeTab} with filter:`, filter);

      if (activeTab === "dashboard") {
        const [
          analyticsResponse,
          dailyResponse,
          topArticlesResponse,
          recentResponse,
        ] = await Promise.all([
          analyticsService.getSummary(filter),
          analyticsService.getDailyViews(filter),
          analyticsService.getTopArticles(filter),
          analyticsService.getRecentPageviews(filter),
        ]);

        setAnalyticsSummary(analyticsResponse.data);
        setDailyViews(dailyResponse.data);
        setTopArticles(topArticlesResponse.data);
        setRecentPageviews(recentResponse.data);
      } else if (activeTab === "articles") {
        console.log("Loading articles with filter:", filter);
        const articlesResponse = await articleService.getWithFilters(filter);
        console.log("Articles received:", articlesResponse.data);
        setArticles(articlesResponse.data);
      } else if (activeTab === "pageviews") {
        const pageviewsResponse = await analyticsService.getFilteredPageviews(
          filter
        );
        setRecentPageviews(pageviewsResponse.data);
      }
    } catch (error) {
      console.error("Error loading data:", error);
      const apiError = error as ApiError;
      setError(
        apiError.response?.data?.error ||
          apiError.message ||
          "Failed to load data"
      );
    } finally {
      setLoading(false);
    }
  }, [filter, activeTab]);

  useEffect(() => {
    loadData();
  }, [loadData, activeTab]);

  const handleFilterChange = (
    field: keyof AnalyticsFilterDto,
    value: string
  ) => {
    setFilter((prev) => {
      const newFilter = { ...prev, [field]: value };
      console.log("Filter changed:", newFilter);
      return newFilter;
    });
  };

  const handleTabChange = (tab: string) => {
    setActiveTab(tab);
  };

  const handleArticleAdded = () => {
    setShowAddForm(false);
    loadData();
  };

  const handleArticleUpdated = () => {
    setEditingArticle(null);
    loadData();
  };

  const handleEditArticle = (article: ArticleDto) => {
    setEditingArticle(article);
  };

  const handleDeleteArticle = async (articleId: number) => {
    if (!window.confirm("Are you sure you want to delete this article?")) {
      return;
    }

    try {
      await articleService.delete(articleId);
      loadData();
    } catch (error) {
      console.error("Error deleting article:", error);
      const apiError = error as ApiError;
      alert(
        apiError.response?.data?.error ||
          apiError.message ||
          "Failed to delete article"
      );
    }
  };

  const formatDate = (dateString: string) => {
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString("en-GB", {
        timeZone: "UTC",
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
      });
    } catch (error) {
      console.error("Error formatting date:", error, dateString);
      return dateString;
    }
  };

  const formatDateTime = (dateString: string) => {
    try {
      const date = new Date(dateString);
      return date.toLocaleString("en-GB", {
        timeZone: "UTC",
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
      });
    } catch (error) {
      console.error("Error formatting date:", error, dateString);
      return dateString;
    }
  };

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  return (
    <div className="dashboard">
      <header className="dashboard-header">
        <h1>Analytics Dashboard</h1>
        <nav className="tabs">
          <button
            className={activeTab === "dashboard" ? "active" : ""}
            onClick={() => handleTabChange("dashboard")}
          >
            Dashboard
          </button>
          <button
            className={activeTab === "articles" ? "active" : ""}
            onClick={() => handleTabChange("articles")}
          >
            Articles
          </button>
          <button
            className={activeTab === "pageviews" ? "active" : ""}
            onClick={() => handleTabChange("pageviews")}
          >
            Pageviews
          </button>
        </nav>
      </header>

      {error && (
        <div className="error-message">
          {error}
          <button className="error-close-btn" onClick={() => setError(null)}>
            ×
          </button>
        </div>
      )}

      <div className="filters">
        <div className="filter-group">
          <label>Start Date:</label>
          <input
            type="date"
            value={filter.startDate}
            onChange={(e) => handleFilterChange("startDate", e.target.value)}
          />
        </div>
        <div className="filter-group">
          <label>End Date:</label>
          <input
            type="date"
            value={filter.endDate}
            onChange={(e) => handleFilterChange("endDate", e.target.value)}
          />
        </div>
        <div className="filter-group">
          <label>Category:</label>
          <select
            value={filter.category}
            onChange={(e) => handleFilterChange("category", e.target.value)}
          >
            <option value="">All Categories</option>
            <option value="Technology">Technology</option>
            <option value="Business">Business</option>
            <option value="Lifestyle">Lifestyle</option>
          </select>
        </div>
      </div>

      {activeTab === "dashboard" && (
        <>
          {analyticsSummary && (
            <div className="analytics-summary">
              <div className="stat-card">
                <h3>Total Views</h3>
                <p className="stat">
                  {analyticsSummary.totalViews.toLocaleString()}
                </p>
              </div>
              <div className="stat-card">
                <h3>Avg. Time on Page</h3>
                <p className="stat">
                  {Math.round(analyticsSummary.averageTimeOnPage)}s
                </p>
              </div>
              <div className="stat-card">
                <h3>Bounce Rate</h3>
                <p className="stat">
                  {analyticsSummary.bounceRate.toFixed(1)}%
                </p>
              </div>
            </div>
          )}

          <div className="charts-section">
            <div className="chart-container">
              <h3>Daily Views</h3>
              <DailyViewsChart data={dailyViews} />
            </div>

            <div className="chart-container">
              <h3>Top Articles</h3>
              <div className="top-articles-list">
                {topArticles.slice(0, 5).map((article, index) => (
                  <div key={article.articleId} className="top-article-item">
                    <span className="rank">{index + 1}</span>
                    <div className="article-info">
                      <h4>{article.title}</h4>
                      <p>
                        {article.views.toLocaleString()} views ·{" "}
                        {Math.round(article.averageTimeOnPage)}s avg ·{" "}
                        {article.bounceRate.toFixed(1)}% bounce
                      </p>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </>
      )}

      {activeTab === "articles" && (
        <div className="articles-section">
          <div className="articles-header">
            <h2>Articles ({articles.length})</h2>
            <button className="btn-add" onClick={() => setShowAddForm(true)}>
              + Add Article
            </button>
          </div>
          <div className="articles-grid">
            {articles.map((article) => (
              <div key={article.id} className="article-card">
                <div className="article-image-container">
                  {article.articleDetail?.heroImageUrl ? (
                    <img
                      src={article.articleDetail.heroImageUrl}
                      alt={article.title}
                      className="article-image"
                      onError={(e) => {
                        const target = e.target as HTMLImageElement;
                        target.style.display = "none";
                        const placeholder = target.nextSibling as HTMLElement;
                        if (
                          placeholder &&
                          placeholder.classList.contains(
                            "article-image-placeholder"
                          )
                        ) {
                          placeholder.style.display = "flex";
                        }
                      }}
                    />
                  ) : null}
                  <div
                    className="article-image-placeholder"
                    style={{
                      display: !article.articleDetail?.heroImageUrl
                        ? "flex"
                        : "none",
                    }}
                  >
                    <span>No Image</span>
                  </div>
                </div>

                <div className="article-content">
                  <h3>{article.title}</h3>
                  <span className="category">{article.category}</span>

                  <div className="article-meta">
                    <p className="date">
                      Published: {formatDate(article.publishedAt)}
                    </p>
                    {article.articleDetail && (
                      <p className="reading-time">
                        ⏱️ {article.articleDetail.readingTimeSeconds} sec
                      </p>
                    )}
                  </div>

                  {article.articleDetail && (
                    <p className="summary">{article.articleDetail.summary}</p>
                  )}

                  <div className="article-actions">
                    <button
                      className="btn-edit"
                      onClick={() => handleEditArticle(article)}
                    >
                      Edit
                    </button>
                    <button
                      className="btn-delete"
                      onClick={() => handleDeleteArticle(article.id)}
                    >
                      Delete
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {activeTab === "pageviews" && (
        <div className="pageviews-section">
          <h2>Recent Pageviews ({recentPageviews.length})</h2>
          <div className="table-container">
            <table className="pageviews-table">
              <thead>
                <tr>
                  <th>Article</th>
                  <th>Viewed At</th>
                  <th>Duration</th>
                  <th>Bounce</th>
                </tr>
              </thead>
              <tbody>
                {recentPageviews.map((pageview) => (
                  <tr key={pageview.id}>
                    <td>{pageview.articleTitle}</td>
                    <td>{formatDateTime(pageview.viewedAt)}</td>
                    <td>{pageview.durationSeconds}s</td>
                    <td>{pageview.isBounce ? "Yes" : "No"}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {showAddForm && (
        <AddArticleForm
          onArticleAdded={handleArticleAdded}
          onCancel={() => setShowAddForm(false)}
        />
      )}

      {editingArticle && (
        <EditArticleForm
          article={editingArticle}
          onArticleUpdated={handleArticleUpdated}
          onCancel={() => setEditingArticle(null)}
        />
      )}
    </div>
  );
};

export default Dashboard;
