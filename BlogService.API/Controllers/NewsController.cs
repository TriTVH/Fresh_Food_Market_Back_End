using BlogService.Service;
using Microsoft.AspNetCore.Mvc;

namespace BlogService.API.Controllers
{
    [Route("api/news")]
    public class NewsController : ControllerBase
    {
        INewsService _newsService;
        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }
        [HttpGet]
        public async Task<IActionResult> GetNews()
        {
            var response = await _newsService.GetNewsAsync();
            if (response.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }
    }
}
