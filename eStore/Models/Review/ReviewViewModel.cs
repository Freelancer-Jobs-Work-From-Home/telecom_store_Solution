namespace eStore.Models.Review
{
    public class ReviewViewModel
    {
        public Guid? ReviewID { get; set; }
        public Guid UserID { get; set; }
        public Guid ProductID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UserName { get; set; } // Để hiển thị tên người dùng

        public bool isOwner { get; set; } 

        public string? ProductName { get; set; } // Để hiển thị tên sản phẩm
    }
}
