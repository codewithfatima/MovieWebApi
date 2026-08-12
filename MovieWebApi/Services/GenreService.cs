using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Identity.Client;
using MovieWebApi.Dtos;
using MovieWebApi.Interfaces;
using MovieWebApi.Models;
using System.Reflection;
using System.Threading.Tasks;

namespace MovieWebApi.Services
{
    public class GenreService:IGenreService
    {
        private readonly IGenreRepository _repository;
        private readonly ILogger<GenreService> _logger;

        public GenreService(IGenreRepository repository, ILogger<GenreService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<GenreDto>> GetAllAsync(string? search = null, int pageNumber = 1, int pageSize = 20)
        {
            var genre = await _repository.GetAllAsync(search, pageNumber, pageSize);

            if (genre == null)
            {
                return Enumerable.Empty<GenreDto>();
            }

            var genreDtos = genre.Select(g => new GenreDto
            {
                Id = g.Id,
                Name = g.Name,
            });

            return genreDtos;
        }
        public async Task<GenreDto?> GetByIdAsync(int id)
        {
            var geners = await _repository.GetByIdAsync(id);

            if(geners == null)
            {
                return null;
            }

            var genreDto = new GenreDto
            {
                 Id = geners.Id,
                Name = geners.Name,
            };

          
            return genreDto;
        }
        public async Task<GenreDto> AddAsync(GenreDto dto)
        {
            var genre = new Genre
            {
                Name = dto.Name
            };
            await _repository.AddAsync(genre);
            await _repository.SaveChangesAsync();

            return new GenreDto
            {
                Id = genre.Id,
                Name = dto.Name
            };
        }

       public  async Task UpdateAsync(int id, GenreDto dto)
        {
            var existingGenre = await _repository.GetByIdAsync(id);

            if(existingGenre == null)
            {
                return;
            }

            existingGenre.Name = dto.Name;

             _repository.Update(existingGenre);
            await _repository.SaveChangesAsync();

        }
        public async Task DeleteAsync(int id)
        {
           var genre = await _repository.GetByIdAsync(id);
            if (genre == null)
            {
                return;
            }

            _repository.Delete(genre);
            await _repository.SaveChangesAsync();


        }

        public async Task SaveChangesAsync()
        {
            await _repository.SaveChangesAsync();
        }


    }
}





