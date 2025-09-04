using AutoMapper;
using analytics_dashboard.models;
using analytics_dashboard.dtos.article;
using analytics_dashboard.dtos.articledetail;
using analytics_dashboard.dtos.pageview;

namespace analytics_dashboard.config.mapper
{
    public class MappingProfiler : AutoMapper.Profile
    {
        public MappingProfiler()
        {
            CreateMap<Article, ArticleDto>();
            CreateMap<ArticleCreateDto, Article>();
            CreateMap<ArticleUpdateDto, Article>();

            CreateMap<ArticleDetail, ArticleDetailDto>();
            CreateMap<ArticleDetailCreateDto, ArticleDetail>();
            CreateMap<ArticleDetailUpdateDto, ArticleDetail>();

            CreateMap<Pageview, PageviewDto>().ForMember(dest => dest.ArticleTitle, opt => opt.MapFrom(
                src => src.Article != null ? src.Article.Title : string.Empty
                ));
            CreateMap<PageviewCreateDto, Pageview>();
        }
    }
}