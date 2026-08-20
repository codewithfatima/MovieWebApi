using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using MovieWebApi.Mvc.Models;
using System.Data;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MovieWebApi.Mvc.Controllers
{
    public class GenreController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public GenreController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(string? search, int pageNumber = 1, int pageSize = 20)
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

            var client = _httpClientFactory.CreateClient("MovieWebApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url =
               $"api/Genre?search={Uri.EscapeDataString(search ?? "")}" +
               $"&pageNumber={pageNumber}" +
               $"&pageSize={pageSize}";

     

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                return Content(
                    $"Genre API ERROR\n\n" +
                    $"Status: {(int)response.StatusCode} {response.StatusCode}\n\n" +
                    $"URL: {client.BaseAddress}{url}\n\n" +
                    $"Response:\n{error}"
                );
            }

            var genre =
                await response.Content.ReadFromJsonAsync<List<GenreViewModel>>();

            return View(genre ?? new List<GenreViewModel>());
            //var response = await client.GetAsync($"Genre?search={search}");

            //if (!response.IsSuccessStatusCode)
            //{
            //    return Content($"API Error: {response.StatusCode}");
            //}

            //var genre = await response.Content.ReadFromJsonAsync<List<GenreViewModel>>();
            //return View(genre);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(GenreViewModel model)
        {
            var token = Request.Cookies["jwt"];
            var client = _httpClientFactory.CreateClient("MovieWebApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(new { name = model.Name });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/Genre", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError(
                    "",
                    $"Failed to create genre: {error}"
                );

                return View(model);
            }

            TempData["Success"] =
                "Genre created successfully.";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var client = _httpClientFactory.CreateClient("MovieWebApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"api/Genre/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                return Content(
                    $"Genre API ERROR\n\n" +
                    $"Status: {(int)response.StatusCode} {response.StatusCode}\n\n" +
                    error
                );
            }

            var genre =
                await response.Content.ReadFromJsonAsync<GenreViewModel>();

            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(GenreViewModel model)
        {
            var token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var client = _httpClientFactory.CreateClient("MovieWebApi");
            client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsJsonAsync($"api/Genre/{model.Id}", new { name = model.Name });

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError(
                    "",
                    $"Failed to update Genre: {error}"
                );

                return View(model);
            }

            TempData["Success"] =
                "Genre updated successfully.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var token = Request.Cookies["jwt"];
            var client = _httpClientFactory.CreateClient("MovieWebApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"api/Genre/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                TempData["Error"] =
                    $"Failed to delete genre: {error}";

                return RedirectToAction("Index");
            }

            TempData["Success"] =
                "Genre deleted successfully.";

            return RedirectToAction("Index");
        }
    }
}