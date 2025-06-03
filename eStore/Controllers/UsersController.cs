using Microsoft.AspNetCore.Mvc;
using eStore.Models.User;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Text;

namespace eStore.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsersController(IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7034/api/");
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("User");
            if (!response.IsSuccessStatusCode)
                return View(new List<UserViewModel>());

            var content = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<UserViewModel>>(content);
            return View(users);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _httpClient.GetAsync($"User/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<UserViewModel>(content);
            return View(user);
        }

        public IActionResult Create() => View(new UserViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            ModelState.Remove("Avatar");
            if (!ModelState.IsValid) return View(model);

            var formData = new MultipartFormDataContent();

            // Thêm các trường văn bản
            formData.Add(new StringContent(model.FullName ?? ""), "FullName");
            formData.Add(new StringContent(model.Email ?? ""), "Email");
            formData.Add(new StringContent(model.Password ?? ""), "Password");
            formData.Add(new StringContent(model.Address ?? ""), "Address");
            formData.Add(new StringContent(model.Role ?? "User"), "Role");

            if (model.DateOfBirth != null)
            {
                formData.Add(new StringContent(model.DateOfBirth.ToString("yyyy-MM-dd")), "DateOfBirth");

            }

            // Thêm file nếu có
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var fileStream = model.ImageFile.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.ImageFile.ContentType);

                formData.Add(fileContent, "ImageFile", model.ImageFile.FileName);
            }

            var response = await _httpClient.PostAsync("User", formData);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", "Tạo người dùng thất bại: " + error);
                return View(model);
            }

            TempData["SuccessMessage"] = "Tạo người dùng thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _httpClient.GetAsync($"User/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<UserViewModel>(content);

            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UserViewModel model)
        {
            ModelState.Remove("ImageFile");
            ModelState.Remove("Avatar");
            if (!ModelState.IsValid) return View(model);


            if (!string.IsNullOrEmpty(model.Password) || !string.IsNullOrEmpty(model.ConfirmPassword))
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Nếu không chọn ảnh mới, giữ ảnh cũ
            if (model.ImageFile == null || model.ImageFile.Length == 0)
            {
                var currentUser = await _httpClient.GetFromJsonAsync<UserViewModel>($"User/{id}");
                model.Avatar = currentUser?.Avatar;
            }

            // Tạo form dữ liệu gửi đi (multipart/form-data)
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(model.FullName ?? ""), "FullName");
            form.Add(new StringContent(model.Email ?? ""), "Email");
            form.Add(new StringContent(model.Address ?? ""), "Address");
            form.Add(new StringContent(model.Role ?? ""), "Role");
            form.Add(new StringContent(model.DateOfBirth.ToString("o")), "DateOfBirth");

            // Chỉ gửi mật khẩu nếu có nhập (để tránh xóa mật khẩu cũ trên server)
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                form.Add(new StringContent(model.Password), "Password");
                form.Add(new StringContent(model.ConfirmPassword ?? ""), "ConfirmPassword");
            }

            // Gửi ảnh đại diện mới (nếu có)
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var fileContent = new StreamContent(model.ImageFile.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.ImageFile.ContentType);
                form.Add(fileContent, "ImageFile", model.ImageFile.FileName);
            }
            var response = await _httpClient.PutAsync($"User/{id}", form);

            if (!response.IsSuccessStatusCode)
            {
                return View(model);
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _httpClient.GetAsync($"User/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<UserViewModel>(content);
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"User/{id}");
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> UserProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Auth");

            var response = await _httpClient.GetAsync($"User/{userId}");
            if (!response.IsSuccessStatusCode) return RedirectToAction("Login", "Auth");

            var content = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<UserViewModel>(content);
            return View(user);
        }

    }
}
