using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Server.ViewModel.Review;
using System.ComponentModel.DataAnnotations;

namespace Server.ViewModel.Product
{
    public class ProductViewModel
    {
        [Ignore]
        public Guid ProductID { get; set; }
        [Display(Name = "Tên sản phẩm")]
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        public string Name { get; set; }
        
        [Display(Name = "Miêu tả")]
        [Required(ErrorMessage = "Mô tả không được để trống")]
        public string Description { get; set; }
        
        [Display(Name = "Giá")]
        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }
        
        [Display(Name = "Số lượng")]
        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Stock { get; set; }
        
        [Display(Name = "Hình ảnh")]
        public string ImageURL { get; set; }
        [Ignore]
        public Guid CategoryId { get; set; }
        [Display(Name = "Danh mục")]
        public string CategoryName { get; set; }
        [Ignore]
        public IFormFile ImageFile { get; set; }
        [Ignore]
        public List<ReviewViewModel> Reviews { get; set; }
        [Ignore]
        public bool CanDetele { get; set; } = true;
    }

}
