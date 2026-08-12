using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieWebApi.Dtos;
using MovieWebApi.Interfaces;
using MovieWebApi.Models;
using MovieWebApi.Services;


namespace MovieWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenreController:ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGenres([FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var genre = await _genreService.GetAllAsync(search, pageNumber, pageSize);
            return Ok(genre);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var genre = await _genreService.GetByIdAsync(id);

            if(genre == null)
            {
                return NotFound($"Genre with this {id} not found");
            }

            return Ok(genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, GenreDto dto)
        {
            var existingGenre = await _genreService.GetByIdAsync(id);
            if(existingGenre == null)
            {
                return NotFound();
            }
            await _genreService.UpdateAsync(id , dto);
                return Ok("Genre is Updated successfully");
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] GenreDto dto)
        {
             await _genreService.AddAsync(dto);
            return Ok("Genre is created successfully");

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var genre =  await _genreService.GetByIdAsync(id);

            if(genre == null)
            {
                return NotFound();
            }
            await _genreService.DeleteAsync(id);
            return Ok("Genre deleted successfully");
        }
    }
}




