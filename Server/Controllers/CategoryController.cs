using BussinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Helper;
using Server.ViewModel.Category;
using Services.IServices;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly CsvService _csvService;
        private readonly ExcelService _excelService;

        public CategoryController(ICategoryService categoryService, CsvService csvService, ExcelService excelService)
        {
            _categoryService = categoryService;
            _csvService = csvService;
            _excelService = excelService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var cateList = _categoryService.GetAll().Select(cate => new CategoryViewModel
            {
                CategoryID = cate.CategoryID,
                CategoryName = cate.CategoryName,
                CanDelete = _categoryService.CanDelete(cate.CategoryID)
            }).ToList();

            return Ok(cateList);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var cate = _categoryService.GetById(id);
            if (cate == null) return NotFound();

            return Ok(new CategoryViewModel
            {
                CategoryID = cate.CategoryID,
                CategoryName = cate.CategoryName
            });
        }

        [HttpPost]
        public IActionResult Create([FromBody] CategoryViewModel model)
        {
            var cate = new Category
            {
                CategoryID = Guid.NewGuid(),
                CategoryName = model.CategoryName,
                CreatedAt = DateTime.UtcNow
            };

            _categoryService.Add(cate);
            return Ok(new { message = "Tạo danh mục thành công." });
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] CategoryViewModel model)
        {
            var cate = _categoryService.GetById(id);
            if (cate == null) return NotFound();

            cate.CategoryName = model.CategoryName;
            cate.CreatedAt = DateTime.UtcNow;

            _categoryService.Update(cate);
            return Ok(new { message = "Cập nhật danh mục thành công." });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var cate = _categoryService.GetById(id);
            if (cate == null) return NotFound();

            _categoryService.Delete(id);
            return Ok(new { message = "Xóa danh mục thành công." });
        }
    }

}
