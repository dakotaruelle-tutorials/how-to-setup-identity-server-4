using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using webclient.Models;

namespace webclient
{
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
      _logger = logger;
    }

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Privacy()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string returnUrl = "/")
    {
      // if (username == password)
      // {
      //   var claims = new List<Claim>
      //   {
      //     new Claim("sub", username),
      //   };

      //   var claimsIdentity = new ClaimsIdentity(claims, "password", "name", "role");
      //   var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

      //   await HttpContext.SignInAsync("cookieAuth", claimsPrincipal);

      //   return RedirectToAction("Claims", "Home");
      // }

      // return Unauthorized();

      return Challenge(new AuthenticationProperties
      {
        RedirectUri = returnUrl
      }, "oidc");
    }

    [Authorize]
    public async Task<IActionResult> Claims()
    {
      return View();
    }

    [Authorize]
    public async Task<IActionResult> Weather()
    {
      var accessToken = await HttpContext.GetTokenAsync("access_token");

      using var httpClient = new HttpClient();
      httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

      var weatherForecasts = await httpClient.GetFromJsonAsync<List<WeatherForecast>>("https://localhost:4001/weatherforecast");

      return View(weatherForecasts);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
