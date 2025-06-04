using eStore.Models.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

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

        // View Import CSV
        public IActionResult ImportCsv()
        {
            return View(new List<CategoryViewModel>());
        }

        // Xem trước dữ liệu CSV
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PreviewCategoryCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn file CSV hợp lệ.";
                return View(new List<CategoryViewModel>());
            }

            List<CategoryViewModel> categories = new List<CategoryViewModel>();

            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                };

                using (var stream = file.OpenReadStream())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                using (var csv = new CsvReader(reader, config))
                {
                    var records = csv.GetRecords<CategoryViewModel>();
                    categories = records.ToList();
                }

                if (categories.Count == 0)
                {
                    TempData["Error"] = "File CSV không chứa dữ liệu hợp lệ.";
                    return View(new List<CategoryViewModel>());
                }

                // Truyền dữ liệu ra view để xem trước
                ViewBag.PreviewData = categories;
                return View(categories);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi đọc file CSV: {ex.Message}";
                return View(new List<CategoryViewModel>());
            }
        }

        // Xử lý import CSV sau khi người dùng xác nhận
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessCsvImport(string PreviewDataJson, string confirmed)
        {
            if (confirmed != "true")
            {
                TempData["Error"] = "Chưa xác nhận nhập dữ liệu.";
                return RedirectToAction(nameof(ImportCsv));
            }

            try
            {
                var categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(PreviewDataJson);

                if (categories == null || categories.Count == 0)
                {
                    TempData["Error"] = "Dữ liệu nhập không hợp lệ.";
                    return RedirectToAction(nameof(ImportCsv));
                }

                foreach (var cate in categories)
                {
                    var json = JsonConvert.SerializeObject(cate);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("Category", content);
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "Một số danh mục không thể được tạo.";
                    }
                }

                TempData["Success"] = $"Đã nhập thành công {categories.Count} danh mục.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi nhập dữ liệu: {ex.Message}";
                return RedirectToAction(nameof(ImportCsv));
            }
        }
        public async Task<IActionResult> ExportCsv()
        {
            var response = await _httpClient.GetAsync("Category");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không thể tải danh mục từ API.";
                return RedirectToAction(nameof(Index));
            }

            var content = await response.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(content);

            if (categories == null || categories.Count == 0)
            {
                TempData["Error"] = "Không có dữ liệu để xuất.";
                return RedirectToAction(nameof(Index));
            }

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords(categories);
                streamWriter.Flush();

                var result = memoryStream.ToArray();

                return File(result, "text/csv", "CategoriesExport.csv");
            }
        }

    }
}
