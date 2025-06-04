using eStore.Helper;
using eStore.Models.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.IServices;

namespace eStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;

        public CartController(IProductService productService, ICouponService couponService)
        {
            _productService = productService;
            _couponService = couponService;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
            return View(cart);
        }

        [HttpPost]
        public IActionResult UpdateQuantity(Guid id, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
            var item = cart.FirstOrDefault(p => p.ProductID == id);

            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
            }
            else
            {
                cart.Remove(item); 
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return Json(new { cartCount = cart.Sum(p => p.Quantity), cartHTML = RenderCartHTML(cart) });
        }
        [HttpPost]
        public IActionResult AddToCart(Guid id)
        {
            Console.WriteLine($"Thêm sản phẩm vào giỏ hàng: {id}");
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
            var product = _productService.GetById(id);

            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            if (product.Stock <= 0)
            {
                return Json(new { success = false, message = "Sản phẩm đã hết hàng." });
            }

            var existingItem = cart.FirstOrDefault(p => p.ProductID == id);
            if (existingItem != null)
            {
                if (existingItem.Quantity < product.Stock)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    return Json(new { success = false, message = "Số lượng vượt quá tồn kho." });
                }
            }
            else
            {
                cart.Add(new CartItemViewModel
                {
                    ProductID = id,
                    Name = product.Name,
                    ImageURL = product.ImageURL,
                    Price = product.Price,
                    AvailableQuantity = product.Stock,
                    Quantity = 1
                });
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return Json(new { success = true, cartCount = cart.Sum(p => p.Quantity), cartHTML = RenderCartHTML(cart) });
        }


        [HttpPost]
        public IActionResult RemoveFromCart(Guid id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
            var item = cart.FirstOrDefault(p => p.ProductID == id);

            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return Json(new { cartCount = cart.Sum(p => p.Quantity), cartHTML = RenderCartHTML(cart) });
        }

        private string RenderCartHTML(List<CartItemViewModel> cart)
        {
            if (cart == null || !cart.Any())
            {
                return "<p class='text-center'>Giỏ hàng trống.</p>";
            }
            string html = "";
            foreach (var item in cart)
            {
                html += $@"
            <div class='cart_item'>
                <div class='cart_img'>
                    <a href='#'><img src='{item.ImageURL}' alt='{item.Name}'></a>
                </div>
                <div class='cart_info'>
                    <a href='#'>{item.Name}</a>
                    <span class='cart_price'>{item.Price:#,##0} VND</span>
                    <span class='quantity'>Qty: {item.Quantity}</span>
                </div>
                <div class='cart_remove'>
                    <a title='Xóa' href='#' class='remove-from-cart' data-id='{item.ProductID}'><i class='fa fa-times-circle'></i></a>
                </div>
            </div>";
            }

            
            html += @"
                    <div class='cart_button'>
                        <a href='/Cart' class='btn btn-primary'>Giỏ hàng</a>
                    </div>";
            return html;
        }

        [HttpGet]
        public IActionResult ApplyDiscount(string code)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
            decimal originalTotal = cart.Sum(p => p.Price * p.Quantity); // Tính tổng tiền chưa giảm giá

            // Lưu tổng tiền ban đầu vào session (nếu chưa có)
            if (HttpContext.Session.GetObjectFromJson<decimal>("OriginalTotal") == 0)
            {
                HttpContext.Session.SetObjectAsJson("OriginalTotal", originalTotal);
            }

            // Kiểm tra mã giảm giá
            var discount = _couponService.GetByCode(code);

            if (discount != null && discount.DiscountPercentage > 0)
            {
                decimal discountAmount = originalTotal * discount.DiscountPercentage / 100;
                decimal newTotal = originalTotal - discountAmount;

                // Lưu mã giảm giá vào session
                HttpContext.Session.SetObjectAsJson("DiscountCode", code);
                return Json(new { success = true, newTotal });
            }

            // Nếu không có mã giảm giá hợp lệ, trả về lỗi và giữ tổng tiền gốc
            return Json(new { success = false, newTotal = originalTotal });
        }

        [HttpPost]
        public IActionResult RemoveDiscount()
        {
            // Xóa mã giảm giá khỏi session
            HttpContext.Session.Remove("DiscountCode");

            // Lấy tổng tiền gốc từ session
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
            decimal originalTotal = cart.Sum(p => p.Price * p.Quantity);

            // Lưu lại tổng tiền gốc vào session (nếu chưa có)
            HttpContext.Session.SetObjectAsJson("OriginalTotal", originalTotal);

            return Json(new { success = true, newTotal = originalTotal });
        }




        [HttpGet]
        public IActionResult UpdateTotal(string discountCode)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();

            // Lấy tổng tiền ban đầu từ session
            decimal total = HttpContext.Session.GetObjectFromJson<decimal>("OriginalTotal");

            // Kiểm tra mã giảm giá
            var discount = _couponService.GetByCode(discountCode);

            if (discount != null && discount.DiscountPercentage > 0)
            {
                decimal discountAmount = total * discount.DiscountPercentage / 100;
                total -= discountAmount; // Áp dụng giảm giá nếu có
            }

            return Json(new { newTotal = total });
        }

        [HttpPost]
        public IActionResult RemoveItem(Guid productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();

            var item = cart.FirstOrDefault(c => c.ProductID == productId);
            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return Json(new { success = true });
        }




    }
}
