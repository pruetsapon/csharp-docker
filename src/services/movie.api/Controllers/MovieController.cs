using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace movie.api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        [HttpGet("list")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public ActionResult List()
        {
            return Ok(1);
        }
    }
}
