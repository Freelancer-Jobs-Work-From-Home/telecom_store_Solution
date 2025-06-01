using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.ViewModel.Category;
using Server.ViewModel.Feedback;
using Server.ViewModel.Home;
using Server.ViewModel.Product;
using Server.ViewModel.Review;
using Services.IServices;
using System.Security.Claims;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IReviewService _reviewService;
        private readonly IUserService _userService;
        private readonly IFeedbackService _feedbackService;

        public HomeController(
            IProductService productService,
            ICategoryService categoryService,
            IReviewService reviewService,
            IUserService userService,
            IFeedbackService feedbackService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _reviewService = reviewService;
            _userService = userService;
            _feedbackService = feedbackService;
        }

        // GET: api/Home
        [HttpGet]
        public IActionResult GetHomeData()
        {
            var categories = _categoryService.GetAll()
                .ToDictionary(c => c.CategoryID, c => c.CategoryName);

            var allProducts = _productService.GetAll()
                .Select(p => new ProductViewModel
                {
                    ProductID = p.ProductID,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    ImageURL = p.ImageURL,
                    CategoryId = p.CategoryID,
                    CategoryName = categories.ContainsKey(p.CategoryID) ? categories[p.CategoryID] : "Unknown"
                }).ToList();

            var telecomProducts = allProducts.Where(p => p.CategoryName.StartsWith("VT:")).ToList();
            var consumerProducts = allProducts.Where(p => p.CategoryName.StartsWith("TD:")).ToList();

            var feedbacks = _feedbackService.GetAll()
                .Select(f => new FeedbackViewModel
                {
                    FeedbackID = f.FeedbackID,
                    FullName = _userService.GetById(f.UserID)?.FullName,
                    Message = f.Message,
                    UserID = f.UserID,
                    CreatedAt = f.CreatedAt,
                }).ToList();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            FeedbackViewModel feedbackVM = null;

            if (!string.IsNullOrEmpty(userId))
            {
                var user = _userService.GetById(Guid.Parse(userId));
                feedbackVM = new FeedbackViewModel
                {
                    FullName = user.FullName,
                    UserID = user.UserID,
                    Email = user.Email
                };
            }

            var result = new HomeViewModel
            {
                TelecomProducts = telecomProducts,
                ConsumerProducts = consumerProducts,
                Categories = categories.Select(c => new CategoryViewModel
                {
                    CategoryID = c.Key,
                    CategoryName = c.Value
                }).ToList(),
                Feedbacks = feedbacks,
                FeedbackViewModel = feedbackVM
            };

            return Ok(result);
        }

        // GET: api/Home/product/{id}
        [HttpGet("product/{id}")]
        public IActionResult GetProductDetail(Guid id)
        {
            var product = _productService.GetById(id);
            if (product == null) return NotFound();

            var reviews = _reviewService.GetReviewsByProductId(id)
                .Select(r => new ReviewViewModel
                {
                    ReviewID = r.ReviewID,
                    UserID = r.UserID,
                    ProductID = r.ProductID,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UserName = _userService.GetById(r.UserID)?.FullName
                }).ToList();

            var productViewModel = new ProductViewModel
            {
                ProductID = product.ProductID,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageURL = product.ImageURL,
                CategoryId = product.CategoryID,
                CategoryName = _categoryService.GetById(product.CategoryID)?.CategoryName,
                Reviews = reviews
            };

            return Ok(productViewModel);
        }

        // GET: api/Home/category/{categoryId}
        [HttpGet("category/{categoryId}")]
        public IActionResult GetProductsByCategory(Guid categoryId)
        {
            var categoryDetails = _categoryService.GetById(categoryId);
            if (categoryDetails == null) return NotFound();

            var products = _productService.GetProductsByCategory(categoryId)
                .Select(p => new ProductViewModel
                {
                    ProductID = p.ProductID,
                    Name = p.Name,
                    Price = p.Price,
                    Stock = p.Stock,
                    ImageURL = p.ImageURL,
                    CategoryId = p.CategoryID,
                    CategoryName = categoryDetails.CategoryName,
                    Description = p.Description
                }).ToList();

            var model = new CategoryProductsViewModel
            {
                Category = new CategoryViewModel
                {
                    CategoryID = categoryDetails.CategoryID,
                    CategoryName = categoryDetails.CategoryName
                },
                Products = products
            };

            return Ok(model);
        }

        // GET: api/Home/filter
        [HttpGet("filter")]
        public IActionResult FilterProducts(string? search, Guid? category, decimal? minPrice, decimal? maxPrice)
        {
            var products = _productService.GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(search))
                products = products.Where(p => p.Name.Contains(search));

            if (category.HasValue)
                products = products.Where(p => p.CategoryID == category.Value);

            if (minPrice.HasValue)
                products = products.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                products = products.Where(p => p.Price <= maxPrice.Value);

            var result = products.Select(p => new ProductViewModel
            {
                ProductID = p.ProductID,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock,
                ImageURL = p.ImageURL,
                CategoryName = _categoryService.GetById(p.CategoryID) != null
                                ? _categoryService.GetById(p.CategoryID).CategoryName
                                : "Unknown",
            }).ToList();

            return Ok(result);
        }
    }

}
