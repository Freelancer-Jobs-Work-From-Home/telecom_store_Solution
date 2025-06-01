using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eStore.Models.User
{
    public class UserViewModel
    {
        public Guid UserID { get; set; }
        [Display(Name = "Họ và Tên")]
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        public string FullName { get; set; }
        [Display(Name = "Địa chỉ email/gmail")]
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        [Display(Name = "Hình ảnh")]
        public string Avatar { get; set; }
        [Required (ErrorMessage = "Mật khẩu không được để trống")]
        [Display(Name = "Mật khẩu")]

        public string Password { get; set; }
        [Display(Name = "Ngày sinh")]
        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        public DateTime DateOfBirth { get; set; }
        [Display(Name ="Vai trò")]
        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        public string Role { get; set; }
        public IFormFile? ImageFile { get; set; }

        public bool CanDetele { get; set; } = true;
    }
}
