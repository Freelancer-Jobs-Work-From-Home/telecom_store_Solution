using BussinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.ViewModel.Cart;
using Server.ViewModel.Order;
using Services.IServices;
using System.Transactions;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;
        private readonly IOrderService _orderService;
        private readonly ICouponService _couponService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public OrderController(IOrderDetailService orderDetailService,
                               IOrderService orderService,
                               ICouponService couponService,
                               IProductService productService,
                               IUserService userService)
        {
            _orderDetailService = orderDetailService;
            _orderService = orderService;
            _couponService = couponService;
            _productService = productService;
            _userService = userService;
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] OrderRequest request)
        {
            if (request.CartItems == null || !request.CartItems.Any())
                return BadRequest(new { error = "Giỏ hàng trống!" });

            decimal totalPrice;
            try
            {
                totalPrice = CalculateTotalPrice(request.CartItems, request.DiscountCode);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }

            var order = CreateOrderObject(request.UserID, totalPrice, request.DiscountCode);
            _orderService.Add(order);

            var cartList = CreateCartList(request.UserID, request.CartItems);
            var orderDetails = CreateOrderDetails(order.OrderID, cartList);
            _orderDetailService.AddRange(orderDetails);

            var user = _userService.GetById(order.UserID);

            var result = new OrderViewModel
            {
                OrderID = order.OrderID,
                UserID = order.UserID,
                TotalPrice = order.TotalPrice,
                FullName = string.IsNullOrEmpty(user?.FullName) ? user?.Email : user.FullName,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                Discount = order.DiscountCode,
                OrderDetails = orderDetails.Select(od => new OrderDetailViewModel
                {
                    ProductID = od.ProductID,
                    ProductName = _productService.GetById(od.ProductID)?.Name ?? "Unknown",
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList()
            };

            return Ok(result);
        }

        [HttpGet("{orderId}")]
        public IActionResult GetOrderDetails(Guid orderId)
        {
            var order = _orderService.GetById(orderId);
            if (order == null) return NotFound();

            var user = _userService.GetById(order.UserID);
            var userName = string.IsNullOrEmpty(user?.FullName) ? user?.Email : user.FullName;

            var orderDetails = _orderDetailService.GetAll()
                .Where(o => o.OrderID == orderId)
                .Select(od => new OrderDetailViewModel
                {
                    ProductID = od.ProductID,
                    ProductName = _productService.GetById(od.ProductID)?.Name ?? "Unknown",
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList();

            var orderVM = new OrderViewModel
            {
                OrderID = order.OrderID,
                UserID = order.UserID,
                FullName = userName,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                OrderDetails = orderDetails
            };

            return Ok(orderVM);
        }

        [HttpGet("history/{userId}")]
        public IActionResult GetOrderHistory(Guid userId)
        {
            var orders = _orderService.GetAll()
                .Where(o => o.UserID == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            var orderIds = orders.Select(o => o.OrderID).ToList();
            var orderDetails = _orderDetailService.GetAll().Where(od => orderIds.Contains(od.OrderID)).ToList();

            var result = orders.Select(order => new OrderViewModel
            {
                OrderID = order.OrderID,
                UserID = order.UserID,
                FullName = _userService.GetById(order.UserID)?.FullName ?? "Unknown",
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                OrderDetails = orderDetails
                    .Where(od => od.OrderID == order.OrderID)
                    .Select(od => new OrderDetailViewModel
                    {
                        ProductID = od.ProductID,
                        ProductName = _productService.GetById(od.ProductID)?.Name ?? "Unknown",
                        Quantity = od.Quantity,
                        Price = od.Price
                    }).ToList()
            });

            return Ok(result);
        }

        [HttpPost("cancel/{orderId}")]
        public IActionResult CancelOrder(Guid orderId)
        {
            var order = _orderService.GetById(orderId);
            if (order == null || order.Status != "Pending")
                return BadRequest(new { error = "Không thể hủy đơn hàng này." });

            order.Status = "Canceled";
            _orderService.Update(order);

            return Ok(new { message = "Đơn hàng đã được hủy." });
        }

        // Các hàm admin giữ nguyên
        // (không cần sửa vì không phụ thuộc vào xác thực người dùng qua claims)

        #region Helpers

        private decimal CalculateTotalPrice(List<CartItemViewModel> cartItems, string discountCode)
        {
            decimal total = cartItems.Sum(i => i.Price * i.Quantity);

            if (!string.IsNullOrEmpty(discountCode))
            {
                var coupon = _couponService.GetByCode(discountCode);
                if (coupon == null || coupon.ExpiryDate <= DateTime.UtcNow)
                    throw new InvalidOperationException("Mã giảm giá không hợp lệ hoặc đã hết hạn");

                total -= total * coupon.DiscountPercentage / 100;
            }

            return total;
        }

        private Order CreateOrderObject(Guid userId, decimal totalPrice, string discountCode)
        {
            return new Order
            {
                OrderID = Guid.NewGuid(),
                UserID = userId,
                TotalPrice = totalPrice,
                DiscountCode = discountCode ?? "",
                CreatedAt = DateTime.UtcNow,
                Status = "Pending"
            };
        }

        private List<Cart> CreateCartList(Guid userId, List<CartItemViewModel> cartItems)
        {
            return cartItems.Select(item => new Cart
            {
                CartID = Guid.NewGuid(),
                UserID = userId,
                ProductID = item.ProductID,
                Quantity = item.Quantity,
                AddedAt = DateTime.UtcNow
            }).ToList();
        }

        private List<OrderDetail> CreateOrderDetails(Guid orderId, List<Cart> cartList)
        {
            return cartList.Select(cart => new OrderDetail
            {
                OrderDetailID = Guid.NewGuid(),
                OrderID = orderId,
                ProductID = cart.ProductID,
                Quantity = cart.Quantity,
                Price = _productService.GetById(cart.ProductID)?.Price ?? 0
            }).ToList();
        }

        #endregion
    }

    public class OrderRequest
    {
        public Guid UserID { get; set; }
        public string DiscountCode { get; set; }
        public List<CartItemViewModel> CartItems { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        public Guid OrderID { get; set; }
        public string Status { get; set; }
    }
}
