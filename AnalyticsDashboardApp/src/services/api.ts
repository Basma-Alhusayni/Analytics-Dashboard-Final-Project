import axios from "axios";
import type { AxiosResponse } from "axios";
import type {
  AnalyticsFilterDto,
  AnalyticsSummaryDto,
  DailyViewsDto,
  TopArticleDto,
  PageviewDto,
  PageviewCreateDto,
  ArticleDto,
  ArticleCreateDto,
  ArticleUpdateDto,
} from "../types";

const API_BASE_URL = "http://localhost:5008/api/v1";

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

export const articleService = {
  getAll: (): Promise<AxiosResponse<ArticleDto[]>> => api.get("/articles"),
  getById: (id: number): Promise<AxiosResponse<ArticleDto>> =>
    api.get(`/articles/${id}`),
  create: (data: ArticleCreateDto): Promise<AxiosResponse<ArticleDto>> =>
    api.post("/articles", data),
  update: (
    id: number,
    data: ArticleUpdateDto
  ): Promise<AxiosResponse<ArticleDto>> => api.put(`/articles/${id}`, data),
  delete: (id: number): Promise<AxiosResponse<void>> =>
    api.delete(`/articles/${id}`),
  search: (
    searchTerm: string,
    category?: string
  ): Promise<AxiosResponse<ArticleDto[]>> =>
    api.get("/articles/search", { params: { searchTerm, category } }),
  getWithFilters: (
    filter: AnalyticsFilterDto
  ): Promise<AxiosResponse<ArticleDto[]>> =>
    api.get("/articles/filtered", { params: filter }),
};

export const analyticsService = {
  getSummary: (
    filter: AnalyticsFilterDto
  ): Promise<AxiosResponse<AnalyticsSummaryDto>> =>
    api.post("/analytics/summary", filter),
  getDailyViews: (
    filter: AnalyticsFilterDto
  ): Promise<AxiosResponse<DailyViewsDto[]>> =>
    api.post("/analytics/daily-views", filter),
  getTopArticles: (
    filter: AnalyticsFilterDto
  ): Promise<AxiosResponse<TopArticleDto[]>> =>
    api.post("/analytics/top-articles", filter),
  getRecentPageviews: (
    filter: AnalyticsFilterDto
  ): Promise<AxiosResponse<PageviewDto[]>> =>
    api.post("/analytics/recent-pageviews", filter),
  getFilteredPageviews: (
    filter: AnalyticsFilterDto
  ): Promise<AxiosResponse<PageviewDto[]>> =>
    api.post("/analytics/filtered-pageviews", filter),
  addPageview: (data: PageviewCreateDto): Promise<AxiosResponse<PageviewDto>> =>
    api.post("/analytics/pageviews", data),
};

api.interceptors.response.use(
  (response) => response,
  (error) => {
    const errorMessage =
      error.response?.data?.error ||
      error.response?.data?.message ||
      error.response?.data ||
      error.message ||
      "An unexpected error occurred";

    console.error("API Error:", errorMessage);

    if (error.response?.status !== 401) {
      alert(`Error: ${errorMessage}`);
    }

    return Promise.reject(error);
  }
);

api.interceptors.request.use(
  (config) => {
    console.log(
      `API Request: ${config.method?.toUpperCase()} ${config.url}`,
      config.data || config.params
    );
    return config;
  },
  (error) => {
    console.error("API Request Error:", error);
    return Promise.reject(error);
  }
);
