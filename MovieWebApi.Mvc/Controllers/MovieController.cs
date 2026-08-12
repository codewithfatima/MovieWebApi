using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MovieWebApi.Mvc.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MovieWebApi.Mvc.Controllers
{
    public class MovieController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MovieController(IHttpClientFactory httpClientFactory)
        {
                _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(
     string? search,
     int? genreId,
     int pageNumber = 1,
     int? status = null)
        {
            var token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
            ViewBag.IsAdmin = role == "Admin";

            // 5 records per page
            int pageSize = 5;

            var client = _httpClientFactory.CreateClient("MovieWebApi");

            var url = $"Movie?search={Uri.EscapeDataString(search ?? "")}" +
                      $"&genreId={genreId}" +
                      $"&pageNumber={pageNumber}" +
                      $"&pageSize={pageSize}" +
                      $"&status={status}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Status: {response.StatusCode}";
                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = 0;

                return View(new List<MovieViewModel>());
            }

            var json = await response.Content.ReadAsStringAsync();

            var movie = JsonSerializer.Deserialize<MovieViewModel>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (movie == null)
            {
                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = 0;

                return View(new List<MovieViewModel>());
            }

            ViewBag.CurrentPage = pageNumber;

            // 5 records per page
            ViewBag.TotalPages = (int)Math.Ceiling(
                (double)movie.TotalCount / pageSize
            );

            return View(movie.Items);
        }



        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(MovieViewModel model)
        {
            var client = _httpClientFactory.CreateClient("MovieWebApi");

            // Serialize the incoming view model into a JSON payload
            var jsonContent  = new StringContent(
                JsonSerializer.Serialize(model),System.Text.Encoding.UTF8,"application/json");

            // Send a POST request to the backend API
            var response = await client.PostAsync("Movie", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to create movie.");
                return View(model);
            }

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
           var client = _httpClientFactory.CreateClient("MovieWebApi");
           var response = await client.GetAsync($"Movie/{id}");

            if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }
    
             var json = await response.Content.ReadAsStringAsync();
             var movie = JsonSerializer.Deserialize<MovieViewModel>(json, new JsonSerializerOptions
             {
                 PropertyNameCaseInsensitive = true
             });

            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, MovieViewModel model)
        {
            var client = _httpClientFactory.CreateClient("MovieWebApi");
            // Serialize the incoming view model into a JSON payload
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            // Send a PUT request to the backend API



            var response = await client.PutAsync($"Movie/{id}", jsonContent);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to update movie.");
                var error = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError("", $"Status: {response.StatusCode}");
                ModelState.AddModelError("", error);

           
            return View(model);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("MovieWebApi");
            var response = await client.DeleteAsync($"Movie/{id}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Unable to delte the Movie"); 
            }


            return RedirectToAction("Index");

        }

        [HttpPost("{id}/duplicate")]

        public async Task<IActionResult> Duplicate(int id)
        {
            var token = Request.Cookies["jwt"];
            var client = _httpClientFactory.CreateClient("MovieWebApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsync($"Movie/{id}/duplicate", null);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to duplicate movie.");
            }

          

            TempData["SuccessMessage"] = "Movie duplicated successfully.";
            return RedirectToAction("Index");
        }

    }
}
