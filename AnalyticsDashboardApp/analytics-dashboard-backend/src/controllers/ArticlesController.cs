using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using analytics_dashboard.interfaces;
using analytics_dashboard.models;
using analytics_dashboard.dtos.article;
using analytics_dashboard.dtos.analytics;

namespace analytics_dashboard.controllers
{
    [ApiController]
    [Route("api/v1/articles")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly IMapper _mapper;

        public ArticlesController(IArticleService articleService, IMapper mapper)
        {
            _articleService = articleService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllArticles()
        {
            var articles = await _articleService.GetAllArticles();
            var result = _mapper.Map<List<ArticleDto>>(articles);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticleById(int id)
        {
            var article = await _articleService.GetArticleById(id);
            if (article == null) return NotFound();
            return Ok(_mapper.Map<ArticleDto>(article));
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchArticles([FromQuery] string searchTerm, [FromQuery] string? category)
        {
            var articles = await _articleService.SearchArticles(searchTerm, category);
            var result = _mapper.Map<List<ArticleDto>>(articles);
            return Ok(result);
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> GetFilteredArticles([FromQuery] AnalyticsFilterDto filter)
        {
            Console.WriteLine($"article filter debug start:");
            Console.WriteLine($"Filter received - Category: '{filter.Category}', StartDate: {filter.StartDate}, EndDate: {filter.EndDate}");
            
            foreach (var queryParam in Request.Query)
            {
                Console.WriteLine($"Query parameter: {queryParam.Key} = {queryParam.Value}");
            }
            
            var articles = await _articleService.GetFilteredArticles(filter);
            
            Console.WriteLine($"Found {articles.Count} articles after filtering");
            foreach (var article in articles)
            {
                Console.WriteLine($"Article: '{article.Title}', Category: '{article.Category}'");
            }
            Console.WriteLine($"end debug");
            
            var result = _mapper.Map<List<ArticleDto>>(articles);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddArticle([FromBody] ArticleCreateDto newArticleDto)
        {
            var article = _mapper.Map<Article>(newArticleDto);
            var createdArticle = await _articleService.AddArticle(article);
            return CreatedAtAction(nameof(GetArticleById), new { id = createdArticle.Id }, _mapper.Map<ArticleDto>(createdArticle));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, [FromBody] ArticleUpdateDto updatedArticleDto)
        {
            var updatedArticle = _mapper.Map<Article>(updatedArticleDto);
            var result = await _articleService.UpdateArticle(id, updatedArticle);
            if (result == null) return NotFound();
            return Ok(_mapper.Map<ArticleDto>(result));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var deleted = await _articleService.DeleteArticle(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}