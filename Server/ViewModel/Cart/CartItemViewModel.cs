namespace Server.ViewModel.Cart
{
    public class CartItemViewModel
    {
        public Guid ProductID { get; set; }         
        public string Name { get; set; }          
        public string ImageURL { get; set; }       
        public decimal Price { get; set; }        
        public int Quantity { get; set; }

        public int AvailableQuantity { get; set; }

        public decimal PriceTotal { get; set; }

        public decimal Discount { get; set; }
    }

}
