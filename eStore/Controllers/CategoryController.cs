using eStore.Models.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace eStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly HttpClient _httpClient;

        public CategoryController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7034/api/");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("Category");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không thể tải danh mục từ API.";
                return View(new List<CategoryViewModel>());
            }

            var content = await response.Content.ReadAsStringAsync();
            var cateList = JsonConvert.DeserializeObject<List<CategoryViewModel>>(content);
            return View(cateList);
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            var response = await _httpClient.GetAsync($"Category/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<CategoryViewModel>(content);
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Category", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            TempData["Error"] = "Tạo danh mục thất bại.";
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _httpClient.GetAsync($"Category/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<CategoryViewModel>(content);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CategoryViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"Category/{id}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            TempData["Error"] = "Cập nhật danh mục thất bại.";
            return View(model);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _httpClient.GetAsync($"Category/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<CategoryViewModel>(content);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"Category/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Xóa danh mục thất bại.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
