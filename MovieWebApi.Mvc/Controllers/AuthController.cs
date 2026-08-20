using Microsoft.AspNetCore.Mvc;
using MovieWebApi.Mvc.Models;
using System.Text;
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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please enter a valid email and password.";
                return View(model);
            }

            try
            {
                var client = _httpClientFactory.CreateClient("MovieWebApi");

                var json = JsonSerializer.Serialize(new
                {
                    email = model.Email,
                    password = model.Password
                });

                var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                );

                Console.WriteLine($"LOGIN: Sending request for {model.Email}");

                var response = await client.PostAsync(
                    "api/Auth/login",
                    content
                );

                var responseJson =
                    await response.Content.ReadAsStringAsync();

                Console.WriteLine(
                    $"LOGIN: API returned {(int)response.StatusCode} {response.StatusCode}"
                );

                Console.WriteLine(
                    $"LOGIN RESPONSE: {responseJson}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode ==
                        System.Net.HttpStatusCode.Unauthorized)
                    {
                        TempData["Error"] =
                            "Login failed: Invalid email or password.";
                    }
                    else
                    {
                        TempData["Error"] =
                            $"Login failed: API returned {(int)response.StatusCode}.";
                    }

                    return View(model);
                }

                var result =
                    JsonSerializer.Deserialize<Dictionary<string, string>>(
                        responseJson,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }
                    );

                if (result == null ||
                    !result.TryGetValue("token", out var token) ||
                    string.IsNullOrWhiteSpace(token))
                {
                    TempData["Error"] =
                        "Login failed: API did not return a valid token.";

                    return View(model);
                }

                Console.WriteLine("=================================");
                Console.WriteLine("JWT TOKEN RECEIVED: " + !string.IsNullOrWhiteSpace(token));
                Console.WriteLine("JWT LENGTH: " + token?.Length);
                Console.WriteLine("HOST: " + Request.Host);
                Console.WriteLine("HTTPS: " + Request.IsHttps);
                Console.WriteLine("=================================");

                Response.Cookies.Append(
           "jwt",
           token,
           new CookieOptions
           {
               HttpOnly = true,
               Secure = false,
               SameSite = SameSiteMode.Lax,
               Path = "/",
               Expires = DateTimeOffset.UtcNow.AddMinutes(30)
           }
       );

                Console.WriteLine(
                    $"LOGIN SUCCESS: {model.Email}"
                );

                TempData["Success"] =
                    $"Welcome back, {model.Email}! Login successful.";

                return RedirectToAction(
                    "Index",
                    "Movie"
                );
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(
                    $"LOGIN HTTP ERROR: {ex.Message}"
                );

                TempData["Error"] =
                    "Unable to connect to the Movie API. Please try again later.";

                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"LOGIN ERROR: {ex}"
                );

                TempData["Error"] =
                    "An unexpected error occurred during login.";

                return View(model);
            }
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] =
                    "Please correct the registration form.";

                return View(model);
            }

            try
            {
                var client =
                    _httpClientFactory.CreateClient("MovieWebApi");

                var json = JsonSerializer.Serialize(new
                {
                    firstName = model.FirstName,
                    lastName = model.LastName,
                    email = model.Email,
                    password = model.Password
                });

                var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                );

                Console.WriteLine(
                    $"REGISTER: Creating user {model.Email}"
                );

                var response = await client.PostAsync(
                    "api/Auth/register",
                    content
                );

                var responseJson =
                    await response.Content.ReadAsStringAsync();

                Console.WriteLine(
                    $"REGISTER: API returned {(int)response.StatusCode} {response.StatusCode}"
                );

                Console.WriteLine(
                    $"REGISTER RESPONSE: {responseJson}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] =
                        $"Registration failed: {responseJson}";

                    return View(model);
                }

                Console.WriteLine(
                    $"REGISTER SUCCESS: {model.Email}"
                );

                TempData["Success"] =
                    "Registration successful! You can now log in.";

                return RedirectToAction("Login");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(
                    $"REGISTER HTTP ERROR: {ex.Message}"
                );

                TempData["Error"] =
                    "Unable to connect to the Movie API. Please try again later.";

                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"REGISTER ERROR: {ex}"
                );

                TempData["Error"] =
                    "An unexpected error occurred during registration.";

                return View(model);
            }
        }


        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");

            TempData["Success"] =
                "You have been logged out successfully.";

            return RedirectToAction("Login");
        }
    }
}