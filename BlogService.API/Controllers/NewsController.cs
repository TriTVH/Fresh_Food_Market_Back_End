using BlogService.Service;
using BlogService.Service.DTO.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogService.API.Controllers
{
    [ApiController]
    [Route("api/news")]
    public class NewsController : ControllerBase
    {
        INewsService _newsService;
        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }
        [HttpGet]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetNews()
        {
            var response = await _newsService.GetNewsAsync();
            if (response.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpGet("GUEST")]
        public async Task<IActionResult> GetPublishedNews()
        {
            var response = await _newsService.GetPublishedNewsAsync();
            if (response.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpGet("{newsId:int}")]
        public async Task<IActionResult> GetNewsById(int newsId)
        {
            var response = await _newsService.GetNewsByIdAsync(newsId);
            if (response.Success)
                return Ok(response);
            else if (response.StatusCode == 404)
                return NotFound(response);
            else
                return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> CreateNews([FromBody] NewsRequestDTO request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            var response = await _newsService.CreateNewsAsync(request);
            if (response.Success)
                return StatusCode(response.StatusCode, response);
            else
                return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{newsId:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> UpdateNews(int newsId, [FromBody] NewsRequestDTO request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            var response = await _newsService.UpdateNewsAsync(newsId, request);
            if (response.Success)
                return Ok(response);
            else
                return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{newsId:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> DeleteNews(int newsId)
        {
            var response = await _newsService.DeleteNewsAsync(newsId);
            if (response.Success)
                return Ok(response);
            else
                return StatusCode(response.StatusCode, response);
        }
    }
}
