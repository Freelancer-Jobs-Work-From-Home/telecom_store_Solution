using BussinessObject.Models;
using eStore.Models.Coupon;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.IServices;
using System.Text;

namespace eStore.Controllers
{
    public class CouponController : Controller
    {
        private readonly HttpClient _httpClient;

        public CouponController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7034/api/");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("Coupon");
            if (!response.IsSuccessStatusCode) return View(new List<CouponViewModel>());

            var content = await response.Content.ReadAsStringAsync();
            var coupons = JsonConvert.DeserializeObject<List<CouponViewModel>>(content);
            return View(coupons);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CouponViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Coupon", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không thể tạo mã giảm giá.";
                return View(model);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _httpClient.GetAsync($"Coupon/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<CouponViewModel>(content);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CouponViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"Coupon/{model.CouponID}", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không thể cập nhật mã giảm giá.";
                return View(model);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"Coupon/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không thể xoá mã giảm giá.";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            var response = await _httpClient.GetAsync($"Coupon/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<CouponViewModel>(content);
            return View(model);
        }
    }
}
