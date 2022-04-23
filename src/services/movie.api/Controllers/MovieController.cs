using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie.api.Infrastructure;
using movie.data.objects;
using System.Net;

namespace movie.api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly MovieContext _movieContext;
        private readonly ILogger<MovieController> _logger;

        public MovieController(
            MovieContext movieContext,
            ILogger<MovieController> logger)
        {
            _movieContext = movieContext;
            _logger = logger;
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(List<Movie>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<Movie>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> ListAsync()
        {
            try
            {
                var movies = _movieContext.Movies.AsQueryable();
                return Ok(await movies.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{AppName}] ERROR get movie list: {Exception}", Program.AppName, ex);
                return BadRequest();
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(MovieResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> CreateMovieAsync([FromBody] MovieRequest request)
        {
            try
            {
                var movie = new Movie() {
                    Code = request.Code,
                    Name = request.Name,
                    Description = request.Description,
                    Created = DateTime.Now
                };

                _movieContext.Movies.Add(movie);
                await _movieContext.SaveChangesAsync();

                return Ok(new MovieResponse() {
                    IsSuccess = true,
                    Message = "Create movie success.",
                    ResponseCode = "200"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{AppName}] ERROR create movie: {Exception}", Program.AppName, ex);
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MovieResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] MovieRequest request)
        {
            try
            {
                var movie = _movieContext.Movies.SingleOrDefault(r => r.Id == id);
                if (movie == null)
                {
                    return NotFound(new { Message = $"Movie with id {id} not found." });
                }

                movie.Code = request.Code;
                movie.Name = request.Name;
                movie.Description = request.Description;
                movie.Updated = DateTime.Now;

                await _movieContext.SaveChangesAsync();

                return Ok(new MovieResponse()
                {
                    IsSuccess = true,
                    Message = $"Update movie with id {id} success.",
                    ResponseCode = "200"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{AppName}] ERROR update movie: {Exception} with {MovieId}", Program.AppName, ex, id);
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(MovieResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            try
            {
                var movie = _movieContext.Movies.SingleOrDefault(r => r.Id == id);
                if (movie == null)
                {
                    return NotFound(new { Message = $"Movie with id {id} not found." });
                }

                _movieContext.Movies.Remove(movie);
                await _movieContext.SaveChangesAsync();

                return Ok(new MovieResponse()
                {
                    IsSuccess = true,
                    Message = $"Delete movie with id {id} success.",
                    ResponseCode = "200"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{AppName}] ERROR delete movie: {Exception} with {MovieId}", Program.AppName, ex, id);
                return BadRequest();
            }
        }
    }
}
