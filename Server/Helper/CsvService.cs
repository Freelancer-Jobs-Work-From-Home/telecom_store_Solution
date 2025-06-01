using BussinessObject.Models;
using CsvHelper.Configuration;
using CsvHelper;
using Services.IServices;
using System.Globalization;
using System.Text;
using Server.ViewModel.Category;
using Server.ViewModel.Product;
using Server.ViewModel.Csv;

namespace Server.Helper
{
    public class CsvService
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public CsvService(ICategoryService categoryService, IProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        /// <summary>
        /// Xử lý xem trước dữ liệu từ file CSV.
        /// </summary>
        public async Task<PreviewCsvResult<ProductViewModel>> PreviewCsvAsync(IFormFile file)
        {
            var result = new PreviewCsvResult<ProductViewModel>();

            if (file == null || file.Length == 0)
            {
                result.PreviewData = null;
                result.CsvFileBase64 = "Vui lòng chọn file CSV hợp lệ!";
                return result;
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                result.CsvFileBase64 = Convert.ToBase64String(memoryStream.ToArray());
            }

            using (var stream = new StreamReader(file.OpenReadStream()))
            using (var csv = new CsvReader(stream, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                try
                {
                    result.PreviewData = csv.GetRecords<ProductViewModel>().ToList();
                    var allCategories = _categoryService.GetAll().ToDictionary(c => c.CategoryName, c => c.CategoryID);

                    foreach (var item in result.PreviewData)
                    {
                        if (!allCategories.TryGetValue(item.CategoryName, out var categoryId))
                        {
                            result.PreviewData = null;
                            result.CsvFileBase64 = $"Danh mục '{item.CategoryName}' không tồn tại!";
                            return result;
                        }

                        item.CategoryId = categoryId;
                    }
                }
                catch (Exception ex)
                {
                    result.PreviewData = null;
                    result.CsvFileBase64 = "Lỗi khi đọc file CSV: " + ex.Message;
                }
            }

            return result;
        }


        /// <summary>
        /// Xử lý nhập dữ liệu từ file CSV đã xem trước.
        /// </summary>
        public async Task<string> ImportCsvAsync(string base64Csv)
        {
            if (string.IsNullOrEmpty(base64Csv))
                return "Vui lòng chọn file CSV hợp lệ!";

            var fileBytes = Convert.FromBase64String(base64Csv);
            List<Product> products = new List<Product>();

            using (var stream = new MemoryStream(fileBytes))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                try
                {
                    var records = csv.GetRecords<ProductViewModel>().ToList();
                    var allCategories = _categoryService.GetAll().ToDictionary(c => c.CategoryName, c => c.CategoryID);

                    using (var transaction = _productService.BeginTransaction())
                    {
                        foreach (var item in records)
                        {
                            if (!allCategories.TryGetValue(item.CategoryName, out var categoryId))
                                return $"Danh mục '{item.CategoryName}' không tồn tại!";

                            if (_productService.GetByName(item.Name) != null)
                                continue; // Bỏ qua sản phẩm trùng tên

                            products.Add(new Product
                            {
                                ProductID = Guid.NewGuid(),
                                Name = item.Name,
                                Price = item.Price,
                                Stock = item.Stock,
                                Description = item.Description,
                                ImageURL = item.ImageURL,
                                CategoryID = categoryId,
                                CreatedAt = DateTime.Now
                            });
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

            return "Nhập dữ liệu từ CSV thành công!";
        }

        /// <summary>
        /// Xuất danh sách sản phẩm ra file CSV.
        /// </summary>
        public byte[] ExportCsv()
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

            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                writer.Write('﻿');
                csv.WriteRecords(products);
                writer.Flush();
                return memoryStream.ToArray();
            }
        }

        //Category CSV

        /// <summary>
        /// Xử lý nhập danh mục từ file CSV.
        /// </summary>
        public async Task<string> ImportCategoryCsvAsync(string base64Csv)
        {
            if (string.IsNullOrEmpty(base64Csv))
                return "Vui lòng chọn file CSV hợp lệ!";

            var fileBytes = Convert.FromBase64String(base64Csv);

            List<Category> newCategories = new List<Category>();

            using (var stream = new MemoryStream(fileBytes))
            using (var reader = new StreamReader(stream, Encoding.UTF8, true))
            {
                // Loại bỏ BOM nếu có
                if (reader.Peek() == 0xFEFF)
                    reader.Read();

                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    IgnoreBlankLines = true,
                    TrimOptions = TrimOptions.Trim,
                    BadDataFound = null,
                    MissingFieldFound = null,
                    HeaderValidated = null  // Bỏ kiểm tra Header nếu cần
                }))
                {
                    try
                    {
                        var records = csv.GetRecords<CategoryViewModel>().ToList();

                        if (!records.Any())
                            return "File CSV không có dữ liệu hợp lệ!";

                        var existingCategories = _categoryService.GetAll()
                                                                 .Select(c => c.CategoryName)
                                                                 .ToHashSet();

                        foreach (var item in records)
                        {
                            if (string.IsNullOrWhiteSpace(item.CategoryName))
                                continue;

                            if (!existingCategories.Contains(item.CategoryName))
                            {
                                newCategories.Add(new Category
                                {
                                    CategoryID = Guid.NewGuid(),
                                    CategoryName = item.CategoryName.Trim()
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
            }

            return $"Nhập thành công {newCategories.Count} danh mục từ CSV!";
        }



        /// <summary>
        /// Xử lý xem trước danh mục từ file CSV.
        /// </summary>
        public async Task<(List<CategoryViewModel>, string)> PreviewCategoryCsvAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (null, "Vui lòng chọn file CSV hợp lệ!");

            List<CategoryViewModel> previewData = new List<CategoryViewModel>();
            string base64Csv = null;

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                base64Csv = Convert.ToBase64String(memoryStream.ToArray());
            }

            // Đọc toàn bộ nội dung file và loại bỏ BOM
            using (var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8, true))
            {
                string content = await stream.ReadToEndAsync();
                content = content.Trim('\uFEFF'); // Loại bỏ BOM nếu có

                using (var reader = new StringReader(content))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    try
                    {
                        previewData = csv.GetRecords<CategoryViewModel>()
                                         .Select(c => new CategoryViewModel { CategoryName = c.CategoryName.Trim() })
                                         .Where(c => !string.IsNullOrEmpty(c.CategoryName))
                                         .Distinct()
                                         .ToList();
                    }
                    catch (Exception ex)
                    {
                        return (null, "Lỗi khi đọc file CSV: " + ex.Message);
                    }
                }
            }

            return (previewData, base64Csv);
        }


        /// <summary>
        /// Xuất danh mục ra file CSV.
        /// </summary>
        public byte[] ExportCategoryCsv()
        {
            var categories = _categoryService.GetAll()
                .Select(c => new { c.CategoryName })
                .ToList();

            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                writer.Write('﻿'); // Đảm bảo UTF-8 BOM để đọc đúng tiếng Việt
                csv.WriteRecords(categories);
                writer.Flush();
                return memoryStream.ToArray();
            }
        }



    }
}
