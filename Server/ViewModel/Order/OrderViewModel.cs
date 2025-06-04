namespace Server.ViewModel.Order
{
    public class OrderViewModel
    {
        public Guid OrderID { get; set; }
        public Guid UserID { get; set; }
        public decimal TotalPrice { get; set; }

        public string FullName { get; set; }    
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Discount { get; set; }
        public List<OrderDetailViewModel> OrderDetails { get; set; }


    }
}
