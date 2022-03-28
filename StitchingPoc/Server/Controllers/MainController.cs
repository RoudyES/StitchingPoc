using Microsoft.AspNetCore.Mvc;
using StitchingPoc.Server.Services;
using StitchingPoc.Shared;

namespace StitchingPoc.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MainController : ControllerBase
    {
        MainService _service;
        public MainController(MainService service)
        {
            _service = service;
        }

        [HttpGet("SetURL")]
        public IActionResult SetURL(string url)
        {
            _service.SetURL(url);
            return Ok(true);
        }
    }
}