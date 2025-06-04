using eStore.Models.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using eStore.Models.Category;
using System.Text;
using BussinessObject.Models;
using OfficeOpenXml;
using System.Globalization;


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
            var products = new List<ProductViewModel>();

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                products = JsonConvert.DeserializeObject<List<ProductViewModel>>(content);
            }

            var categoryResponse = await _httpClient.GetAsync("Category");
            if (categoryResponse.IsSuccessStatusCode)
            {
                var categoryContent = await categoryResponse.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(categoryContent);

                ViewBag.Categories = categories;
            }
            else
            {
                ViewBag.Categories = new List<CategoryViewModel>();
            }

            return View(products);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _httpClient.GetAsync($"Product/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<ProductViewModel>(content);
            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            var categoryResponse = await _httpClient.GetAsync("Category");
            if (categoryResponse.IsSuccessStatusCode)
            {
                var categoryContent = await categoryResponse.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(categoryContent);

                ViewBag.Categories = categories;
            }
            else
            {
                ViewBag.Categories = new List<CategoryViewModel>();
            }
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
            content.Add(new StringContent(model.Description, Encoding.UTF8), "Description");
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

            var categoryResponse = await _httpClient.GetAsync("Category");
            if (categoryResponse.IsSuccessStatusCode)
            {
                var categoryContent = await categoryResponse.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(categoryContent);

                ViewBag.Categories = categories;
            }
            else
            {
                ViewBag.Categories = new List<CategoryViewModel>();
            }
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

        public IActionResult ImportProductExcel()
        {
            return View("ImportProductExcel"); // Gọi đúng view ImportProductExcel.cshtml
        }

        [HttpPost]
        public IActionResult PreviewProductExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn file Excel.";
                return View();
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Quan trọng

            var previewList = new List<ProductViewModel>();

            try
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        int rows = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rows; row++) // Bỏ qua header
                        {
                            var name = worksheet.Cells[row, 1].Text?.Trim();
                            var priceText = worksheet.Cells[row, 2].Text?.Trim();
                            var stockText = worksheet.Cells[row, 3].Text?.Trim();
                            var description = worksheet.Cells[row, 4].Text?.Trim();
                            var imageUrl = worksheet.Cells[row, 5].Text?.Trim();
                            var categoryName = worksheet.Cells[row, 6].Text?.Trim();

                            decimal price = 0;
                            int stock = 0;

                            decimal.TryParse(priceText, out price);
                            int.TryParse(stockText, out stock);

                            // Bạn có thể kiểm tra thêm các điều kiện bắt buộc ở đây nếu muốn

                            previewList.Add(new ProductViewModel
                            {
                                Name = name,
                                Price = price,
                                Stock = stock,
                                Description = description,
                                ImageURL = imageUrl,
                                CategoryName = categoryName
                            });
                        }
                    }
                }

                ViewBag.PreviewData = previewList;
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi đọc file Excel: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessProductExcel(bool confirmed)
        {
            if (!confirmed)
                return RedirectToAction(nameof(ImportProductExcel));

            // Ở đây bạn có thể lấy lại dữ liệu từ ViewBag hoặc Session, hoặc tốt nhất
            // nên lưu tạm vào Session trong PreviewProductExcel và lấy lại ở đây
            // Ví dụ: Bạn có thể truyền lại JSON trong form ẩn, hoặc lưu vào temp storage

            // Giả sử bạn nhận dữ liệu JSON trong form (cần thêm input hidden chứa JSON)
            var jsonData = Request.Form["PreviewDataJson"];
            if (string.IsNullOrEmpty(jsonData))
            {
                TempData["Error"] = "Không có dữ liệu để nhập.";
                return RedirectToAction(nameof(ImportProductExcel));
            }

            var products = JsonConvert.DeserializeObject<List<ProductViewModel>>(jsonData);

            foreach (var product in products)
            {
                // Tạo đối tượng MultipartFormDataContent tương tự Create action để gọi API tạo mới
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(product.Name), "Name");
                content.Add(new StringContent(product.Description ?? ""), "Description");
                content.Add(new StringContent(product.Price.ToString()), "Price");
                content.Add(new StringContent(product.Stock.ToString()), "Stock");

                // Lấy CategoryId từ tên danh mục, hoặc để mặc định GUID.Empty
                Guid categoryId = Guid.Empty;

                // Bạn cần gọi API Category lấy danh sách và lấy Id tương ứng với CategoryName
                // (Bạn nên cache danh mục hoặc làm sao lấy được categoryId)
                var categoryResponse = await _httpClient.GetAsync("Category");
                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categoryContent = await categoryResponse.Content.ReadAsStringAsync();
                    var categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(categoryContent);
                    var cat = categories.FirstOrDefault(c => c.CategoryName == product.CategoryName);
                    if (cat != null) categoryId = cat.CategoryID;
                }
                content.Add(new StringContent(categoryId.ToString()), "CategoryId");

                // ImageURL có thể là link, bạn không xử lý ảnh ở đây
                // Nếu bạn muốn upload ảnh thì cần thêm xử lý riêng

                var response = await _httpClient.PostAsync("Product", content);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Có lỗi xảy ra khi nhập sản phẩm: " + product.Name;
                    return RedirectToAction(nameof(ImportProductExcel));
                }
            }

            TempData["Success"] = "Nhập dữ liệu thành công!";
            return RedirectToAction(nameof(Index));
        }
    }

}

