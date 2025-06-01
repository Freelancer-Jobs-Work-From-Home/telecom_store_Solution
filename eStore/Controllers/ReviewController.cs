using eStore.Models.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace eStore.Controllers
{
    public class ReviewController : Controller
    {
        private readonly HttpClient _httpClient;

        public ReviewController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7034/api/");
        }

        public async Task<IActionResult> GetReviews(Guid productId)
        {
            var response = await _httpClient.GetAsync($"Review/product/{productId}");
            if (!response.IsSuccessStatusCode)
                return PartialView("_ReviewList", new List<ReviewViewModel>());

            var content = await response.Content.ReadAsStringAsync();
            var reviews = JsonConvert.DeserializeObject<List<ReviewViewModel>>(content);
            return PartialView("_ReviewList", reviews);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] ReviewViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Review", content);

            if (!response.IsSuccessStatusCode)
                return BadRequest();

            return await GetReviews(model.ProductID);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteReview(Guid reviewId, Guid productId)
        {
            var response = await _httpClient.DeleteAsync($"Review/{reviewId}");
            if (!response.IsSuccessStatusCode)
                return BadRequest();

            return await GetReviews(productId);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateReview(ReviewViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("Review", content);

            if (!response.IsSuccessStatusCode)
                return BadRequest();

            return await GetReviews(model.ProductID);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetListReview()
        {
            var response = await _httpClient.GetAsync("Review");
            if (!response.IsSuccessStatusCode)
                return View(new List<ReviewViewModel>());

            var content = await response.Content.ReadAsStringAsync();
            var reviews = JsonConvert.DeserializeObject<List<ReviewViewModel>>(content);
            return View(reviews);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"Review/{id}");
            if (!response.IsSuccessStatusCode)
                TempData["Error"] = "Không thể xoá đánh giá.";

            return RedirectToAction(nameof(GetListReview));
        }
    }
}
