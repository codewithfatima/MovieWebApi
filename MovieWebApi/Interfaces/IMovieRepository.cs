using MovieWebApi.Models;
using System.Threading.Tasks;

namespace MovieWebApi.Interfaces
{
    public interface IMovieRepository
    {
        Task<(IEnumerable<Movie> Movies, int TotalCount)> GetAllAsync(string? search = null, int? genreId = null, int pageNumber = 1, int pageSize = 10 , int? status=null);
        Task<Movie?> GetByIdAsync(int id);

        Task AddAsync(Movie movie);

        void Update(Movie movie);

        void Delete(Movie movie);

        Task SaveChangesAsync();
    }
}
