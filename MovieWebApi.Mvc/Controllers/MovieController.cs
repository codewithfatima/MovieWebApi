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

        // Helper method to attach JWT token to HttpClient requests
        private HttpClient CreateAuthorizedClient()
        {
            var client = _httpClientFactory.CreateClient("MovieWebApi");
            var token = Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
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

            var role = jwtToken.Claims
                .FirstOrDefault(c =>
                    c.Type == System.Security.Claims.ClaimTypes.Role)
                ?.Value;

            ViewBag.IsAdmin = role == "Admin";

            int pageSize = 5;

            var client = CreateAuthorizedClient();

            var url =
                $"api/Movie?search={Uri.EscapeDataString(search ?? "")}" +
                $"&genreId={genreId}" +
                $"&pageNumber={pageNumber}" +
                $"&pageSize={pageSize}" +
                $"&status={status}";

            Console.WriteLine("JWT FOUND: " + !string.IsNullOrEmpty(token));
            Console.WriteLine("API URL: " + client.BaseAddress + url);
            Console.WriteLine("AUTH HEADER: " + client.DefaultRequestHeaders.Authorization);

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                return Content(
                    $"Movie API ERROR\n\n" +
                    $"Status: {(int)response.StatusCode} {response.StatusCode}\n\n" +
                    $"URL: {client.BaseAddress}{url}\n\n" +
                    $"Response:\n{error}"
                );
            }

            var json = await response.Content.ReadAsStringAsync();

            var movie = JsonSerializer.Deserialize<MovieViewModel>(
                json,
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

            ViewBag.TotalPages = (int)Math.Ceiling(
                (double)movie.TotalCount / pageSize
            );

            return View(movie.Items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");
            return View();
        }

       [HttpPost]
public async Task<IActionResult> Create(MovieViewModel model)
{
    if (!ModelState.IsValid)
    {
        ModelState.AddModelError("", "Please check all movie fields.");
        return View(model);
    }

    var token = Request.Cookies["jwt"];

    if (string.IsNullOrEmpty(token))
    {
        return RedirectToAction("Login", "Auth");
    }

    var client = CreateAuthorizedClient();

    var json = JsonSerializer.Serialize(model);

    Console.WriteLine("CREATE MOVIE REQUEST:");
    Console.WriteLine(json);
    Console.WriteLine("API URL: " + client.BaseAddress + "api/Movie");
    Console.WriteLine("AUTH: " + client.DefaultRequestHeaders.Authorization);

    var content = new StringContent(
        json,
        Encoding.UTF8,
        "application/json"
    );

    var response = await client.PostAsync(
        "api/Movie",
        content
    );

    var responseBody = await response.Content.ReadAsStringAsync();

    Console.WriteLine(
        $"CREATE MOVIE RESPONSE: {(int)response.StatusCode} {response.StatusCode}"
    );

    Console.WriteLine(
        $"CREATE MOVIE BODY: {responseBody}"
    );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError(
                    "",
                    $"Create failed: {(int)response.StatusCode} {response.StatusCode}"
                );

                ModelState.AddModelError("", error);

                return View(model);
            }
            TempData["SuccessMessage"] = "Movie created successfully!";

    return RedirectToAction("Index");
}

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthorizedClient();

            var response = await client.GetAsync($"api/Movie/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                return Content(
                    $"Movie API Error: {(int)response.StatusCode} {response.StatusCode}\n\n" +
                    error
                );
            }

            var json = await response.Content.ReadAsStringAsync();

            var movie = JsonSerializer.Deserialize<MovieViewModel>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, MovieViewModel model)
        {
            var client = CreateAuthorizedClient(); // Uses token

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"api/Movie/{id}", jsonContent); if (!response.IsSuccessStatusCode)
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
            var client = CreateAuthorizedClient(); // Uses token
            var response = await client.DeleteAsync($"api/Movie/{id}");
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Unable to delete the Movie");
            }

            return RedirectToAction("Index");
        }

        [HttpPost("{id}/duplicate")]
        public async Task<IActionResult> Duplicate(int id)
        {
            var client = CreateAuthorizedClient(); // Uses token
            var response = await client.PostAsync($"api/Movie/{id}/duplicate", null);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to duplicate movie.");
            }

            TempData["SuccessMessage"] = "Movie duplicated successfully.";
            return RedirectToAction("Index");
        }
    }
}