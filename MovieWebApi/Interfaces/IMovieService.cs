using MovieWebApi.Dtos;
using MovieWebApi.Models;

namespace MovieWebApi.Interfaces
{
    public interface IMovieService
    {
        Task<(IEnumerable<MovieDto> Movies, int TotalCount)> GetAllAsync(string? search = null , int? genreId = null, int pageNumber = 1, int pageSize = 10 , int? status=null);

        Task<MovieDto?> GetByIdAsync(int id);

        Task<MovieDto> CreateAsync(CreateMovieDto dto);
        Task UpdateAsync(int id, UpdateMovieDto updateMovieDto);
        
        Task DeleteAsync(int id);
    }
}
