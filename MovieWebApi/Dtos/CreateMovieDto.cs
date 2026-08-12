using MovieWebApi.Models;

namespace MovieWebApi.Dtos
{
    public class CreateMovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public int Rating { get; set; }
        public int GenreId { get; set; }
        public MovieStatus Status { get; set; }
    }
}
