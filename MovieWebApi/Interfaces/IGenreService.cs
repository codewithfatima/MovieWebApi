using Microsoft.AspNetCore.Identity;
using MovieWebApi.Models;
using MovieWebApi.Dtos;


namespace MovieWebApi.Interfaces
{
    public interface IGenreService
    {
        Task<IEnumerable<GenreDto>> GetAllAsync(string? search = null ,int pageNumber = 1, int pageSize = 20);
        Task<GenreDto?> GetByIdAsync(int id);
        Task<GenreDto> AddAsync(GenreDto dto);
        Task UpdateAsync(int id, GenreDto dto);
        Task DeleteAsync(int id);

        Task SaveChangesAsync();
    }
}
