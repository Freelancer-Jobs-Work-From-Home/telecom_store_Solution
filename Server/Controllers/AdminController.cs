using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IReviewService _reviewService;
        private readonly IFeedbackService _feedbackService;

        public AdminController(
            IOrderService orderService,
            IUserService userService,
            IReviewService reviewService,
            IFeedbackService feedbackService)
        {
            _orderService = orderService;
            _userService = userService;
            _reviewService = reviewService;
            _feedbackService = feedbackService;
        }

        [HttpGet("dashboard")]
        public IActionResult GetDashboardData()
        {
            var data = new
            {
                PendingOrders = _orderService.CountOrdersByStatus("Pending"),
                CancelledOrders = _orderService.CountOrdersByStatus("Canceled"),
                CompletedOrders = _orderService.CountOrdersByStatus("Completed"),
                UserCount = _userService.GetAll().Count,
                TotalReviews = _reviewService.GetAll().Count,
                TotalFeedbacks = _feedbackService.GetAll().Count,
                MonthlyEarnings = _orderService.GetMonthlyEarnings(),
                YearlyEarnings = _orderService.GetYearlyEarnings(),
                MonthlyEarningsData = _orderService.GetEarningsByMonth()
            };

            return Ok(data);
        }

    }
}
