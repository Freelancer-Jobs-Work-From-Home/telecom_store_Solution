using BussinessObject.Models;
using ClosedXML.Excel;
using Server.ViewModel.Category;
using Server.ViewModel.Product;
using Services.IServices;

namespace Server.Helper
{
    public class ExcelService
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public ExcelService(ICategoryService categoryService, IProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        public async Task<(List<ProductViewModel>, string)> PreviewExcelAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (null, "Vui lòng chọn file Excel hợp lệ!");

            List<ProductViewModel> previewData = new List<ProductViewModel>();
            string base64Excel = null;

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                base64Excel = Convert.ToBase64String(memoryStream.ToArray());
                memoryStream.Position = 0;

                using (var workbook = new XLWorkbook(memoryStream))
                {
                    var worksheet = workbook.Worksheets.First();
                    var allCategories = _categoryService.GetAll().ToDictionary(c => c.CategoryName, c => c.CategoryID);

                    foreach (var row in worksheet.RowsUsed().Skip(1))
                    {
                        var model = new ProductViewModel
                        {
                            Name = row.Cell(1).GetString(),
                            Price = decimal.TryParse(row.Cell(2).GetString(), out var p) ? p : 0,
                            Stock = int.TryParse(row.Cell(3).GetString(), out var s) ? s : 0,
                            Description = row.Cell(4).GetString(),
                            ImageURL = row.Cell(5).GetString(),
                            CategoryName = row.Cell(6).GetString()
                        };

                        if (!allCategories.TryGetValue(model.CategoryName, out var categoryId))
                            return (null, $"Danh mục '{model.CategoryName}' không tồn tại!");

                        model.CategoryId = categoryId;
                        previewData.Add(model);
                    }
                }
            }

            return (previewData, base64Excel);
        }

        public async Task<string> ImportExcelAsync(string base64Excel)
        {
            if (string.IsNullOrEmpty(base64Excel))
                return "Vui lòng chọn file Excel hợp lệ!";

            var fileBytes = Convert.FromBase64String(base64Excel);
            List<Product> products = new List<Product>();

            using (var stream = new MemoryStream(fileBytes))
            using (var workbook = new XLWorkbook(stream))
            {
                try
                {
                    var worksheet = workbook.Worksheets.First();
                    var allCategories = _categoryService.GetAll().ToDictionary(c => c.CategoryName, c => c.CategoryID);

                    using (var transaction = _productService.BeginTransaction())
                    {
                        foreach (var row in worksheet.RowsUsed().Skip(1))
                        {
                            string name = row.Cell(1).GetString();
                            if (_productService.GetByName(name) != null)
                                continue;

                            var categoryName = row.Cell(6).GetString();
                            if (!allCategories.TryGetValue(categoryName, out var categoryId))
                                return $"Danh mục '{categoryName}' không tồn tại!";

                            var product = new Product
                            {
                                ProductID = Guid.NewGuid(),
                                Name = name,
                                Price = decimal.TryParse(row.Cell(2).GetString(), out var p) ? p : 0,
                                Stock = int.TryParse(row.Cell(3).GetString(), out var s) ? s : 0,
                                Description = row.Cell(4).GetString(),
                                ImageURL = row.Cell(5).GetString(),
                                CategoryID = categoryId,
                                CreatedAt = DateTime.Now
                            };

                            products.Add(product);
                        }

                        _productService.AddRange(products);
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    return "Lỗi khi nhập dữ liệu: " + ex.Message;
                }
            }

            return "Nhập dữ liệu từ Excel thành công!";
        }

        public byte[] ExportExcel()
        {
            var products = _productService.GetAll()
                .Select(p => new
                {
                    p.Name,
                    p.Price,
                    p.Stock,
                    p.Description,
                    p.ImageURL,
                    CategoryName = _categoryService.GetById(p.CategoryID)?.CategoryName
                }).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Products");

                worksheet.Cell(1, 1).Value = "Name";
                worksheet.Cell(1, 2).Value = "Price";
                worksheet.Cell(1, 3).Value = "Stock";
                worksheet.Cell(1, 4).Value = "Description";
                worksheet.Cell(1, 5).Value = "ImageURL";
                worksheet.Cell(1, 6).Value = "CategoryName";

                for (int i = 0; i < products.Count; i++)
                {
                    var p = products[i];
                    worksheet.Cell(i + 2, 1).Value = p.Name;
                    worksheet.Cell(i + 2, 2).Value = p.Price;
                    worksheet.Cell(i + 2, 3).Value = p.Stock;
                    worksheet.Cell(i + 2, 4).Value = p.Description;
                    worksheet.Cell(i + 2, 5).Value = p.ImageURL;
                    worksheet.Cell(i + 2, 6).Value = p.CategoryName;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public async Task<(List<CategoryViewModel>, string)> PreviewCategoryExcelAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (null, "Vui lòng chọn file Excel hợp lệ!");

            List<CategoryViewModel> previewData = new List<CategoryViewModel>();
            string base64Excel = null;

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                base64Excel = Convert.ToBase64String(memoryStream.ToArray());
                memoryStream.Position = 0;

                using (var workbook = new XLWorkbook(memoryStream))
                {
                    var worksheet = workbook.Worksheets.First();

                    foreach (var row in worksheet.RowsUsed().Skip(1))
                    {
                        var name = row.Cell(1).GetString().Trim();
                        if (!string.IsNullOrEmpty(name))
                        {
                            previewData.Add(new CategoryViewModel { CategoryName = name });
                        }
                    }
                }
            }

            previewData = previewData.Distinct().ToList();
            return (previewData, base64Excel);
        }

        public async Task<string> ImportCategoryExcelAsync(string base64Excel)
        {
            if (string.IsNullOrEmpty(base64Excel))
                return "Vui lòng chọn file Excel hợp lệ!";

            var fileBytes = Convert.FromBase64String(base64Excel);
            List<Category> newCategories = new List<Category>();

            using (var stream = new MemoryStream(fileBytes))
            using (var workbook = new XLWorkbook(stream))
            {
                try
                {
                    var worksheet = workbook.Worksheets.First();
                    var existing = _categoryService.GetAll()
                                                   .Select(c => c.CategoryName)
                                                   .ToHashSet();

                    foreach (var row in worksheet.RowsUsed().Skip(1))
                    {
                        var name = row.Cell(1).GetString().Trim();
                        if (!string.IsNullOrEmpty(name) && !existing.Contains(name))
                        {
                            newCategories.Add(new Category
                            {
                                CategoryID = Guid.NewGuid(),
                                CategoryName = name
                            });
                        }
                    }

                    if (!newCategories.Any())
                        return "Không có danh mục mới cần thêm!";

                    using (var transaction = _categoryService.BeginTransaction())
                    {
                        _categoryService.AddRange(newCategories);
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    return "Lỗi khi nhập danh mục: " + ex.Message;
                }
            }

            return $"Nhập thành công {newCategories.Count} danh mục từ Excel!";
        }

        public byte[] ExportCategoryExcel()
        {
            var categories = _categoryService.GetAll()
                .Select(c => new { c.CategoryName })
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Categories");
                worksheet.Cell(1, 1).Value = "CategoryName";

                for (int i = 0; i < categories.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = categories[i].CategoryName;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}
