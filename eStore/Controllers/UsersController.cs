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

            string uniqueFileName = null;
            if (model.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/users");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }
                model.Avatar = "/images/users/" + uniqueFileName;
            }

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("User", content);
            if (!response.IsSuccessStatusCode) return View(model);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _httpClient.GetAsync($"User/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();
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

            if (model.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/users");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                string uniqueFileName = "/images/users/" + Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, uniqueFileName.TrimStart('/'));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }
                model.Avatar = uniqueFileName;
            }

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"User/{id}", content);
            if (!response.IsSuccessStatusCode) return View(model);

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
