namespace MovieWebApi.Mvc.Models
{
    public class GenreViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
}


//"The API sends data in JSON format. My frontend receives it fine, but can't " +
//    "directly use it as a real C# object — because JSON is just text, and text has no .Name or" +
//    " .Id properties. So I need GenreViewModel to convert that text into a real object my code can actually work with."