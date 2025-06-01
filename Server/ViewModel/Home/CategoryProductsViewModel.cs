

using Server.ViewModel.Category;
using Server.ViewModel.Product;

namespace Server.ViewModel.Home
{
    public class CategoryProductsViewModel
    {
        public CategoryViewModel Category { get; set; }
        public List<ProductViewModel> Products { get; set; }
    }
}
