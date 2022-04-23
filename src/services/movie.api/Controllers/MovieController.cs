using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie.api.Infrastructure;
using movie.api.Models;
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

        [HttpGet]
        [ProducesResponseType(typeof(ResultSet<Movie>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> ListWithPageAsync(
            [FromQuery] string? code,
            [FromQuery] string? name,
            [FromQuery] int? page = 1,
            [FromQuery] int? rows = 10)
        {
            try
            {
                var movies = _movieContext.Movies.AsQueryable();
                var resultSet = new MovieResult(movies, (int)rows);
                if (!string.IsNullOrEmpty(code))
                {
                    resultSet.ApplyCodeFilter(code);
                }
                if (!string.IsNullOrEmpty(name))
                {
                    resultSet.ApplyNameFilter(name);
                }
                return Ok(await resultSet.GetItemsByPageAsync((int)page));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{AppName}] ERROR get movie list with page: {Exception}", Program.AppName, ex);
                return BadRequest();
            }
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(List<Movie>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<Movie>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> ListAsync(
            [FromQuery] string? code,
            [FromQuery] string? name)
        {
            try
            {
                var movies = _movieContext.Movies.AsQueryable();
                var resultSet = new MovieResult(movies);
                if (!string.IsNullOrEmpty(code))
                {
                    resultSet.ApplyCodeFilter(code);
                }
                if (!string.IsNullOrEmpty(name))
                {
                    resultSet.ApplyNameFilter(name);
                }
                return Ok(await resultSet.GetAllItemsAsync());
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
