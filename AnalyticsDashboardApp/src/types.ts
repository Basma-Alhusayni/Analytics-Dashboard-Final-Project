export interface AnalyticsFilterDto {
  startDate?: string;
  endDate?: string;
  category?: string;
}

export interface AnalyticsSummaryDto {
  totalViews: number;
  averageTimeOnPage: number;
  bounceRate: number;
}

export interface DailyViewsDto {
  date: string;
  views: number;
}

export interface TopArticleDto {
  articleId: number;
  title: string;
  views: number;
  averageTimeOnPage: number;
  bounceRate: number;
}

export interface PageviewDto {
  id: number;
  articleId: number;
  viewedAt: string;
  durationSeconds: number;
  isBounce: boolean;
  articleTitle: string;
}

export interface PageviewCreateDto {
  articleId: number;
  viewedAt: string;
  durationSeconds: number;
  isBounce: boolean;
}

export interface ArticleDto {
  id: number;
  title: string;
  category: string;
  publishedAt: string;
  articleDetail?: ArticleDetailDto;
}

export interface ArticleCreateDto {
  title: string;
  category: string;
  publishedAt: string;
  articleDetail?: ArticleDetailCreateDto;
}

export interface ArticleUpdateDto {
  title: string;
  category: string;
  publishedAt: string;
  articleDetail?: ArticleDetailUpdateDto;
}

export interface ArticleDetailDto {
  articleId: number;
  summary: string;
  heroImageUrl: string;
  readingTimeSeconds: number;
}

export interface ArticleDetailCreateDto {
  summary: string;
  heroImageUrl: string;
  readingTimeSeconds: number;
}

export interface ArticleDetailUpdateDto {
  summary: string;
  heroImageUrl: string;
  readingTimeSeconds: number;
}
