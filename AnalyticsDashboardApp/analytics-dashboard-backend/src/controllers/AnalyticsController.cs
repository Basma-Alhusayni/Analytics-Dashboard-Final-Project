using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using analytics_dashboard.interfaces;
using analytics_dashboard.dtos.analytics;
using analytics_dashboard.dtos.pageview;

namespace analytics_dashboard.controllers
{
    [ApiController]
    [Route("api/v1/analytics")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IPageviewService _pageviewService;
        private readonly IMapper _mapper;

        public AnalyticsController(IPageviewService pageviewService, IMapper mapper)
        {
            _pageviewService = pageviewService;
            _mapper = mapper;
        }

        [HttpPost("summary")]
        public async Task<IActionResult> GetAnalyticsSummary([FromBody] AnalyticsFilterDto filter)
        {
            try
            {
                var summary = await _pageviewService.GetAnalyticsSummary(filter);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("daily-views")]
        public async Task<IActionResult> GetDailyViews([FromBody] AnalyticsFilterDto filter)
        {
            try
            {
                var dailyViews = await _pageviewService.GetDailyViews(filter);
                return Ok(dailyViews);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("top-articles")]
        public async Task<IActionResult> GetTopArticles([FromBody] AnalyticsFilterDto filter)
        {
            try
            {
                var topArticles = await _pageviewService.GetTopArticles(filter);
                return Ok(topArticles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("recent-pageviews")]
        public async Task<IActionResult> GetRecentPageviews([FromBody] AnalyticsFilterDto filter)
        {
            try
            {
                var pageviews = await _pageviewService.GetRecentPageviews(filter);
                var result = _mapper.Map<List<PageviewDto>>(pageviews);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("filtered-pageviews")]
        public async Task<IActionResult> GetFilteredPageviews([FromBody] AnalyticsFilterDto filter)
        {
            try
            {
                var pageviews = await _pageviewService.GetFilteredPageviews(filter);
                var result = _mapper.Map<List<PageviewDto>>(pageviews);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("pageviews")]
        public async Task<IActionResult> AddPageview([FromBody] PageviewCreateDto newPageviewDto)
        {
            try
            {
                var pageview = _mapper.Map<models.Pageview>(newPageviewDto);
                var createdPageview = await _pageviewService.AddPageview(pageview);
                return Ok(_mapper.Map<PageviewDto>(createdPageview));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("pageviews/{id}")]
        public async Task<IActionResult> DeletePageview(int id)
        {
            try
            {
                var deleted = await _pageviewService.DeletePageview(id);
                if (!deleted) return NotFound(new { error = $"Pageview with ID {id} not found" });
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}