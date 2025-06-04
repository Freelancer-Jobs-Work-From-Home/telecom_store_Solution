using eStore.Models.Cart;
using eStore.Models.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Security.Claims;
using System.Text;

namespace eStore.Controllers
{
        public class OrderController : Controller
    {
        private readonly HttpClient _httpClient;

        public OrderController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7034/api/");
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(Guid userId, string discountCode, List<CartItemViewModel> cartItems)
        {
            if (cartItems == null || !cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("Index", "Cart");
            }

            var requestBody = new
            {
                userID = userId,
                discountCode = discountCode?? "",
                cartItems = cartItems.Select(c => new CartItemViewModel
                {
                    ProductID = c.ProductID,
                    Name = c.Name ?? "",
                    ImageURL = c.ImageURL ?? "",
                    Price = c.Price,
                    Quantity = c.Quantity,
                    AvailableQuantity = c.AvailableQuantity,
                    PriceTotal = c.PriceTotal,
                    Discount = c.Discount
                }).ToList()
        };

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var json = JsonConvert.SerializeObject(requestBody, settings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Order", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Tạo đơn hàng thất bại.";
                return RedirectToAction("Index", "Cart");
            }

            var responseData = await response.Content.ReadAsStringAsync();

            // ✅ Deserialize về kiểu rõ ràng
            var order = JsonConvert.DeserializeObject<OrderViewModel>(responseData);

            TempData["Success"] = "Đơn hàng đã được tạo thành công!";
            return RedirectToAction("OrderDetails", new { orderId = order.OrderID });
        }

        public async Task<IActionResult> OrderDetails(Guid orderId)
        {
            var response = await _httpClient.GetAsync($"Order/{orderId}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var order = JsonConvert.DeserializeObject<OrderViewModel>(content);
            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> OrderHistory()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out Guid userId))
                return RedirectToAction("Login", "Auth");

            var response = await _httpClient.GetAsync($"Order/history/{userId}");
            if (!response.IsSuccessStatusCode)
                return View(new List<OrderViewModel>());

            var content = await response.Content.ReadAsStringAsync();
            var orders = JsonConvert.DeserializeObject<List<OrderViewModel>>(content);
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            var response = await _httpClient.PostAsync($"Order/cancel/{orderId}", null);
            TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                response.IsSuccessStatusCode ? "Đơn hàng đã được huỷ thành công!" : "Không thể huỷ đơn hàng.";

            return RedirectToAction("OrderHistory");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageOrders()
        {
            var response = await _httpClient.GetAsync("Order/admin");
            if (!response.IsSuccessStatusCode)
                return View(new List<OrderViewModel>());

            var content = await response.Content.ReadAsStringAsync();
            var orders = JsonConvert.DeserializeObject<List<OrderViewModel>>(content);
            return View(orders);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageOrderDetails(Guid orderId)
        {
            var response = await _httpClient.GetAsync($"Order/admin/details/{orderId}");
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("ManageOrders");

            var content = await response.Content.ReadAsStringAsync();
            var order = JsonConvert.DeserializeObject<OrderViewModel>(content);
            return View(order);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, string status)
        {
            var json = JsonConvert.SerializeObject(new { orderID = orderId, status });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Order/admin/update-status", content);

            TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                response.IsSuccessStatusCode ? "Trạng thái đơn hàng đã được cập nhật!" : "Cập nhật trạng thái thất bại!";

            return RedirectToAction("ManageOrders");
        }


    }
}
