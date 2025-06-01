using System.ComponentModel.DataAnnotations;

namespace eStore.Models.Coupon
{
    public class CouponViewModel
    {
        public Guid CouponID { get; set; } = Guid.NewGuid();
        [Display(Name = "Mã Code")]
        [Required(ErrorMessage = "Mã Code không được để trống")]
        public string Code { get; set; }
        [Display(Name = "Phần trăm giảm giá")]
        [Required(ErrorMessage = "Phần trăm giảm giá không được để trống")]
        [Range(1, 100)]
        public decimal DiscountPercentage { get; set; }
        [Display(Name = "Ngày hết hạn")]
        public DateTime ExpiryDate { get; set; }
        
    }
}
