using Microsoft.EntityFrameworkCore;
using MovieWebApi.Data;
using MovieWebApi.Dtos;
using MovieWebApi.Interfaces;
using MovieWebApi.Models;
using System.Threading.Tasks;

namespace MovieWebApi.Repositories
{
    public class GenreRepository: IGenreRepository
    {
        private readonly AppDbContext _context;

        public GenreRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Genre>> GetAllAsync(string? search = null, int pageNumber = 1, int pageSize = 20)
        {
            var query = _context.Genres.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(g => g.Name.Contains(search));
            }

            return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        public async Task<Genre?> GetByIdAsync(int id)
            => await _context.Genres.FindAsync(id);

        public async Task AddAsync( Genre  genre)
        {
            await _context.Genres.AddAsync(genre);
        }

        public void Delete(Genre genre)
            => _context.Genres.Remove(genre);

        public void Update(Genre genre)
            => _context.Genres.Update(genre);

        public async Task SaveChangesAsync() 
            => await _context.SaveChangesAsync();


    }
}

