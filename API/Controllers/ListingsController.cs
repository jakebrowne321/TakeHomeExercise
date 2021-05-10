using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListingsController : ControllerBase
    {
        private readonly ILogger<ListingsController> _logger;

        public ListingsController(ILogger<ListingsController> logger)
        {
            _logger = logger;
        }

        public ActionResult GetListings()
        {
            return Ok("TODO");
        }
    }
}
