using eStore.Models.Category;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace eStore.Components
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public CategoryMenuViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IViewComponentResult Invoke()
        {
            var categories = _categoryService.GetAll()
                .Select(c => new CategoryViewModel
                {
                    CategoryID = c.CategoryID,
                    CategoryName = c.CategoryName
                }).ToList();

            // Phân loại danh mục
            var tdCategories = categories.Where(c => c.CategoryName.StartsWith("TD:")).ToList();
            var vtCategories = categories.Where(c => c.CategoryName.StartsWith("VT:")).ToList();

            var model = new CategoryMenuViewModel
            {
                TdCategories = tdCategories,
                VtCategories = vtCategories
            };

            return View(model);
        }

    }
}
