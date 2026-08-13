using Microsoft.AspNetCore.Mvc;
using MovieWebApi.Mvc.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace MovieWebApi.Mvc.Controllers
{
    public class AuthController : Controller


    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login() { return View(); }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var client = _httpClientFactory.CreateClient("MovieWebApi");
            var json = JsonSerializer.Serialize(new { email = model.Email, password = model.Password });
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            var responseJson = await response.Content.ReadAsStringAsync(); // take api reply and read its  body as p[lain text 
            var result = JsonSerializer.Deserialize<Dictionary<string, string>>(responseJson, new JsonSerializerOptions
            { 
                PropertyNameCaseInsensitive = true
            });
            var token = result["token"];

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30)
            });

            TempData["Success"] = "Login successful.";
            return RedirectToAction("Index", "Movie");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var client = _httpClientFactory.CreateClient("MovieWebApi");
            var json = JsonSerializer.Serialize(new
            {
                firstName = model.FirstName,
                lastName = model.LastName,
                email = model.Email,
                password = model.Password
            });
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Auth/register", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to register user.");
                return View(model);
            }
            TempData["Success"] = "Registration successful. Please log in.";

            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login");
        }

        

    }
}
