using Microsoft.EntityFrameworkCore;
using movie.data.objects;
using System.Text.RegularExpressions;

namespace movie.api.Models
{
    public class MovieResult
    {
        private int _rows = 0;
        private IQueryable<Movie> _movies;
        public MovieResult(IQueryable<Movie> movies, int rows)
        {
            _movies = movies;
            _rows = rows;
        }

        public MovieResult(IQueryable<Movie> movies)
        {
            _movies = movies;
        }

        public void ApplyCodeFilter(string code)
        {
            _movies = _movies.Where(c => Regex.IsMatch(c.Code, code));
        }

        public void ApplyNameFilter(string name)
        {
            _movies = _movies.Where(c => Regex.IsMatch(c.Name, name));
        }

        public async Task<ResultSet<Movie>> GetItemsByPageAsync(int page)
        {
            var totalItems = await _movies.CountAsync();
            var itemsOnPage = _movies.Skip((page - 1) * this._rows).Take(this._rows).ToListAsync();
            var resultset = new ResultSet<Movie>(page, _rows, totalItems, await itemsOnPage);
            return resultset;
        }

        public async Task<List<Movie>> GetAllItemsAsync()
        {
            return await _movies.ToListAsync();
        }
    }
}
