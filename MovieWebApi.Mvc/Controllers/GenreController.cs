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
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
            ViewBag.IsAdmin = role == "Admin";

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var client = _httpClientFactory.CreateClient("MovieWebApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

           
            var response = await client.GetAsync($"Genre?search={search}");

            if (!response.IsSuccessStatusCode)
            {
                return Content($"API Error: {response.StatusCode}");

            }
            
            var genre = await response.Content.ReadFromJsonAsync<List<GenreViewModel>>();
            return View(genre);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(GenreViewModel model)
        {
            var token = Request.Cookies["jwt"];
            var client = _httpClientFactory.CreateClient("MovieWebApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(new {name = model.Name });
            var content = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await client.PostAsync("Genre", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to create genre.");
                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id )
        {
            var token = Request.Cookies["jwt"];

            var client = _httpClientFactory.CreateClient("MovieWebApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
           
            var response = await client.GetAsync($"Genre/{id}");

            var genre = await response.Content.ReadFromJsonAsync<GenreViewModel>();

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to create genre.");
                return View(genre);
            }

            return View(genre);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(GenreViewModel model)
        {
            var token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("login ", "Auth");
            }

            var client = _httpClientFactory.CreateClient("MovieWebApi");
            client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsJsonAsync($"Genre/{model.Id}", new { name = model.Name });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to update Gnere");
                return View(model);
            }

            TempData["SuccessMessage"] = "Genre updated successfully";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var token = Request.Cookies["jwt"];
            var client = _httpClientFactory.CreateClient("MovieWebApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"Genre/{id}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to delete genre.");

            }

            TempData["SuccessMessage"] = "Genre deleted successfully.";
            return RedirectToAction("Index");
        }


    }

}
