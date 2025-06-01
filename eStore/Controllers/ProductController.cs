using eStore.Models.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace eStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7034/api/");
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("Product");
            if (!response.IsSuccessStatusCode) return View(new List<ProductViewModel>());

            var content = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<ProductViewModel>>(content);
            return View(products);
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            var response = await _httpClient.GetAsync($"Product/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<ProductViewModel>(content);
            return View(product);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            ModelState.Remove("CategoryName");
            ModelState.Remove("ImageURL");
            ModelState.Remove("Reviews");
            if (!ModelState.IsValid)
                return View(model);

            string uniqueFileName = null;
            if (model.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }
            }

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.Name), "Name");
            content.Add(new StringContent(model.Description), "Description");
            content.Add(new StringContent(model.Price.ToString()), "Price");
            content.Add(new StringContent(model.Stock.ToString()), "Stock");
            content.Add(new StringContent(model.CategoryId.ToString()), "CategoryId");

            if (model.ImageFile != null)
            {
                var streamContent = new StreamContent(model.ImageFile.OpenReadStream());
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.ImageFile.ContentType);
                content.Add(streamContent, "ImageFile", model.ImageFile.FileName);
            }

            var response = await _httpClient.PostAsync("Product", content);
            if (!response.IsSuccessStatusCode) TempData["Error"] = "Không thể thêm sản phẩm.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _httpClient.GetAsync($"Product/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<ProductViewModel>(content);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProductViewModel model)
        {
            ModelState.Remove("CategoryName");
            ModelState.Remove("ImageURL");
            ModelState.Remove("Reviews");
            if (!ModelState.IsValid)
                return View(model);

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.Name), "Name");
            content.Add(new StringContent(model.Description), "Description");
            content.Add(new StringContent(model.Price.ToString()), "Price");
            content.Add(new StringContent(model.Stock.ToString()), "Stock");
            content.Add(new StringContent(model.CategoryId.ToString()), "CategoryId");

            if (model.ImageFile != null)
            {
                var streamContent = new StreamContent(model.ImageFile.OpenReadStream());
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.ImageFile.ContentType);
                content.Add(streamContent, "ImageFile", model.ImageFile.FileName);
            }

            var response = await _httpClient.PutAsync($"Product/{id}", content);
            if (!response.IsSuccessStatusCode) TempData["Error"] = "Không thể cập nhật sản phẩm.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _httpClient.GetAsync($"Product/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<ProductViewModel>(content);
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"Product/{id}");
            if (!response.IsSuccessStatusCode) TempData["Error"] = "Không thể xoá sản phẩm.";

            return RedirectToAction(nameof(Index));
        }
    }

}

