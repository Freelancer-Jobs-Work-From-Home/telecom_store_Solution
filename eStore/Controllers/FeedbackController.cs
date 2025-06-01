using BussinessObject.Models;
using eStore.Models.Feedback;
using eStore.Models.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.IServices;
using Services.Services;
using System.Security.Claims;
using System.Text;

namespace eStore.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly HttpClient _httpClient;

        public FeedbackController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7034/api/");
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> SubmitFeedback(FeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Index.cshtml", model);
            }

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Feedback", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Gửi phản hồi thất bại.";
                return RedirectToAction("Index", "Home");
            }

            TempData["SuccessFeedbackMessage"] = "Cảm ơn bạn đã gửi phản hồi!";
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("Feedback");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không thể tải danh sách phản hồi.";
                return View(new List<FeedbackViewModel>());
            }

            var content = await response.Content.ReadAsStringAsync();
            var feedbacks = JsonConvert.DeserializeObject<List<FeedbackViewModel>>(content);
            return View(feedbacks);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"Feedback/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không thể xoá phản hồi.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
