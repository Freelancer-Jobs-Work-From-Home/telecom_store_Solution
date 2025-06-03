using BussinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Server.Helper;
using Server.ViewModel.Product;
using Services.IServices;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly CsvService _csvService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ExcelService _excelService;

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            IWebHostEnvironment webHostEnvironment,
            CsvService csvService,
            ExcelService excelService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _webHostEnvironment = webHostEnvironment;
            _csvService = csvService;
            _excelService = excelService;
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _categoryService.GetAll().ToDictionary(c => c.CategoryID, c => c.CategoryName);

            var result = _productService.GetAll().Select(pro => new ProductViewModel
            {
                ProductID = pro.ProductID,
                Name = pro.Name,
                Price = pro.Price,
                Stock = pro.Stock,
                ImageURL = pro.ImageURL,
                CategoryName = categories.ContainsKey(pro.CategoryID) ? categories[pro.CategoryID] : "Unknown",
                CanDetele = _productService.CanDelete(pro.ProductID)
            }).ToList();

            return Ok(result);
        }


        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var product = _productService.GetById(id);
            if (product == null) return NotFound();

            var viewModel = new ProductViewModel
            {
                ProductID = product.ProductID,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                Description = product.Description,
                ImageURL = product.ImageURL,
                CategoryId = product.CategoryID,
                CategoryName = _categoryService.GetById(product.CategoryID)?.CategoryName
            };

            return Ok(viewModel);
        }

        [HttpPost]
        public IActionResult Create([FromForm] ProductFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Xử lý lưu hình ảnh nếu có
            string imageUrl = string.Empty;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                // Đường dẫn vật lý tới thư mục Upload/images/products (nằm ngoài wwwroot)
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "images", "products");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Tạo tên file duy nhất
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Ghi file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(fileStream);
                }

                // Lưu đường dẫn tương đối (trình duyệt có thể truy cập qua /Upload/...)
                imageUrl = "/Upload/images/products/" + uniqueFileName;
            }

            // Tạo đối tượng Product để lưu vào DB
            var newProduct = new Product
            {
                ProductID = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                ImageURL = imageUrl,
                CategoryID = model.CategoryId
            };

            _productService.Add(newProduct);

            return Ok(new { message = "Tạo sản phẩm thành công", productId = newProduct.ProductID });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] ProductFormModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = _productService.GetById(id);
            if (product == null)
                return NotFound();

            string uniqueFileName = product.ImageURL;

            if (model.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                if (!string.IsNullOrEmpty(product.ImageURL))
                {
                    string oldPath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageURL.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                uniqueFileName = "/images/products/" + Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, uniqueFileName.TrimStart('/'));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }
            }

            product.Name = model.Name;
            product.Price = model.Price;
            product.Stock = model.Stock;
            product.Description = model.Description;
            product.ImageURL = uniqueFileName;
            product.CategoryID = model.CategoryId;
            product.CreatedAt = DateTime.UtcNow;

            _productService.Update(product);
            return Ok(new { message = "Cập nhật sản phẩm thành công." });
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var product = _productService.GetById(id);
            if (product == null) return NotFound();

            _productService.Delete(id);
            return Ok(new { message = "Xóa sản phẩm thành công." });
        }

    }

}
