
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.ViewModel.Cart;
using Services.IServices;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;

        public CartController(IProductService productService, ICouponService couponService)
        {
            _productService = productService;
            _couponService = couponService;
        }

        // POST: api/cart/add
        [HttpPost("add")]
        public IActionResult AddToCart([FromBody] CartItemViewModel item)
        {
            var product = _productService.GetById(item.ProductID);
            if (product == null) return NotFound(new { message = "Sản phẩm không tồn tại." });
            if (product.Stock < item.Quantity) return BadRequest(new { message = "Không đủ hàng trong kho." });

            // Giả định frontend lưu cart state, server không cần lưu session
            var resultItem = new CartItemViewModel
            {
                ProductID = product.ProductID,
                Name = product.Name,
                ImageURL = product.ImageURL,
                Price = product.Price,
                Quantity = item.Quantity,
                AvailableQuantity = product.Stock
            };

            return Ok(resultItem);
        }

        // POST: api/cart/update
        [HttpPost("update")]
        public IActionResult UpdateQuantity([FromBody] CartItemViewModel model)
        {
            var product = _productService.GetById(model.ProductID);
            if (product == null) return NotFound();

            if (model.Quantity > product.Stock)
                return BadRequest(new { message = "Số lượng vượt quá tồn kho." });

            return Ok(new { message = "Cập nhật thành công.", quantity = model.Quantity });
        }

        // DELETE: api/cart/remove/{id}
        [HttpDelete("remove/{id}")]
        public IActionResult RemoveFromCart(Guid id)
        {
            return Ok(new { message = "Đã xóa sản phẩm khỏi giỏ hàng." });
        }

        // GET: api/cart/apply-discount?code=XXX&total=Y
        [HttpGet("apply-discount")]
        public IActionResult ApplyDiscount(string code, decimal total)
        {
            var discount = _couponService.GetByCode(code);
            if (discount == null || discount.ExpiryDate <= DateTime.UtcNow)
                return BadRequest(new { success = false, message = "Mã không hợp lệ.", newTotal = total });

            var discountAmount = total * discount.DiscountPercentage / 100;
            var newTotal = total - discountAmount;

            return Ok(new { success = true, newTotal, discountPercentage = discount.DiscountPercentage });
        }

        // GET: api/cart/remove-discount?total=X
        [HttpGet("remove-discount")]
        public IActionResult RemoveDiscount(decimal total)
        {
            return Ok(new { success = true, newTotal = total });
        }

        // GET: api/cart/update-total?code=XXX&total=Y
        [HttpGet("update-total")]
        public IActionResult UpdateTotal(string code, decimal total)
        {
            var discount = _couponService.GetByCode(code);
            if (discount == null || discount.ExpiryDate <= DateTime.UtcNow)
                return Ok(new { newTotal = total });

            var discountAmount = total * discount.DiscountPercentage / 100;
            return Ok(new { newTotal = total - discountAmount });
        }
    }

}
