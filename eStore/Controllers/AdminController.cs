using eStore.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.IServices;

namespace eStore.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7034/api/");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("Admin/dashboard");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không thể lấy dữ liệu dashboard từ API.";
                return View(new DashboardViewModel()); 
            }

            var content = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<DashboardViewModel>(content);

            return View(model);
        }
    }
}
