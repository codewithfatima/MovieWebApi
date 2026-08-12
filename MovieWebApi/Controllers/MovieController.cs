using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieWebApi.Dtos;
using MovieWebApi.Interfaces;

namespace MovieWebApi.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController:ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
           _movieService = movieService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovies(
     [FromQuery] string? search, [FromQuery] int? genreId,
     [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
     [FromQuery] int? status = null)
        {
            var (movies, totalCount) = await _movieService.GetAllAsync(search, genreId, pageNumber, pageSize, status);
            return Ok(new { items = movies, totalCount });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MovieDto>> GetByIdAsync(int id)
        {
            var movie = await _movieService.GetByIdAsync(id);   
            if (movie == null)
            {
                return NotFound();
            }
            return Ok(movie);  
        }

        [HttpPost]
        public async Task<ActionResult<MovieDto>> CreateAsync([FromBody] CreateMovieDto dto )
        {
            var createdMovie = await _movieService.CreateAsync(dto);
            return Ok("Moviee is Created successfully");
        }

        //Task UpdateAsync(int id, UpdateMovieDto updateMovieDto);

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var movie = await _movieService.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            await _movieService.DeleteAsync(id);
            return Ok("Moviee is deleted successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, UpdateMovieDto dto)
        {
            var movie = await _movieService.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            await _movieService.UpdateAsync(id, dto);
            return Ok("Movie updated successfully");
        }
    }
}




