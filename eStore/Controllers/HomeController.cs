using eStore.Helper;
using eStore.Models;
using eStore.Models.Home;
using eStore.Models.Product;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;


namespace eStore.Controllers
{
    
    public class HomeController : Controller
    {
        
        private readonly HttpClient _httpClient;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7034/api/"); 
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("Home");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không thể tải dữ liệu từ API.";
                return View(new HomeViewModel()); // fallback
            }

            var content = await response.Content.ReadAsStringAsync();
            var viewModel = JsonConvert.DeserializeObject<HomeViewModel>(content);

            ViewBag.Categories = viewModel.Categories;
            return View(viewModel);
        }
        public async Task<IActionResult> ProductDetail(Guid id)
        {
            var response = await _httpClient.GetAsync($"Home/product/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<ProductViewModel>(content);

            return View(product);
        }


        public async Task<IActionResult> ListCategory(Guid category)
        {
            var response = await _httpClient.GetAsync($"Home/category/{category}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<CategoryProductsViewModel>(content);

            return View(model);
        }


        public async Task<IActionResult> FilterProducts(string search, Guid? category, decimal? minPrice, decimal? maxPrice)
        {
            var url = $"Home/filter?search={search}&category={category}&minPrice={minPrice}&maxPrice={maxPrice}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode) return PartialView("_ProductListPartial", new List<ProductViewModel>());

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<ProductViewModel>>(content);

            return PartialView("_ProductListPartial", result);
        }


    }


}
