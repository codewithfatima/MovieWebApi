namespace MovieWebApi.Mvc.Models
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public int Rating { get; set; }
        public int GenreId { get; set; }
        public int? Status { get; set; }
        public List<MovieViewModel> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
