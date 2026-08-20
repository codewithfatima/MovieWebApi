using MovieWebApi.Models;


namespace MovieWebApi.Models
{
    public enum MovieStatus
    {
        Active = 0,
        Postponed = 1,
        Cancelled = 2
    }
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public int Rating { get; set; }
        public MovieStatus Status { get; set; }

        public int GenreId { get; set; }
        public Genre? Genre { get; set; } = null;

    }
}
