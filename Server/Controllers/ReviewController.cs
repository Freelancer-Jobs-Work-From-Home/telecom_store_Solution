using BussinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.ViewModel.Review;
using Services.IServices;
using System.Security.Claims;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;

        public ReviewController(IReviewService reviewService, IUserService userService, IProductService productService)
        {
            _reviewService = reviewService;
            _userService = userService;
            _productService = productService;
        }

        // GET: api/review/product/{productId}
        [HttpGet("product/{productId}")]
        public IActionResult GetReviews(Guid productId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

            var reviews = _reviewService.GetReviewsByProductId(productId)
                .Select(r => new ReviewViewModel
                {
                    ReviewID = r.ReviewID,
                    UserID = r.UserID,
                    ProductID = r.ProductID,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UserName = _userService.GetById(r.UserID)?.FullName,
                    isOwner = r.UserID == userId
                }).ToList();

            return Ok(reviews);
        }

        // POST: api/review
        [HttpPost]
        public IActionResult AddReview([FromBody] ReviewViewModel model)
        {

            var review = new Review
            {
                UserID = model.UserID,
                ProductID = model.ProductID,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _reviewService.Add(review);
            return Ok(new { message = "Đánh giá đã được thêm." });
        }

        // PUT: api/review
        [HttpPut]
        public IActionResult UpdateReview([FromBody] ReviewViewModel model)
        {
            var review = _reviewService.GetById(model.ReviewID);
            if (review == null) return NotFound();

            review.Rating = model.Rating;
            review.Comment = model.Comment;
            _reviewService.Update(review);

            return Ok(new { message = "Đã cập nhật đánh giá." });
        }

        // DELETE: api/review/{reviewId}
        [HttpDelete("{reviewId}")]
        public IActionResult DeleteReview(Guid reviewId)
        {
            var review = _reviewService.GetById(reviewId);
            if (review == null) return NotFound();

            _reviewService.Delete(reviewId);
            return Ok(new { message = "Đã xóa đánh giá." });
        }

        // ADMIN - GET: api/review/admin
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllReviews()
        {
            var reviews = _reviewService.GetAll()
                .Select(r => new ReviewViewModel
                {
                    ReviewID = r.ReviewID,
                    ProductID = r.ProductID,
                    ProductName = _productService.GetById(r.ProductID)?.Name,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UserID = r.UserID,
                    UserName = _userService.GetById(r.UserID)?.FullName
                }).ToList();

            return Ok(reviews);
        }

        // ADMIN - DELETE: api/review/admin/{id}
        [HttpDelete("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteReviewAsAdmin(Guid id)
        {
            var review = _reviewService.GetById(id);
            if (review == null) return NotFound();

            _reviewService.Delete(id);
            return Ok(new { message = "Admin đã xóa đánh giá thành công." });
        }
    }

}
