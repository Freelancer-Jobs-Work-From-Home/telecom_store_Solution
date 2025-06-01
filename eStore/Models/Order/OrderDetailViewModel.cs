namespace eStore.Models.Order
{
    public class OrderDetailViewModel
    {
        public Guid ProductID { get; set; }

        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
