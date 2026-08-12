using Microsoft.EntityFrameworkCore;
using MovieWebApi.Data;
using MovieWebApi.Interfaces;
using MovieWebApi.Models;
using System.Threading.Tasks;

namespace MovieWebApi.Repositories
{
    public class MovieRepository:IMovieRepository
    {
        private readonly AppDbContext _context;

        public MovieRepository (AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Movie> Movies, int TotalCount)> GetAllAsync(string? search = null, int? genreId = null, int pageNumber = 1, int pageSize = 10, int? status = null)
        {
            var query = _context.Movies.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m => m.Title.Contains(search));
            }
            if (genreId.HasValue)
            {
                query = query.Where(m => m.GenreId == genreId.Value);
            }
            if (status.HasValue)
            {
                query = query.Where(m => (int)m.Status == status.Value);
            }

            var totalCount = await query.CountAsync();

            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            var movies = await query.ToListAsync();

            return (movies, totalCount);
        }

        public async Task<Movie?> GetByIdAsync(int id)
            => await _context.Movies.FindAsync(id);

        public async Task AddAsync(Movie movie)
            => await _context.Movies.AddAsync(movie);

        public void Update(Movie movie)
            => _context.Movies.Update(movie);

        public void Delete(Movie movie)
            => _context.Movies.Remove(movie);

        public async Task  SaveChangesAsync()
            => await _context.SaveChangesAsync();

    }
}




