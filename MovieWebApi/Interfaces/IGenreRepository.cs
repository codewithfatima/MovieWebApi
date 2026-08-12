using MovieWebApi.Dtos;
using MovieWebApi.Models;

namespace MovieWebApi.Interfaces
{
    public interface IGenreRepository
    {
        Task<IEnumerable<Genre>> GetAllAsync(string? search=null ,  int pageNumber = 1, int pageSize = 20 ); 
        Task<Genre?> GetByIdAsync(int id);
        Task AddAsync(Genre genre);
        void Update(Genre genre);
        void Delete(Genre genre);
        Task SaveChangesAsync();
    }
}
