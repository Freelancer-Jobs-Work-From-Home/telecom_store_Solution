using eStore.Models.Category;
using eStore.Models.Product;

namespace eStore.Models.Home
{
    public class CategoryProductsViewModel
    {
        public CategoryViewModel Category { get; set; }
        public List<ProductViewModel> Products { get; set; }
    }
}
